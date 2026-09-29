using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KanvasProje.Data;
using KanvasProje.Core.Varliklar;
using KanvasProje.Core.Helpers;

namespace KanvasProje.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class IadeController : AdminBaseController
    {
        private const int IadeHakkiGun = 14;

        private readonly KanvasDbContext _context;

        public IadeController(KanvasDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var talepler = await IadeTalebiQuery()
                .OrderByDescending(x => x.OlusturulmaTarihi)
                .ToListAsync();

            return View(talepler);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var talep = await IadeTalebiQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (talep == null)
            {
                return NotFound();
            }

            ViewBag.IadeHakkiGun = IadeHakkiGun;
            ViewBag.IadeSonTarih = talep.Siparis.OlusturulmaTarihi.ToUniversalTime().AddDays(IadeHakkiGun);
            ViewBag.IadeTalepZamanindaMi = IadeTalebiZamanindaMi(talep);

            return View(talep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DurumGuncelle(int id, int durum, string adminNotu)
        {
            var talep = await _context.IadeTalepleri
                .Include(x => x.Siparis)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (talep == null) return NotFound();

            if (durum is < 1 or > 3)
            {
                TempData["Hata"] = "Gecersiz iade durumu.";
                return RedirectToAction(nameof(Detay), new { id });
            }

            if (durum == 1 && !IadeTalebiZamanindaMi(talep))
            {
                TempData["Hata"] = $"{IadeHakkiGun} gunluk iade suresi disindaki talep onaylanamaz.";
                return RedirectToAction(nameof(Detay), new { id });
            }

            talep.Durum = durum; // 1: Onay, 2: Tamamlandı, 3: Red
            talep.AdminNotu = adminNotu?.Trim();

            var siparis = talep.Siparis;
            if (siparis != null)
            {
                if (durum == 1) siparis.Durum = SiparisDurumHelper.IadeOnaylandi;
                else if (durum == 2) siparis.Durum = SiparisDurumHelper.IadeTamamlandi;
                else if (durum == 3) siparis.Durum = SiparisDurumHelper.TeslimEdildi;
            }

            await _context.SaveChangesAsync();
            TempData["Mesaj"] = "Iade talebi guncellendi.";
            return RedirectToAction(nameof(Detay), new { id });
        }

        private IQueryable<IadeTalebi> IadeTalebiQuery()
        {
            return _context.IadeTalepleri
                .AsSplitQuery()
                .Include(x => x.Siparis)
                    .ThenInclude(x => x.SiparisDetaylari)
                        .ThenInclude(x => x.Urun)
                            .ThenInclude(x => x.UrunResimleri)
                .Include(x => x.Siparis)
                    .ThenInclude(x => x.SiparisDetaylari)
                        .ThenInclude(x => x.UrunSecenek)
                .Where(x => !x.SilindiMi);
        }

        private static bool IadeTalebiZamanindaMi(IadeTalebi talep)
        {
            var sonTarih = talep.Siparis.OlusturulmaTarihi.ToUniversalTime().AddDays(IadeHakkiGun);
            return talep.OlusturulmaTarihi.ToUniversalTime() <= sonTarih;
        }
    }
}
