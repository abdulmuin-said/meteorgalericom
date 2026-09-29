using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using KanvasProje.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace KanvasProje.Service.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly ISiteSettingsService _siteSettingsService;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(
            IConfiguration config,
            IWebHostEnvironment env,
            ISiteSettingsService siteSettingsService,
            ILogger<SmtpEmailService> logger)
        {
            _config = config;
            _env = env;
            _siteSettingsService = siteSettingsService;
            _logger = logger;
        }

        public async Task SendMailAsync(string to, string subject, string body)
        {
            await SendMailInternalAsync(to, subject, body);
        }

        private async Task SendMailInternalAsync(string to, string subject, string body, string? inlineLogoPath = null)
        {
            var host = _config["EmailSettings:Host"] ?? "smtp.gmail.com";
            var port = int.TryParse(_config["EmailSettings:Port"], out var parsedPort) ? parsedPort : 587;
            var enableSsl = bool.TryParse(_config["EmailSettings:EnableSSL"], out var parsedSsl) ? parsedSsl : true;
            var username = _config["EmailSettings:Username"] ?? string.Empty;
            var password = _config["EmailSettings:Password"] ?? string.Empty;
            var siteSettings = _siteSettingsService.GetSettings();
            var brandName = string.IsNullOrWhiteSpace(siteSettings.MarkaAdi) ? siteSettings.SiteAdi : siteSettings.MarkaAdi;
            var fromEmail = _config["EmailSettings:FromEmail"];
            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                fromEmail = siteSettings.Email;
            }
            var fromName = _config["EmailSettings:FromName"];
            if (string.IsNullOrWhiteSpace(fromName))
            {
                fromName = brandName;
            }

            if (!TryCreateMailAddress(fromEmail, fromName, out var fromAddress) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("SMTP e-posta ayarlari eksik oldugu icin mail gonderimi atlandi. Subject={Subject}", subject);
                throw new InvalidOperationException("SMTP ayarlari eksik. Gonderici e-posta, Brevo kullanici adi ve SMTP anahtari kontrol edilmelidir.");
            }

            if (IsBrevoSmtpLoginAddress(fromEmail))
            {
                _logger.LogWarning("Brevo SMTP login adresi gonderici olarak kullanilamaz. Brevo panelinde dogrulanmis bir sender e-posta adresi tanimlanmalidir. Subject={Subject}", subject);
                throw new InvalidOperationException("Brevo SMTP login adresi gonderici olarak kullanilamaz. Dogrulanmis sender e-posta adresi tanimlanmalidir.");
            }

            if (!TryCreateMailAddress(to, null, out var toAddress))
            {
                _logger.LogWarning("Gecersiz alici e-posta adresi nedeniyle mail gonderimi atlandi. To={To}, Subject={Subject}", to, subject);
                throw new InvalidOperationException("Alici e-posta adresi gecersiz.");
            }

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl,
                Timeout = 30000
            };

            using var mailMessage = new MailMessage
            {
                From = fromAddress,
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            if (!string.IsNullOrWhiteSpace(inlineLogoPath) && File.Exists(inlineLogoPath))
            {
                var htmlView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
                var logoResource = new LinkedResource(inlineLogoPath, "image/png")
                {
                    ContentId = "canvasia-logo",
                    TransferEncoding = TransferEncoding.Base64
                };

                logoResource.ContentType.Name = "canvasia-logo.png";
                htmlView.LinkedResources.Add(logoResource);
                mailMessage.AlternateViews.Add(htmlView);
            }

            mailMessage.To.Add(toAddress);
            await client.SendMailAsync(mailMessage).WaitAsync(TimeSpan.FromSeconds(30));
        }

        public async Task SendTemplateMailAsync(string to, string baslik, string adSoyad, string icerik, string btnLink = "", string btnYazi = "")
        {
            var siteSettings = _siteSettingsService.GetSettings();
            var brandName = string.IsNullOrWhiteSpace(siteSettings.MarkaAdi) ? siteSettings.SiteAdi : siteSettings.MarkaAdi;
            var siteUrl = _siteSettingsService.BuildAbsoluteUrl(string.Empty);
            var logoCandidates = new[]
            {
                (FilePath: Path.Combine(_env.WebRootPath, "EmailTemplates", "canvasia-logo.png"), Url: "/EmailTemplates/canvasia-logo.png"),
                (FilePath: Path.Combine(_env.WebRootPath, "img", "canvasia-logo.png"), Url: "/img/canvasia-logo.png")
            };
            var logoUrl = logoCandidates
                .Where(x => File.Exists(x.FilePath))
                .Select(x => _siteSettingsService.BuildAbsoluteUrl(x.Url))
                .FirstOrDefault() ?? _siteSettingsService.BuildAbsoluteUrl("/EmailTemplates/canvasia-logo.png");
            var instagramUrl = string.IsNullOrWhiteSpace(siteSettings.InstagramUrl) ? siteUrl : siteSettings.InstagramUrl;
            var contactSeparator = !string.IsNullOrWhiteSpace(siteSettings.Email) && !string.IsNullOrWhiteSpace(siteSettings.Telefon)
                ? "|"
                : string.Empty;
            var path = Path.Combine(_env.WebRootPath, "EmailTemplates", "Sablon.html");
            var body = await File.ReadAllTextAsync(path);

            var butonGorunum = string.IsNullOrEmpty(btnLink) ? "none" : "block";

            body = body.Replace("{BASLIK}", baslik)
                       .Replace("{ADSOYAD}", adSoyad)
                       .Replace("{ICERIK}", icerik)
                       .Replace("{BUTON_GORUNUM}", butonGorunum)
                       .Replace("{BUTON_LINK}", btnLink)
                       .Replace("{BUTON_YAZI}", btnYazi)
                       .Replace("{SITE_ADI}", siteSettings.SiteAdi)
                       .Replace("{MARKA_ADI}", brandName)
                       .Replace("{SITE_URL}", siteUrl)
                       .Replace("{SITE_LOGO_URL}", logoUrl)
                       .Replace("{SITE_EMAIL}", siteSettings.Email)
                       .Replace("{SITE_PHONE}", siteSettings.Telefon)
                       .Replace("{SITE_CONTACT_SEPARATOR}", contactSeparator)
                       .Replace("{INSTAGRAM_URL}", instagramUrl);

            await SendMailInternalAsync(to, baslik, body);
        }

        public async Task<bool> SendKargoNotificationEmail(string toEmail, string musteriAdi, string siparisNo, string kargoFirmasi, string kargoTakipNo)
        {
            try
            {
                var subject = $"Sipari\u015Finiz Kargoya Verildi - {siparisNo}";
                var safeSiparisNo = WebUtility.HtmlEncode(siparisNo);
                var safeKargoFirmasi = WebUtility.HtmlEncode(kargoFirmasi);
                var safeKargoTakipNo = WebUtility.HtmlEncode(kargoTakipNo);
                var content = $@"
                    <p>Sipari&#351; numaran&#305;z <strong>{safeSiparisNo}</strong> olan &uuml;r&uuml;n&uuml;n&uuml;z kargoya verildi.</p>
                    <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='border:1px solid #e5e2dc; border-radius:12px; background:#fffaf0; margin:18px 0;'>
                        <tr>
                            <td style='padding:16px; border-bottom:1px solid #e5e2dc; color:#47473d;'>
                                <strong style='color:#313511;'>Kargo Firmas&#305;:</strong> {safeKargoFirmasi}
                            </td>
                        </tr>
                        <tr>
                            <td style='padding:16px; color:#47473d;'>
                                <strong style='color:#313511;'>Takip Numaras&#305;:</strong> <span style='font-size:18px; color:#b58735; font-weight:700;'>{safeKargoTakipNo}</span>
                            </td>
                        </tr>
                    </table>
                    <p>Kargonuzu a&#351;a&#287;&#305;daki ba&#287;lant&#305;dan takip edebilirsiniz.</p>
                    <div style='text-align:center; margin:24px 0;'>{GetKargoTrackingLink(kargoFirmasi, kargoTakipNo)}</div>";

                await SendTemplateMailAsync(toEmail, subject, musteriAdi, content, "", "");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kargo email hatasi: {ex.Message}");
                return false;
            }
        }

        private string GetKargoTrackingLink(string kargoFirmasi, string takipNo)
        {
            var firma = (kargoFirmasi ?? string.Empty).ToLowerInvariant();
            var encodedTakipNo = Uri.EscapeDataString(takipNo ?? string.Empty);
            var safeTakipNo = WebUtility.HtmlEncode(takipNo);

            if (firma.Contains("aras"))
            {
                return $"<a href='https://kargotakip.araskargo.com.tr/mainpage.aspx?code={encodedTakipNo}' style='display:inline-block; background:#313511; color:#ffffff; padding:13px 24px; text-decoration:none; border-radius:999px; font-size:12px; font-weight:700; letter-spacing:.08em; text-transform:uppercase;'>Aras Kargo'da Takip Et</a>";
            }

            if (firma.Contains("yurtici") || firma.Contains("yurti\u00E7i"))
            {
                return $"<a href='https://www.yurticikargo.com/tr/online-servisler/gonderi-sorgula?code={encodedTakipNo}' style='display:inline-block; background:#313511; color:#ffffff; padding:13px 24px; text-decoration:none; border-radius:999px; font-size:12px; font-weight:700; letter-spacing:.08em; text-transform:uppercase;'>Yurti\u00E7i Kargo'da Takip Et</a>";
            }

            if (firma.Contains("mng"))
            {
                return $"<a href='https://www.mngkargo.com.tr/tracking?q={encodedTakipNo}' style='display:inline-block; background:#313511; color:#ffffff; padding:13px 24px; text-decoration:none; border-radius:999px; font-size:12px; font-weight:700; letter-spacing:.08em; text-transform:uppercase;'>MNG Kargo'da Takip Et</a>";
            }

            if (firma.Contains("ptt"))
            {
                return $"<a href='https://gonderitakip.ptt.gov.tr/Track/Verify?q={encodedTakipNo}' style='display:inline-block; background:#313511; color:#ffffff; padding:13px 24px; text-decoration:none; border-radius:999px; font-size:12px; font-weight:700; letter-spacing:.08em; text-transform:uppercase;'>PTT Kargo'da Takip Et</a>";
            }

            return $"<div style='background:#fffaf0; border:1px solid #e5e2dc; padding:15px; border-radius:12px; text-align:center;'><strong>Takip No:</strong> {safeTakipNo}</div>";
        }

        private static bool TryCreateMailAddress(string? address, string? displayName, out MailAddress mailAddress)
        {
            mailAddress = null!;
            if (string.IsNullOrWhiteSpace(address))
            {
                return false;
            }

            try
            {
                mailAddress = string.IsNullOrWhiteSpace(displayName)
                    ? new MailAddress(address.Trim())
                    : new MailAddress(address.Trim(), displayName.Trim());
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

private static bool IsBrevoSmtpLoginAddress(string? email)
        {
            return !string.IsNullOrWhiteSpace(email)
                && email.Trim().EndsWith("@smtp-brevo.com", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> SendInvoiceEmailAsync(string toEmail, string musteriAdi, string siparisNo, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("Fatura dosyasi bulunamadi: {FilePath}", filePath);
                    return false;
                }

                var siteSettings = _siteSettingsService.GetSettings();
                var brandName = string.IsNullOrWhiteSpace(siteSettings.MarkaAdi) ? siteSettings.SiteAdi : siteSettings.MarkaAdi;
                var fromEmail = _config["EmailSettings:FromEmail"];
                if (string.IsNullOrWhiteSpace(fromEmail))
                {
                    fromEmail = siteSettings.Email;
                }
                var fromName = _config["EmailSettings:FromName"];
                if (string.IsNullOrWhiteSpace(fromName))
                {
                    fromName = brandName;
                }

                var host = _config["EmailSettings:Host"] ?? "smtp.gmail.com";
                var port = int.TryParse(_config["EmailSettings:Port"], out var parsedPort) ? parsedPort : 587;
                var enableSsl = bool.TryParse(_config["EmailSettings:EnableSSL"], out var parsedSsl) ? parsedSsl : true;
                var username = _config["EmailSettings:Username"] ?? string.Empty;
                var password = _config["EmailSettings:Password"] ?? string.Empty;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    _logger.LogWarning("SMTP ayarlari eksik - fatura maili gonderilemedi");
                    return false;
                }

                if (!TryCreateMailAddress(fromEmail, fromName, out var fromAddress) || !TryCreateMailAddress(toEmail, null, out var toAddress))
                {
                    _logger.LogWarning("Gecersiz e-posta adresi - fatura maili gonderilemedi");
                    return false;
                }

                var subject = $"Sipariş Faturanız Hazır - {siparisNo}";
                var body = $@"
                    <div style='font-family: system-ui, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #313511; margin-bottom: 20px;'>Merhaba {musteriAdi},</h2>
                        <p style='color: #47473d; line-height: 1.6;'>
                            Siparişiniz için fatura hazırlanmıştır. Aşağıdaki ekten fatura belgesini indirebilirsiniz.
                        </p>
                        <div style='background: #fcf9f3; border-radius: 12px; padding: 20px; margin: 20px 0; border: 1px solid #e5e2dc;'>
                            <p style='margin: 0;'><strong>Sipariş No:</strong> {siparisNo}</p>
                        </div>
                        <p style='color: #7a766a; font-size: 14px;'>
                            Herhangi bir sorunuz olursa bizimle iletişime geçebilirsiniz.
                        </p>
                        <p style='color: #313511; margin-top: 30px;'>
                            Saygılarımızla,<br/>
                            <strong>{brandName}</strong>
                        </p>
                    </div>";

                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = enableSsl,
                    Timeout = 30000
                };

                using var mailMessage = new MailMessage
                {
                    From = fromAddress,
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toAddress);

                var attachment = new Attachment(filePath, "application/pdf");
                if (attachment.ContentDisposition != null)
                {
                    attachment.ContentDisposition.Inline = false;
                    attachment.ContentDisposition.DispositionType = DispositionTypeNames.Attachment;
                }
                mailMessage.Attachments.Add(attachment);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Fatura maili basariyla gonderildi. SiparisNo={SiparisNo}, To={To}", siparisNo, toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatura maili gonderim hatasi. SiparisNo={SiparisNo}", siparisNo);
                return false;
            }
        }
    }
}
