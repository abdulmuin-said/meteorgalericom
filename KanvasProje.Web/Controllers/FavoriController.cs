using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KanvasProje.Core.Varliklar;
using KanvasProje.Data;
using KanvasProje.Service.Services;
using Microsoft.AspNetCore.Authorization;

namespace KanvasProje.Web.Controllers
{
    // DİKKAT: [Authorize] buradan kaldırıldı, aşağıya özel olarak eklendi.
    public class FavoriController : Controller
    {
        private readonly KanvasDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFavoriService _favoriService;

        public FavoriController(KanvasDbContext context, UserManager<AppUser> userManager, IFavoriService favoriService)
        {
            _context = context;
            _userManager = userManager;
            _favoriService = favoriService;
        }

        // 1. Favorilerim Sayfası (Burası Korumalı - Giriş yapmayan giremez)
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.Identity?.IsAuthenticated == true ? _userManager.GetUserId(User) : null;

            var favoriler = await _context.Favoriler
                .AsNoTracking()
                .Include(f => f.Urun)
                .Where(f => f.AppUserId == userId && !f.SilindiMi)
                .ToListAsync();

            return View(favoriler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int urunId)
        {
            // Giriş yapmamışsa hata döndür (Opsiyonel: JSON ile bildir)
            if (User.Identity?.IsAuthenticated != true)
            {
                return Json(new { success = false, message = "Lütfen önce giriş yapınız." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false, message = "Kullanıcı bulunamadı." });

            // Veritabanında var mı bak
            var favori = await _context.Favoriler
                .FirstOrDefaultAsync(x => x.UrunId == urunId && x.AppUserId == user.Id);

            if (favori != null)
            {
                // Varsa SİL (Favorilerden Çıkar)
                _context.Favoriler.Remove(favori);
                await _context.SaveChangesAsync();
                // JavaScript'e "Silindi" bilgisini ve durumu TRUE olarak dönüyoruz
                return Json(new { success = true, isAdded = false });
            }
            else
            {
                // Yoksa EKLE
                var yeniFavori = new Favori
                {
                    UrunId = urunId,
                    AppUserId = user.Id,
                    OlusturulmaTarihi = DateTime.UtcNow
                };
                _context.Favoriler.Add(yeniFavori);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isAdded = true });
            }
        }

        /// <summary>
        /// Fiyat düşüş bildirimini aç/kapat
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePriceNotification(int urunId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });

            try
            {
                var isEnabled = await _favoriService.TogglePriceNotificationAsync(user.Id, urunId);
                return Json(new
                {
                    success = true,
                    isEnabled,
                    message = isEnabled
                        ? "Bu ürünün fiyatı düştüğünde size e-posta ile bildireceğiz."
                        : "Fiyat düşüş bildirimi kapatıldı."
                });
            }
            catch
            {
                return Json(new { success = false, message = "Bu ürün favori listenizde bulunamadı." });
            }
        }
    }
}