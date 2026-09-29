using KanvasProje.Core.Models;
using KanvasProje.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace KanvasProje.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AnaSayfaController : AdminBaseController
    {
        private readonly IHomePageSettingsService _homePageSettingsService;
        private readonly IWebHostEnvironment _env;

        public AnaSayfaController(IHomePageSettingsService homePageSettingsService, IWebHostEnvironment env)
        {
            _homePageSettingsService = homePageSettingsService;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_homePageSettingsService.GetSettings());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(HomePageSettingsModel model)
        {
            try
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "slider");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                if (model.Hero?.DesktopSlides != null)
                {
                    for (int i = 0; i < model.Hero.DesktopSlides.Count; i++)
                    {
                        var file = Request.Form.Files[$"SlideImage_{i}"];
                        if (file != null && file.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString("N") + "_" + Path.GetFileName(file.FileName);
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            model.Hero.DesktopSlides[i].ImageUrl = $"/uploads/slider/{uniqueFileName}";
                        }

                        var videoFile = Request.Form.Files[$"SlideVideo_{i}"];
                        if (videoFile != null && videoFile.Length > 0)
                        {
                            string uniqueVideoName = Guid.NewGuid().ToString("N") + "_" + Path.GetFileName(videoFile.FileName);
                            string videoPath = Path.Combine(uploadsFolder, uniqueVideoName);
                            using (var fileStream = new FileStream(videoPath, FileMode.Create))
                            {
                                await videoFile.CopyToAsync(fileStream);
                            }

                            model.Hero.DesktopSlides[i].VideoUrl = $"/uploads/slider/{uniqueVideoName}";
                        }
                    }
                }

                if (model.Hero?.DesktopSlides != null && model.Hero.MobileSlides != null)
                {
                    for (var i = 0; i < model.Hero.DesktopSlides.Count && i < model.Hero.MobileSlides.Count; i++)
                    {
                        model.Hero.MobileSlides[i].Title = model.Hero.DesktopSlides[i].Title;
                        model.Hero.MobileSlides[i].Subtitle = model.Hero.DesktopSlides[i].Subtitle;
                        model.Hero.MobileSlides[i].Description = model.Hero.DesktopSlides[i].Description;
                        model.Hero.MobileSlides[i].ButtonText = model.Hero.DesktopSlides[i].ButtonText;
                        model.Hero.MobileSlides[i].ButtonUrl = model.Hero.DesktopSlides[i].ButtonUrl;
                        model.Hero.MobileSlides[i].VideoUrl = model.Hero.DesktopSlides[i].VideoUrl;
                    }
                }

                _homePageSettingsService.SaveSettings(model);
                TempData["Basari"] = "Ana sayfa slider ayarları kaydedildi.";
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Ana sayfa ayarları kaydedilirken hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
