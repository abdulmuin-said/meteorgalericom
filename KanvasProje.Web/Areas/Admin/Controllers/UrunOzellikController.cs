using KanvasProje.Core.Varliklar;
using KanvasProje.Data;
using KanvasProje.Service.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanvasProje.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UrunOzellikController : AdminBaseController
    {
        private readonly KanvasDbContext _context;

        public UrunOzellikController(KanvasDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? urunTipi)
        {
            var selectedType = UrunOzellikCatalog.NormalizeProductType(urunTipi);
            var query = _context.UrunOzellikTanimlari.AsQueryable();

            if (!string.IsNullOrWhiteSpace(urunTipi))
            {
                query = query.Where(x => x.UrunTipi == selectedType);
            }

            ViewBag.UrunTipleri = UrunOzellikCatalog.GetProductTypeSelectList(urunTipi);
            ViewBag.CurrentUrunTipi = string.IsNullOrWhiteSpace(urunTipi) ? string.Empty : selectedType;

            var items = await query
                .OrderBy(x => x.UrunTipi)
                .ThenBy(x => x.Sira)
                .ThenBy(x => x.Ad)
                .ToListAsync();

            return View(items);
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            PopulateProductTypes(UrunOzellikCatalog.Genel);
            return View(new UrunOzellikTanimi
            {
                UrunTipi = UrunOzellikCatalog.Genel,
                AlanTipi = "text",
                AktifMi = true,
                DetaySayfasindaGoster = true,
                TeknikTablodaGoster = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(UrunOzellikTanimi model)
        {
            NormalizeDefinition(model);
            await ValidateDefinitionAsync(model, null);

            if (!ModelState.IsValid)
            {
                PopulateProductTypes(model.UrunTipi);
                return View(model);
            }

            model.OlusturulmaTarihi = DateTime.UtcNow;
            model.SilindiMi = false;

            _context.UrunOzellikTanimlari.Add(model);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Özellik alanı eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Duzenle(int id)
        {
            var model = await _context.UrunOzellikTanimlari.FirstOrDefaultAsync(x => x.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            PopulateProductTypes(model.UrunTipi);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(int id, UrunOzellikTanimi model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var existing = await _context.UrunOzellikTanimlari.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null)
            {
                return NotFound();
            }

            NormalizeDefinition(model);
            await ValidateDefinitionAsync(model, id);

            if (!ModelState.IsValid)
            {
                PopulateProductTypes(model.UrunTipi);
                return View(model);
            }

            existing.Ad = model.Ad;
            existing.Kod = model.Kod;
            existing.UrunTipi = model.UrunTipi;
            existing.AlanTipi = model.AlanTipi;
            existing.YardimMetni = model.YardimMetni;
            existing.Secenekler = model.Secenekler;
            existing.FiltredeGoster = model.FiltredeGoster;
            existing.DetaySayfasindaGoster = model.DetaySayfasindaGoster;
            existing.TeknikTablodaGoster = model.TeknikTablodaGoster;
            existing.AktifMi = model.AktifMi;
            existing.Sira = model.Sira;

            await _context.SaveChangesAsync();
            TempData["Mesaj"] = "Özellik alanı güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var model = await _context.UrunOzellikTanimlari.FirstOrDefaultAsync(x => x.Id == id);
            if (model != null)
            {
                model.SilindiMi = true;
                model.AktifMi = false;
                await _context.SaveChangesAsync();
            }

            TempData["Mesaj"] = "Özellik alanı arşive alındı.";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateProductTypes(string? selectedProductType)
        {
            ViewBag.UrunTipleri = UrunOzellikCatalog.GetProductTypeSelectList(selectedProductType);
        }

        private static void NormalizeDefinition(UrunOzellikTanimi model)
        {
            model.Ad = model.Ad?.Trim() ?? string.Empty;
            model.Kod = SlugHelper.GenerateSlug(string.IsNullOrWhiteSpace(model.Kod) ? model.Ad : model.Kod).Replace("-", "_");
            model.UrunTipi = UrunOzellikCatalog.NormalizeProductType(model.UrunTipi);
            model.AlanTipi = string.IsNullOrWhiteSpace(model.AlanTipi) ? "text" : model.AlanTipi.Trim().ToLowerInvariant();
            model.YardimMetni = model.YardimMetni?.Trim() ?? string.Empty;
            model.Secenekler = model.Secenekler?.Trim() ?? string.Empty;
            model.Sira = model.Sira < 0 ? 0 : model.Sira;
        }

        private async Task ValidateDefinitionAsync(UrunOzellikTanimi model, int? currentId)
        {
            if (string.IsNullOrWhiteSpace(model.Ad))
            {
                ModelState.AddModelError(nameof(UrunOzellikTanimi.Ad), "Alan başlığı zorunludur.");
            }

            if (string.IsNullOrWhiteSpace(model.Kod))
            {
                ModelState.AddModelError(nameof(UrunOzellikTanimi.Kod), "Kod zorunludur.");
            }

            var validFieldTypes = new[] { "text", "textarea", "number", "select" };
            if (!validFieldTypes.Contains(model.AlanTipi))
            {
                ModelState.AddModelError(nameof(UrunOzellikTanimi.AlanTipi), "Geçerli bir alan tipi seçin.");
            }

            if (model.AlanTipi == "select" && string.IsNullOrWhiteSpace(model.Secenekler))
            {
                ModelState.AddModelError(nameof(UrunOzellikTanimi.Secenekler), "Seçimli liste alanları için seçenek listesi girin.");
            }

            var duplicateExists = await _context.UrunOzellikTanimlari
                .IgnoreQueryFilters()
                .AnyAsync(x =>
                    x.Id != currentId &&
                    x.Kod == model.Kod &&
                    x.UrunTipi == model.UrunTipi);

            if (duplicateExists)
            {
                ModelState.AddModelError(nameof(UrunOzellikTanimi.Kod), "Bu ürün tipi için aynı kod zaten tanımlı.");
            }
        }
    }
}
