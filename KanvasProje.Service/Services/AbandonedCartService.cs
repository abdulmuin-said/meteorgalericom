using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using KanvasProje.Data;
using KanvasProje.Core.Interfaces;
using KanvasProje.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KanvasProje.Service.Services
{
public class AbandonedCartService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AbandonedCartService> _logger;
        private readonly IConfiguration _config;

        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(60);
        private readonly TimeSpan _abandonmentThreshold = TimeSpan.FromHours(1);

        public AbandonedCartService(
            IServiceScopeFactory scopeFactory,
            ILogger<AbandonedCartService> logger,
            IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Abandoned Cart Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Abandoned Cart Service checking for carts...");

                if (!IsSmtpConfigured())
                {
                    _logger.LogWarning("SMTP ayarlari yapilandirilmadigi icin terk edilmis sepet e-posta kontrolu atlandi.");
                    try
                    {
                        await Task.Delay(_checkInterval, stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }
                    continue;
                }

try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<KanvasDbContext>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                        var siteSettingsService = scope.ServiceProvider.GetRequiredService<ISiteSettingsService>();

                        var thresholdTime = DateTime.UtcNow.Subtract(_abandonmentThreshold);
                        
                        // Terk edilmiş sepetleri bul:
                        // 1. Son güncelleme threshold'dan eski
                        // 2. Henüz hatırlatma gönderilmemiş
                        // 3. Üye sepeti (AppUserId != null) - Anonimlere mail atamayız
                        // 4. Sepet boş değil
                        var abandonedCarts = await context.Sepetler
                            .Include(x => x.AppUser)
                            .Include(x => x.SepetItems.Where(i => !i.SilindiMi))
                            .Where(x => x.AppUserId != null 
                                     && x.SepetItems.Any(i => !i.SilindiMi) 
                                     && x.SonGuncellemeTarihi < thresholdTime 
                                     && !x.HatirlatmaGonderildi)
                            .ToListAsync(stoppingToken);

                        if (abandonedCarts.Any())
                        {
                            _logger.LogInformation($"Found {abandonedCarts.Count} abandoned carts.");

                            foreach (var sepet in abandonedCarts)
                            {
                                if (sepet.AppUser == null || string.IsNullOrWhiteSpace(sepet.AppUser.Email)) continue;

                                if (!IsValidEmail(sepet.AppUser.Email))
                                {
                                    sepet.HatirlatmaGonderildi = true;
                                    sepet.TerkEdildi = true;
                                    sepet.TerkEdilmeTarihi = DateTime.UtcNow;
                                    _logger.LogWarning("Invalid user email skipped for abandoned cart. CartId={CartId}, Email={Email}", sepet.Id, sepet.AppUser.Email);
                                    continue;
                                }

                                try
                                {
                                    var activeItems = sepet.SepetItems.Where(x => !x.SilindiMi).ToList();
                                    var itemCount = activeItems.Sum(x => x.Adet);
                                    var productList = string.Join("", activeItems.Take(4).Select(x => $"<li>{System.Net.WebUtility.HtmlEncode(x.UrunBaslik)} - {x.Adet} adet</li>"));
                                    string subject = "Sepetinizdeki ürünler sizi bekliyor";
                                    string body = $@"
                                        <p>Sepetinize eklediğiniz ürünleri sizin için ayırdık.</p>
                                        <p>Sepetinizde <strong>{itemCount}</strong> adet ürün bekliyor. Siparişinizi tamamlayarak seçtiğiniz tabloları güvenle satın alabilirsiniz.</p>
                                        {(string.IsNullOrWhiteSpace(productList) ? string.Empty : $"<ul>{productList}</ul>")}
                                        <p>Stok ve kampanya koşulları değişmeden sepetinizi tamamlamak için aşağıdaki butonu kullanabilirsiniz.</p>";

                                    string btnLink = siteSettingsService.BuildAbsoluteUrl("/Sepet");
                                    
                                    await emailService.SendTemplateMailAsync(
                                        sepet.AppUser.Email, 
                                        subject, 
                                        sepet.AppUser.AdSoyad, 
                                        body, 
                                        btnLink, 
                                        "Sepete Git"
                                    );

                                    // Güncelle
                                    sepet.HatirlatmaGonderildi = true;
                                    sepet.TerkEdildi = true;
                                    sepet.TerkEdilmeTarihi = DateTime.UtcNow;
                                    
                                    _logger.LogInformation($"Reminder email sent to {sepet.AppUser.Email}");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Failed to send reminder email to cart {sepet.Id}");
                                }
                            }

                            await context.SaveChangesAsync(stoppingToken); // Toplu kayıt
                        }
                        else
                        {
                            _logger.LogInformation("No abandoned carts found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Abandoned Cart Service");
                }

                // Bir sonraki kontrol için bekle
                try
                {
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private bool IsSmtpConfigured()
        {
            var username = _config["EmailSettings:Username"];
            var password = _config["EmailSettings:Password"];
            return IsValidEmail(username) && !string.IsNullOrWhiteSpace(password);
        }

        private static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                _ = new MailAddress(email.Trim());
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
