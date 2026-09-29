using Microsoft.AspNetCore.Mvc;
using KanvasProje.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace KanvasProje.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : AdminBaseController
    {
        private readonly KanvasDbContext _context;

        public SearchController(KanvasDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string q)
        {
            if (string.IsNullOrEmpty(q)) return RedirectToAction("Index", "Home");

            q = q.ToLower();

            // 1. Ürünlerde Ara
            var urunler = await _context.Urunler
                .Where(x => x.Baslik.ToLower().Contains(q) || x.Id.ToString() == q)
                .Select(x => new SearchResult { 
                    Baslik = x.Baslik, 
                    Tip = "Ürün", 
                    Url = $"/Admin/Urun/Duzenle/{x.Id}",
                    Detay = $"{x.Fiyat} TL" 
                })
                .ToListAsync();

            // 2. Müşterilerde Ara (Identity tablosu)
            // Not: UserManager kullanmak daha doğru olur ama hızlı çözüm için context üzerinden gidiyoruz
            var musteriler = await _context.Users
                .Where(x => (x.Email != null && x.Email.ToLower().Contains(q)) || 
                            (x.UserName != null && x.UserName.ToLower().Contains(q)) || 
                            (x.AdSoyad != null && x.AdSoyad.ToLower().Contains(q)))
                .Select(x => new SearchResult { 
                    Baslik = x.AdSoyad ?? x.Email ?? "İsimsiz", 
                    Tip = "Müşteri", 
                    Url = $"/Admin/Kullanici/Duzenle/{x.Id}",
                    Detay = x.Email ?? ""
                })
                .ToListAsync();

            // 3. Sonuçları Birleştir
            var sonuclar = new List<SearchResult>();
            sonuclar.AddRange(urunler);
            sonuclar.AddRange(musteriler);

            ViewBag.ArananKelime = q;
            return View(sonuclar);
        }
    }

    // Basit bir DTO sınıfı
    public class SearchResult
    {
        public string Baslik { get; set; } = string.Empty;
        public string Tip { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Detay { get; set; } = string.Empty;
    }
}
