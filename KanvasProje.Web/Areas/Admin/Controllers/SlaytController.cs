using System.IO;
using KanvasProje.Core.Varliklar;
using KanvasProje.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanvasProje.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlaytController : AdminBaseController
    {
        private readonly KanvasDbContext _db;
        private readonly IWebHostEnvironment _env;

        public SlaytController(KanvasDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var slaytlar = await _db.Slaytlar
                .OrderBy(s => s.Sira)
                .ThenBy(s => s.OlusturmaTarihi)
                .ToListAsync();

            return View(slaytlar);
        }

        public IActionResult Ekle()
        {
            return View(new Slayt { Sira = 1, AktifMi = true, Tur = "Resim" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(209_715_200)]
        [RequestFormLimits(MultipartBodyLengthLimit = 209_715_200)]
        public async Task<IActionResult> Ekle(Slayt model, IFormFile? Resim, IFormFile? Video)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Tur == "Video" && Video == null && string.IsNullOrWhiteSpace(model.VideoUrl))
            {
                ModelState.AddModelError("Video", "Video tipinde slayt için video yüklemeniz veya video URL'si girmeniz gerekir.");
                return View(model);
            }

            if (model.Tur == "Resim" && Resim == null && string.IsNullOrWhiteSpace(model.ResimUrl))
            {
                ModelState.AddModelError("Resim", "Resim tipinde slayt için görsel yüklemeniz veya görsel URL'si girmeniz gerekir.");
                return View(model);
            }

            if (Resim != null)
            {
                var resimPath = await SaveFileAsync(Resim, "uploads/slider");
                model.ResimUrl = resimPath;
            }

            if (Video != null)
            {
                var videoPath = await SaveFileAsync(Video, "uploads/slider");
                model.VideoUrl = videoPath;
            }

            var maxSira = await _db.Slaytlar.MaxAsync(s => (int?)s.Sira) ?? 0;
            model.Sira = maxSira + 1;
            model.OlusturmaTarihi = DateTime.UtcNow;

            _db.Slaytlar.Add(model);
            await _db.SaveChangesAsync();

            TempData["Basari"] = "Slayt başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Duzenle(int id)
        {
            var slayt = await _db.Slaytlar.FindAsync(id);
            if (slayt == null)
            {
                TempData["Hata"] = "Slayt bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            return View(slayt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(209_715_200)]
        [RequestFormLimits(MultipartBodyLengthLimit = 209_715_200)]
        public async Task<IActionResult> Duzenle(int id, Slayt model, IFormFile? Resim, IFormFile? Video, bool? ResimSil, bool? VideoSil)
        {
            var slayt = await _db.Slaytlar.FindAsync(id);
            if (slayt == null)
            {
                TempData["Hata"] = "Slayt bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
                return View(slayt);

            slayt.Baslik = model.Baslik;
            slayt.AltBaslik = model.AltBaslik;
            slayt.Aciklama = model.Aciklama;
            slayt.Tur = model.Tur;
            slayt.Sira = model.Sira;
            slayt.AktifMi = model.AktifMi;

            if (Resim != null)
            {
                if (!string.IsNullOrEmpty(slayt.ResimUrl))
                    DeleteFile(slayt.ResimUrl);
                slayt.ResimUrl = await SaveFileAsync(Resim, "uploads/slider");
            }

            if (Video != null)
            {
                if (!string.IsNullOrEmpty(slayt.VideoUrl))
                    DeleteFile(slayt.VideoUrl);
                slayt.VideoUrl = await SaveFileAsync(Video, "uploads/slider");
            }

            if (ResimSil == true && !string.IsNullOrEmpty(slayt.ResimUrl))
            {
                DeleteFile(slayt.ResimUrl);
                slayt.ResimUrl = null;
            }

            if (VideoSil == true && !string.IsNullOrEmpty(slayt.VideoUrl))
            {
                DeleteFile(slayt.VideoUrl);
                slayt.VideoUrl = null;
            }

            if (!string.IsNullOrWhiteSpace(model.ResimUrl) && model.ResimUrl != slayt.ResimUrl)
            {
                slayt.ResimUrl = model.ResimUrl;
            }

            if (!string.IsNullOrWhiteSpace(model.VideoUrl) && model.VideoUrl != slayt.VideoUrl)
            {
                slayt.VideoUrl = model.VideoUrl;
            }

            await _db.SaveChangesAsync();

            TempData["Basari"] = "Slayt başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AktiflikDegistir(int id)
        {
            var slayt = await _db.Slaytlar.FindAsync(id);
            if (slayt != null)
            {
                slayt.AktifMi = !slayt.AktifMi;
                await _db.SaveChangesAsync();
                TempData["Basari"] = slayt.AktifMi ? "Slayt aktif edildi." : "Slayt pasif edildi.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JSONdanAl()
        {
            var jsonPath = System.IO.Path.Combine(_env.ContentRootPath, "App_Data", "home-page-settings.json");
            if (!System.IO.File.Exists(jsonPath))
            {
                TempData["Hata"] = "JSON dosyası bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var json = await System.IO.File.ReadAllTextAsync(jsonPath);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var hero = doc.RootElement.GetProperty("Hero");
            var slides = hero.GetProperty("DesktopSlides");

            int sira = 1;
            foreach (var slide in slides.EnumerateArray())
            {
                var imageUrl = slide.GetProperty("ImageUrl").GetString() ?? "";
                var videoUrl = slide.GetProperty("VideoUrl").GetString() ?? "";
                var title = slide.GetProperty("Title").GetString() ?? "";
                var subtitle = slide.GetProperty("Subtitle").GetString() ?? "";
                var description = slide.GetProperty("Description").GetString() ?? "";

                var tur = !string.IsNullOrEmpty(videoUrl) ? "Video" : "Resim";

                var mevcut = await _db.Slaytlar
                    .FirstOrDefaultAsync(s => s.ResimUrl == imageUrl || s.VideoUrl == videoUrl);

                if (mevcut == null)
                {
                    var cleanTitle = title.Replace("\\n", " ").Replace("\n", "");
                    _db.Slaytlar.Add(new Slayt
                    {
                        Baslik = cleanTitle,
                        AltBaslik = subtitle ?? "",
                        Aciklama = description ?? "",
                        ResimUrl = imageUrl,
                        VideoUrl = videoUrl,
                        Tur = tur,
                        Sira = sira,
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.UtcNow
                    });
                    sira++;
                }
            }

            await _db.SaveChangesAsync();
            TempData["Basari"] = "JSON'dan " + (sira - 1) + " slayt içe aktarıldı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var slayt = await _db.Slaytlar.FindAsync(id);
            if (slayt == null)
            {
                return Json(new { success = false, message = "Slayt bulunamadı." });
            }

            var totalCount = await _db.Slaytlar.CountAsync();
            if (totalCount <= 1)
            {
                return Json(new { success = false, message = "En az 1 slayt kalmalıdır. Tüm slaytları silemezsiniz." });
            }

            if (!string.IsNullOrEmpty(slayt.ResimUrl))
                DeleteFile(slayt.ResimUrl);

            if (!string.IsNullOrEmpty(slayt.VideoUrl))
                DeleteFile(slayt.VideoUrl);

            _db.Slaytlar.Remove(slayt);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Slayt silindi." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Siralama(List<int> ids)
        {
            var slaytlar = await _db.Slaytlar.ToListAsync();
            for (int i = 0; i < ids.Count; i++)
            {
                var slayt = slaytlar.FirstOrDefault(s => s.Id == ids[i]);
                if (slayt != null)
                    slayt.Sira = i + 1;
            }
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        private async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, subFolder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString("N") + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/" + subFolder.Replace("\\", "/") + "/" + uniqueFileName;
        }

        private void DeleteFile(string? url)
        {
            if (string.IsNullOrEmpty(url)) return;
            var relativePath = url.TrimStart('/').Replace("/", "\\");
            var fullPath = System.IO.Path.Combine(_env.WebRootPath, relativePath);
            if (System.IO.File.Exists(fullPath))
            {
                try { System.IO.File.Delete(fullPath); } catch { }
            }
        }
    }
}