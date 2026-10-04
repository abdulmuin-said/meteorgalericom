using System.Globalization;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using KanvasProje.Core.Models;
using KanvasProje.Service.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace KanvasProje.Service.Services
{
    /// <summary>
    /// İyzico (iyzico Checkout Form) ödeme altyapısı implementasyonu.
    /// IPaymentService arayüzünü uygular.
    /// PCI-DSS uyumlu, responsive iframe ve hosted form destekler.
    /// </summary>
    public class IyzicoPaymentService : IPaymentService
    {
        private const string ProtectorPurpose = "MeteorGaleri.Iyzico.Settings.v1";

        private readonly ISiteSettingsService _siteSettingsService;
        private readonly IDataProtector _iyzicoProtector;
        private readonly ILogger<IyzicoPaymentService> _logger;

        public IyzicoPaymentService(
            ISiteSettingsService siteSettingsService,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<IyzicoPaymentService> logger)
        {
            _siteSettingsService = siteSettingsService;
            _iyzicoProtector = dataProtectionProvider.CreateProtector(ProtectorPurpose);
            _logger = logger;
        }

        /// <summary>
        /// İyzico Checkout Form başlatır ve token / form içeriğini alır.
        /// </summary>
        public async Task<PaymentInitResult> InitializeCheckoutAsync(PaymentInitRequest request)
        {
            try
            {
                var settings = _siteSettingsService.GetSettings();

                if (!settings.IyzicoAktifMi)
                {
                    return new PaymentInitResult
                    {
                        Success = false,
                        ErrorMessage = "İyzico ödeme yöntemi şu an aktif değil."
                    };
                }

                var (apiKey, secretKey, baseUrl) = GetCredentials(settings);
                if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(secretKey))
                {
                    _logger.LogWarning("İyzico API anahtarları eksik. OrderId={OrderId}", request.OrderId);
                    return new PaymentInitResult
                    {
                        Success = false,
                        ErrorMessage = "İyzico mağaza anahtarları eksik. Lütfen admin panelinden ayarları kontrol edin."
                    };
                }

                var options = new Options
                {
                    ApiKey = apiKey,
                    SecretKey = secretKey,
                    BaseUrl = baseUrl
                };

                var (buyerName, buyerSurname) = SplitName(request.BuyerName);

                var req = new CreateCheckoutFormInitializeRequest
                {
                    Locale = Locale.TR.ToString(),
                    ConversationId = request.OrderId,
                    Price = request.BasketPrice.ToString("0.00", CultureInfo.InvariantCulture),
                    PaidPrice = request.TotalPrice.ToString("0.00", CultureInfo.InvariantCulture),
                    Currency = Currency.TRY.ToString(),
                    BasketId = request.OrderId,
                    PaymentGroup = PaymentGroup.PRODUCT.ToString(),
                    CallbackUrl = !string.IsNullOrWhiteSpace(settings.IyzicoCallbackUrl)
                        ? settings.IyzicoCallbackUrl
                        : request.CallbackUrl,
                    EnabledInstallments = new List<int> { 1, 2, 3, 6, 9, 12 },
                    Buyer = new Buyer
                    {
                        Id = string.IsNullOrWhiteSpace(request.BuyerEmail) ? "GUEST" : request.BuyerEmail,
                        Name = buyerName,
                        Surname = buyerSurname,
                        GsmNumber = NormalizePhone(request.BuyerPhone),
                        Email = request.BuyerEmail,
                        IdentityNumber = "11111111111", // Standart B2C kimlik numarası
                        RegistrationAddress = Truncate(request.BuyerAddress, 250),
                        Ip = string.IsNullOrWhiteSpace(request.BuyerIp) ? "127.0.0.1" : request.BuyerIp,
                        City = string.IsNullOrWhiteSpace(request.BuyerCity) ? "Istanbul" : request.BuyerCity,
                        Country = "Turkey"
                    },
                    ShippingAddress = new Address
                    {
                        ContactName = Truncate(request.BuyerName, 50),
                        City = string.IsNullOrWhiteSpace(request.BuyerCity) ? "Istanbul" : request.BuyerCity,
                        Country = "Turkey",
                        Description = Truncate(request.BuyerAddress, 250)
                    },
                    BillingAddress = new Address
                    {
                        ContactName = Truncate(request.BuyerName, 50),
                        City = string.IsNullOrWhiteSpace(request.BuyerCity) ? "Istanbul" : request.BuyerCity,
                        Country = "Turkey",
                        Description = Truncate(request.BuyerAddress, 250)
                    }
                };

                var basketItems = new List<BasketItem>();
                int itemIndex = 1;
                foreach (var item in request.BasketItems)
                {
                    basketItems.Add(new BasketItem
                    {
                        Id = $"ITEM_{itemIndex++}",
                        Name = Truncate(string.IsNullOrWhiteSpace(item.Name) ? "MeteorGaleri Tablo" : item.Name, 50),
                        Category1 = "Tablo",
                        ItemType = BasketItemType.PHYSICAL.ToString(),
                        Price = (item.Price * item.Quantity).ToString("0.00", CultureInfo.InvariantCulture)
                    });
                }

                if (!basketItems.Any())
                {
                    basketItems.Add(new BasketItem
                    {
                        Id = "DEFAULT_1",
                        Name = "MeteorGaleri Tablo",
                        Category1 = "Tablo",
                        ItemType = BasketItemType.PHYSICAL.ToString(),
                        Price = request.BasketPrice.ToString("0.00", CultureInfo.InvariantCulture)
                    });
                }

                req.BasketItems = basketItems;

                _logger.LogInformation("İyzico checkout form başlatılıyor. OrderId={OrderId}, Tutar={Tutar}", request.OrderId, request.TotalPrice);

                var formInit = await CheckoutFormInitialize.Create(req, options);

                if (formInit.Status == "success" && !string.IsNullOrWhiteSpace(formInit.Token))
                {
                    _logger.LogInformation("İyzico checkout token başarıyla alındı. OrderId={OrderId}, Token={Token}", request.OrderId, formInit.Token);

                    return new PaymentInitResult
                    {
                        Success = true,
                        Token = formInit.Token,
                        CheckoutFormContent = formInit.CheckoutFormContent,
                        PaymentPageUrl = formInit.PaymentPageUrl,
                        IframeUrl = formInit.PaymentPageUrl
                    };
                }

                var errMsg = formInit.ErrorMessage ?? formInit.ErrorGroup ?? "İyzico formu başlatılamadı.";
                _logger.LogWarning("İyzico form başlatma hatası. OrderId={OrderId}, Hata={Error}", request.OrderId, errMsg);

                return new PaymentInitResult
                {
                    Success = false,
                    ErrorMessage = $"İyzico: {errMsg}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İyzico checkout başlatılırken beklenmeyen hata. OrderId={OrderId}", request.OrderId);
                return new PaymentInitResult
                {
                    Success = false,
                    ErrorMessage = "İyzico ödeme servisine bağlanırken bir hata oluştu."
                };
            }
        }

        /// <summary>
        /// İyzico callback sonrasında token doğrulaması ve ödeme sonucunu sorgular.
        /// </summary>
        public async Task<PaymentVerifyResult> VerifyPaymentAsync(PaymentVerifyRequest request)
        {
            try
            {
                var token = !string.IsNullOrWhiteSpace(request.Token) ? request.Token : request.MerchantOid;

                if (string.IsNullOrWhiteSpace(token))
                {
                    return new PaymentVerifyResult
                    {
                        SignatureValid = false,
                        PaymentSuccessful = false,
                        ErrorMessage = "Doğrulama token'ı bulunamadı."
                    };
                }

                var settings = _siteSettingsService.GetSettings();
                var (apiKey, secretKey, baseUrl) = GetCredentials(settings);

                var options = new Options
                {
                    ApiKey = apiKey,
                    SecretKey = secretKey,
                    BaseUrl = baseUrl
                };

                var req = new RetrieveCheckoutFormRequest
                {
                    Locale = Locale.TR.ToString(),
                    Token = token
                };

                var checkoutForm = await CheckoutForm.Retrieve(req, options);

                _logger.LogInformation(
                    "İyzico ödeme sorgulandı. Token={Token}, Status={Status}, PaymentStatus={PaymentStatus}, BasketId={BasketId}",
                    token, checkoutForm?.Status, checkoutForm?.PaymentStatus, checkoutForm?.BasketId);

                if (checkoutForm != null && checkoutForm.Status == "success")
                {
                    bool isPaid = checkoutForm.PaymentStatus == "SUCCESS";
                    decimal.TryParse(checkoutForm.PaidPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var paidAmount);

                    return new PaymentVerifyResult
                    {
                        SignatureValid = true,
                        PaymentSuccessful = isPaid,
                        PaidPrice = paidAmount,
                        TransactionId = checkoutForm.PaymentId,
                        OrderId = checkoutForm.BasketId ?? checkoutForm.ConversationId,
                        PaymentType = checkoutForm.CardFamily ?? checkoutForm.CardType ?? "Kredi Kartı",
                        Currency = checkoutForm.Currency ?? "TRY",
                        ErrorMessage = isPaid ? null : (checkoutForm.ErrorMessage ?? "Ödeme tamamlanamadı.")
                    };
                }

                var errorMsg = checkoutForm?.ErrorMessage ?? "İyzico ödeme doğrulanamadı.";
                return new PaymentVerifyResult
                {
                    SignatureValid = false,
                    PaymentSuccessful = false,
                    ErrorMessage = errorMsg
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İyzico ödeme doğrulanırken hata. Token={Token}", request.Token);
                return new PaymentVerifyResult
                {
                    SignatureValid = false,
                    PaymentSuccessful = false,
                    ErrorMessage = "Ödeme doğrulama işlemi sırasında hata oluştu."
                };
            }
        }

        private (string apiKey, string secretKey, string baseUrl) GetCredentials(SiteAyarlari settings)
        {
            var apiKey = settings.IyzicoApiKey;
            var secretKey = string.Empty;

            try
            {
                if (!string.IsNullOrWhiteSpace(settings.IyzicoSecretKeyProtected))
                {
                    secretKey = _iyzicoProtector.Unprotect(settings.IyzicoSecretKeyProtected);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İyzico secret key çözülürken hata.");
            }

            var baseUrl = !string.IsNullOrWhiteSpace(settings.IyzicoBaseUrl)
                ? settings.IyzicoBaseUrl
                : (settings.IyzicoTestModu ? "https://sandbox-api.iyzipay.com" : "https://api.iyzipay.com");

            return (apiKey, secretKey, baseUrl);
        }

        private static (string name, string surname) SplitName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return ("MeteorGaleri", "Müşteri");

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return (parts[0], "Müşteri");

            var name = string.Join(' ', parts.Take(parts.Length - 1));
            var surname = parts.Last();
            return (name, surname);
        }

        private static string NormalizePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "+905000000000";

            var digits = new string(phone.Where(char.IsDigit).ToArray());
            if (digits.StartsWith("90") && digits.Length >= 12)
                return "+" + digits;
            if (digits.StartsWith("0") && digits.Length >= 11)
                return "+9" + digits;
            if (digits.Length == 10)
                return "+90" + digits;

            return "+905000000000";
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Length <= maxLength ? value : value[..maxLength];
        }
    }
}
