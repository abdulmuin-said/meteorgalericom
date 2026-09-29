using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KanvasProje.Data;
using KanvasProje.Core.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace KanvasProje.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlugToolController : AdminBaseController
    {
        private readonly KanvasDbContext _context;

        public SlugToolController(KanvasDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mevcut tüm ürünler için slug generate eder (one-time migration tool)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GenerateAllSlugs()
        {
            var urunler = await _context.Urunler
                .Where(u => u.Slug == null || u.Slug == "")
                .ToListAsync();

            if (!urunler.Any())
            {
                return Content("Tüm ürünlerde slug mevcut! İşlem gerekmiyor.");
            }

            var existingSlugs = await _context.Urunler
                .Where(u => u.Slug != null && u.Slug != "")
                .Select(u => u.Slug!)
                .ToListAsync();

            int updatedCount = 0;

            foreach (var urun in urunler)
            {
                try
                {
                    var slug = SlugHelper.GenerateSlug(urun.Baslik);
                    slug = SlugHelper.EnsureUnique(slug, existingSlugs);
                    
                    urun.Slug = slug;
                    existingSlugs.Add(slug); // Listeye ekle ki diğerleri conflict etmesin
                    
                    updatedCount++;
                }
                catch (Exception ex)
                {
                    // Hata logla ama devam et
                    Console.WriteLine($"SLUG ERROR for Urun ID {urun.Id}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();

            return Content($"✅ Başarılı! {updatedCount} adet ürün için slug oluşturuldu.\n\n" +
                          $"Artık ürün linkleri SEO-friendly olacak:\n" +
                          $"Örnek: /Urun/Detay/ataturk-portresi-kanvas-1355");
        }
    }
}
