using Microsoft.AspNetCore.Mvc.Filters;
using KanvasProje.Data;
using KanvasProje.Core.Varliklar;

namespace KanvasProje.Web.Filters
{
    public class ZiyaretciTakipFilter : IAsyncActionFilter
    {
        private readonly KanvasDbContext _context;

        // Dependency Injection ile veritabanını alıyoruz
        public ZiyaretciTakipFilter(KanvasDbContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. İsteği Yakala
            var request = context.HttpContext.Request;
            var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            var url = $"{request.Path}{request.QueryString}";
            var userAgent = request.Headers["User-Agent"].ToString();
            var referer = request.Headers["Referer"].ToString(); // Nereden geldi?
            var method = request.Method; // GET/POST

            // Admin panelindeki hareketleri kaydetmeyelim (Veri kirliliği olmasın)
            if (!url.StartsWith("/Admin") && !url.Contains("/img/") && !url.Contains("/css/"))
            {
                // 2. Log Nesnesini Oluştur
                var log = new ZiyaretciLog
                {
                    IpAdresi = ip ?? "Bilinmiyor",
                    Url = url,
                    CihazBilgisi = userAgent.Length > 250 ? userAgent.Substring(0, 250) : userAgent, // Çok uzunsa kes
                    ReferansUrl = string.IsNullOrEmpty(referer) ? "Direkt Giriş" : referer,
                    Metod = method,
                    OlusturulmaTarihi = DateTime.UtcNow,
                    SilindiMi = false,
                    
                    // Eğer kullanıcı giriş yapmışsa ismini al, yoksa "Misafir" yaz
                    KullaniciAdi = (context.HttpContext.User.Identity?.IsAuthenticated == true) 
                                   ? context.HttpContext.User.Identity.Name 
                                   : "Misafir"
                };

                // 3. Veritabanına Kaydet
                _context.ZiyaretciLoglari.Add(log);
                await _context.SaveChangesAsync();
            }

            // 4. İşlemi Devam Ettir (Sayfa açılsın)
            await next();
        }
    }
}