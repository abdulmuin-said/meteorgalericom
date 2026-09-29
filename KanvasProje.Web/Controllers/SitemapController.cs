using Microsoft.AspNetCore.Mvc;
using KanvasProje.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using KanvasProje.Service.Services;
using System.Net;

namespace KanvasProje.Web.Controllers
{
    public class SitemapController : Controller
    {
        private readonly KanvasDbContext _context;
        private readonly ISiteSettingsService _siteSettingsService;

        public SitemapController(KanvasDbContext context, ISiteSettingsService siteSettingsService)
        {
            _context = context;
            _siteSettingsService = siteSettingsService;
        }

        [Route("sitemap.xml")]
        public async Task<IActionResult> Index()
        {
            var baseUrl = _siteSettingsService.BuildAbsoluteUrl(string.Empty).TrimEnd('/');
            var sitemap = new StringBuilder();
            sitemap.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sitemap.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Ana sayfa
            sitemap.AppendLine("  <url>");
            sitemap.AppendLine($"    <loc>{XmlEncode(baseUrl + "/")}</loc>");
            sitemap.AppendLine($"    <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>");
            sitemap.AppendLine("    <changefreq>daily</changefreq>");
            sitemap.AppendLine("    <priority>1.0</priority>");
            sitemap.AppendLine("  </url>");


            // Kategoriler
            var kategoriler = await _context.Kategoriler
                .AsNoTracking()
                .Where(k => !k.SilindiMi && k.AktifMi)
                .ToListAsync();

            foreach (var kategori in kategoriler)
            {
                sitemap.AppendLine("  <url>");
                sitemap.AppendLine($"    <loc>{XmlEncode($"{baseUrl}/Urun?k={kategori.Id}")}</loc>");
                sitemap.AppendLine($"    <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>");
                sitemap.AppendLine("    <changefreq>weekly</changefreq>");
                sitemap.AppendLine("    <priority>0.8</priority>");
                sitemap.AppendLine("  </url>");
            }

            // Ürünler (ilk 1000 ürün - performans için)
            var urunler = await _context.Urunler
                .AsNoTracking()
                .Where(u => !u.SilindiMi && u.AktifMi)
                .OrderByDescending(u => u.OlusturulmaTarihi)
                .Take(1000)
                .ToListAsync();

            foreach (var urun in urunler)
            {
                var detailSegment = string.IsNullOrWhiteSpace(urun.Slug) ? urun.Id.ToString() : $"{urun.Slug}-{urun.Id}";
                sitemap.AppendLine("  <url>");
                sitemap.AppendLine($"    <loc>{XmlEncode($"{baseUrl}/Urun/Detay/{detailSegment}")}</loc>");
                sitemap.AppendLine($"    <lastmod>{urun.OlusturulmaTarihi:yyyy-MM-dd}</lastmod>");
                sitemap.AppendLine("    <changefreq>monthly</changefreq>");
                sitemap.AppendLine("    <priority>0.7</priority>");
                sitemap.AppendLine("  </url>");
            }

            sitemap.AppendLine("</urlset>");

            return Content(sitemap.ToString(), "application/xml", Encoding.UTF8);
        }

        private static string XmlEncode(string value) => WebUtility.HtmlEncode(value);
    }
}
