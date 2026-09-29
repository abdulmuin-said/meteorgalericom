using System.Net;
using System.Security.Cryptography;
using System.Text;
using KanvasProje.Core.DTOs;
using KanvasProje.Core.Varliklar;
using KanvasProje.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanvasProje.Web.Controllers.Api
{
    [ApiController]
    [Route("api/canvasia-social/products")]
    public class CanvasiaSocialProductsController : ControllerBase
    {
        private const int DefaultPageSize = 24;
        private const int MaxPageSize = 100;
        private readonly KanvasDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public CanvasiaSocialProductsController(
            KanvasDbContext context,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _context = context;
            _configuration = configuration;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = DefaultPageSize,
            [FromQuery] string? category = null,
            [FromQuery] string? search = null,
            [FromQuery] bool onlyDiscounted = false,
            [FromQuery] bool onlyInStock = false)
        {
            if (!IsAuthorized())
            {
                return UnauthorizedResponse();
            }

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

            var query = ApplyFilters(BuildProductQuery(), category, search, onlyDiscounted, onlyInStock);
            var totalItems = await query.CountAsync();
            var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);

            var products = await query
                .OrderByDescending(x => x.YeniUrunMu)
                .ThenByDescending(x => x.OlusturulmaTarihi)
                .ThenBy(x => x.Sira)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                totalItems,
                totalPages,
                items = products.Select(MapProduct)
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            if (!IsAuthorized())
            {
                return UnauthorizedResponse();
            }

            var product = await BuildProductQuery().FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound(new
                {
                    error = "not_found",
                    message = "Urun bulunamadi."
                });
            }

            return Ok(MapProduct(product));
        }

        [HttpGet("sample")]
        public async Task<IActionResult> GetSample()
        {
            if (!IsAuthorized())
            {
                return UnauthorizedResponse();
            }

            var products = await BuildProductQuery()
                .OrderByDescending(x => x.OneCikanMi)
                .ThenByDescending(x => x.YeniUrunMu)
                .ThenByDescending(x => x.OlusturulmaTarihi)
                .ThenBy(x => x.Sira)
                .Take(5)
                .ToListAsync();

            return Ok(products.Select(MapProduct));
        }

        private IQueryable<Urun> BuildProductQuery()
        {
            return _context.Urunler
                .AsNoTracking()
                .Include(x => x.Kategori)
                .Include(x => x.UrunResimleri)
                .Include(x => x.UrunSecenek)
                .Where(x =>
                    x.AktifMi &&
                    !x.SilindiMi &&
                    x.Kategori != null &&
                    x.Kategori.AktifMi &&
                    !x.Kategori.SilindiMi);
        }

        private static IQueryable<Urun> ApplyFilters(
            IQueryable<Urun> query,
            string? category,
            string? search,
            bool onlyDiscounted,
            bool onlyInStock)
        {
            if (!string.IsNullOrWhiteSpace(category))
            {
                var categoryFilter = category.Trim().ToLowerInvariant();
                query = query.Where(x =>
                    x.Kategori != null &&
                    ((x.Kategori.Slug ?? string.Empty).ToLower().Contains(categoryFilter) ||
                     (x.Kategori.Ad ?? string.Empty).ToLower().Contains(categoryFilter)));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchFilter = search.Trim().ToLowerInvariant();
                query = query.Where(x =>
                    (x.Baslik ?? string.Empty).ToLower().Contains(searchFilter) ||
                    (x.KisaAd ?? string.Empty).ToLower().Contains(searchFilter) ||
                    (x.SKU ?? string.Empty).ToLower().Contains(searchFilter) ||
                    (x.Etiketler ?? string.Empty).ToLower().Contains(searchFilter) ||
                    (x.UrunTipi ?? string.Empty).ToLower().Contains(searchFilter));
            }

            if (onlyDiscounted)
            {
                query = query.Where(x =>
                    x.IndirimliFiyat.HasValue &&
                    x.IndirimliFiyat.Value > 0 &&
                    x.IndirimliFiyat.Value < x.Fiyat);
            }

            if (onlyInStock)
            {
                query = query.Where(x =>
                    x.StokDurumu.ToLower() == "stokta" ||
                    x.UrunSecenek.Any(s =>
                        !s.SilindiMi &&
                        s.AktifMi &&
                        (s.StokAdedi > 0 || s.OnSipariseAcikMi)));
            }

            return query;
        }

        private CanvasiaSocialProductDto MapProduct(Urun product)
        {
            var images = product.UrunResimleri
                .Where(x => !x.SilindiMi && !x.VideoMu && !string.IsNullOrWhiteSpace(x.ResimYolu))
                .OrderByDescending(x => x.VarsayilanMi)
                .ThenBy(x => x.Sira)
                .Select(x => new CanvasiaSocialProductImageDto
                {
                    Url = BuildAbsoluteUrl(x.ResimYolu),
                    Alt = !string.IsNullOrWhiteSpace(x.AltMetin) ? x.AltMetin : product.Baslik,
                    Sira = x.Sira,
                    AnaGorselMi = x.VarsayilanMi
                })
                .ToList();

            if (!string.IsNullOrWhiteSpace(product.AnaGorselUrl))
            {
                var mainImageUrl = BuildAbsoluteUrl(product.AnaGorselUrl);
                if (images.All(x => !string.Equals(x.Url, mainImageUrl, StringComparison.OrdinalIgnoreCase)))
                {
                    images.Insert(0, new CanvasiaSocialProductImageDto
                    {
                        Url = mainImageUrl,
                        Alt = product.Baslik,
                        Sira = 0,
                        AnaGorselMi = true
                    });
                }
            }

            var options = product.UrunSecenek
                .Where(x => !x.SilindiMi && x.AktifMi)
                .OrderByDescending(x => x.VarsayilanMi)
                .ThenBy(x => x.Sira)
                .Take(20)
                .Select(x => new CanvasiaSocialProductOptionDto
                {
                    Ad = string.IsNullOrWhiteSpace(x.VaryantBasligi) ? "Standart" : x.VaryantBasligi,
                    Fiyat = x.SatisFiyati > 0 ? x.SatisFiyati : product.EtkinFiyat + x.FiyatFarki,
                    StoktaVarMi = x.SatinAlinabilirMi
                })
                .ToList();

            return new CanvasiaSocialProductDto
            {
                Id = product.Id,
                Baslik = product.Baslik,
                Slug = product.Slug ?? string.Empty,
                Aciklama = product.Aciklama,
                KisaAciklama = product.KisaAciklama,
                KategoriAdi = product.Kategori?.Ad ?? string.Empty,
                EtkinFiyat = product.EtkinFiyat,
                IndirimVarMi = product.IndirimVarMi,
                StoktaVarMi = product.StoktaVarMi,
                UrunUrl = BuildProductUrl(product),
                Resimler = images,
                Secenekler = options,
                SosyalMedyaPromptOzeti = BuildPromptSummary(product)
            };
        }

        private string BuildProductUrl(Urun product)
        {
            var detailId = !string.IsNullOrWhiteSpace(product.Slug)
                ? $"{product.Slug}-{product.Id}"
                : product.Id.ToString();

            return BuildAbsoluteUrl($"/Urun/Detay/{detailId}");
        }

        private string BuildAbsoluteUrl(string pathOrUrl)
        {
            if (string.IsNullOrWhiteSpace(pathOrUrl))
            {
                return string.Empty;
            }

            if (Uri.TryCreate(pathOrUrl, UriKind.Absolute, out var absoluteUri))
            {
                if (IsHttpUrl(absoluteUri))
                {
                    return absoluteUri.ToString();
                }

                pathOrUrl = !string.IsNullOrWhiteSpace(absoluteUri.PathAndQuery)
                    ? absoluteUri.PathAndQuery
                    : absoluteUri.AbsolutePath;
            }

            var normalizedPath = pathOrUrl.StartsWith("/")
                ? pathOrUrl
                : $"/{pathOrUrl}";

            var configuredAppUrl = _configuration["AppUrl"]?.TrimEnd('/');
            if (!string.IsNullOrWhiteSpace(configuredAppUrl) &&
                Uri.TryCreate(configuredAppUrl, UriKind.Absolute, out var appUri) &&
                IsHttpUrl(appUri))
            {
                return new Uri(appUri, normalizedPath).ToString();
            }

            var requestScheme = string.Equals(Request.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
                ? Uri.UriSchemeHttps
                : Uri.UriSchemeHttp;

            return $"{requestScheme}://{Request.Host}{normalizedPath}";
        }

        private static bool IsHttpUrl(Uri uri)
        {
            return string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildPromptSummary(Urun product)
        {
            var categoryName = product.Kategori?.Ad ?? "Genel";
            var productType = string.IsNullOrWhiteSpace(product.UrunTipi) ? "dekoratif urun" : product.UrunTipi;
            var discountText = product.IndirimVarMi
                ? $"Indirimli fiyat: {product.EtkinFiyat:N2} TL. Liste fiyati: {product.Fiyat:N2} TL."
                : $"Fiyat: {product.EtkinFiyat:N2} TL.";
            var stockText = product.StoktaVarMi ? "Stokta var." : "Stok bilgisi sinirli veya stokta yok.";

            return string.Join(" ", new[]
            {
                $"Canvasia urun adi: {product.Baslik}.",
                $"Kategori: {categoryName}.",
                $"Urun tipi: {productType}.",
                discountText,
                stockText,
                !string.IsNullOrWhiteSpace(product.KisaAciklama) ? $"Kisa aciklama: {ToPlainText(product.KisaAciklama)}" : string.Empty,
                "Turkce, samimi, satis odakli ve dekorasyon ilhami veren sosyal medya aciklamasi ile hashtag uret."
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static string ToPlainText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var withoutTags = System.Text.RegularExpressions.Regex.Replace(value, "<.*?>", " ");
            var decoded = WebUtility.HtmlDecode(withoutTags);
            return string.Join(" ", decoded.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
        private IActionResult UnauthorizedResponse()
        {
            return Unauthorized(new
            {
                error = "unauthorized",
                message = "Canvasia Social API anahtari gecersiz veya eksik."
            });
        }

        private bool IsAuthorized()
        {
            var configuredKey = _configuration["CanvasiaSocial:ApiKey"];
            if (string.IsNullOrWhiteSpace(configuredKey))
            {
                return _environment.IsDevelopment() && IsLocalRequest();
            }

            if (!Request.Headers.TryGetValue("X-Canvasia-Social-Key", out var headerValue))
            {
                return false;
            }

            var providedKey = headerValue.ToString();
            if (string.IsNullOrWhiteSpace(providedKey))
            {
                return false;
            }

            var configuredBytes = Encoding.UTF8.GetBytes(configuredKey);
            var providedBytes = Encoding.UTF8.GetBytes(providedKey);

            return configuredBytes.Length == providedBytes.Length &&
                CryptographicOperations.FixedTimeEquals(configuredBytes, providedBytes);
        }

        private bool IsLocalRequest()
        {
            var remoteIp = HttpContext.Connection.RemoteIpAddress;
            return remoteIp == null || IPAddress.IsLoopback(remoteIp);
        }
    }
}

