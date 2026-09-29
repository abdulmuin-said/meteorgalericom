using System.Text.Json;
using KanvasProje.Core.Varliklar;
using KanvasProje.Core.Models;

namespace KanvasProje.Service.Services
{
    public interface ISeoService
    {
        SeoMetadata GenerateProductSeo(Urun product);
        SeoMetadata GenerateHomeSeo();
        SeoMetadata GenerateCategorySeo(Kategori category);
    }

    public class SeoService : ISeoService
    {
        private readonly ISiteSettingsService _siteSettingsService;

        public SeoService(ISiteSettingsService siteSettingsService)
        {
            _siteSettingsService = siteSettingsService;
        }

        public SeoMetadata GenerateProductSeo(Urun product)
        {
            var settings = _siteSettingsService.GetSettings();
            var brandName = GetBrandName(settings);
            var description = product.Aciklama?.Length > 150
                ? product.Aciklama[..150] + "..."
                : product.Aciklama ?? "Kaliteli urun";

            return new SeoMetadata
            {
                Title = $"{product.Baslik} | {brandName}",
                Description = $"{description} Hizli kargo, guvenli odeme, kaliteli hizmet.",
                Keywords = GenerateKeywords(product),
                OgImage = ResolveOgImage(settings, product.AnaGorselUrl),
                OgType = "product",
                CanonicalUrl = BuildProductUrl(product),
                StructuredData = GenerateProductSchema(product, settings)
            };
        }

        public SeoMetadata GenerateHomeSeo()
        {
            var settings = _siteSettingsService.GetSettings();
            var brandName = GetBrandName(settings);

            return new SeoMetadata
            {
                Title = string.IsNullOrWhiteSpace(settings.MetaTitle) ? settings.SiteBasligi : settings.MetaTitle,
                Description = string.IsNullOrWhiteSpace(settings.MetaDescription) ? settings.SiteAciklamasi : settings.MetaDescription,
                Keywords = string.IsNullOrWhiteSpace(settings.MetaKeywords)
                    ? $"{brandName}, e-ticaret, dekorasyon, premium urunler"
                    : settings.MetaKeywords,
                OgImage = ResolveOgImage(settings, settings.VarsayilanSosyalPaylasimGorseliUrl),
                OgType = "website",
                CanonicalUrl = _siteSettingsService.BuildAbsoluteUrl(string.Empty),
                StructuredData = GenerateWebsiteSchema(settings)
            };
        }

        public SeoMetadata GenerateCategorySeo(Kategori category)
        {
            var settings = _siteSettingsService.GetSettings();
            var brandName = GetBrandName(settings);

            return new SeoMetadata
            {
                Title = $"{category.Ad} | {brandName}",
                Description = $"{category.Ad} kategorisindeki urunleri kesfedin.",
                Keywords = $"{category.Ad}, e-ticaret, urunler",
                OgImage = ResolveOgImage(settings, settings.VarsayilanSosyalPaylasimGorseliUrl),
                OgType = "website",
                CanonicalUrl = _siteSettingsService.BuildAbsoluteUrl($"/Urun/Index?k={category.Id}"),
                StructuredData = null
            };
        }

        private string GenerateKeywords(Urun product)
        {
            return $"{product.Baslik}, e-ticaret, urun";
        }

        private string GenerateProductSchema(Urun product, SiteAyarlari settings)
        {
            var schema = new
            {
                context = "https://schema.org/",
                type = "Product",
                name = product.Baslik,
                image = ResolveOgImage(settings, product.AnaGorselUrl),
                description = product.Aciklama,
                brand = new { type = "Brand", name = GetBrandName(settings) },
                offers = new
                {
                    type = "Offer",
                    url = BuildProductUrl(product),
                    priceCurrency = "TRY",
                    price = product.Fiyat,
                    availability = "https://schema.org/InStock",
                    priceValidUntil = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd")
                }
            };

            return JsonSerializer.Serialize(schema);
        }

        private string GenerateWebsiteSchema(SiteAyarlari settings)
        {
            var siteUrl = _siteSettingsService.BuildAbsoluteUrl(string.Empty);
            var schema = new
            {
                context = "https://schema.org",
                type = "WebSite",
                name = GetBrandName(settings),
                url = siteUrl,
                potentialAction = new
                {
                    type = "SearchAction",
                    target = $"{siteUrl}/Urun/CanliAra?q={{search_term_string}}",
                    queryInput = "required name=search_term_string"
                }
            };

            return JsonSerializer.Serialize(schema);
        }

        private string BuildProductUrl(Urun product)
        {
            var detailSegment = string.IsNullOrWhiteSpace(product.Slug) ? product.Id.ToString() : product.Slug;
            return _siteSettingsService.BuildAbsoluteUrl($"/Urun/Detay/{detailSegment}");
        }

        private string ResolveOgImage(SiteAyarlari settings, string? imagePath)
        {
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                return _siteSettingsService.BuildAbsoluteUrl(imagePath);
            }

            if (!string.IsNullOrWhiteSpace(settings.VarsayilanSosyalPaylasimGorseliUrl))
            {
                return _siteSettingsService.BuildAbsoluteUrl(settings.VarsayilanSosyalPaylasimGorseliUrl);
            }

            return _siteSettingsService.BuildAbsoluteUrl(settings.SiteLogoUrl);
        }

        private static string GetBrandName(SiteAyarlari settings)
        {
            return string.IsNullOrWhiteSpace(settings.MarkaAdi) ? settings.SiteAdi : settings.MarkaAdi;
        }
    }

    public class SeoMetadata
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Keywords { get; set; }
        public string? OgImage { get; set; }
        public string? OgType { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? StructuredData { get; set; }
    }
}
