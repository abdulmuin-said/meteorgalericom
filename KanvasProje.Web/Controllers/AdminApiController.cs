using KanvasProje.Data;
using KanvasProje.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanvasProje.Web.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Policy = AdminPolicyNames.AdminPanelAccess)]
    public class AdminApiController : ControllerBase
    {
        private readonly KanvasDbContext _context;

        public AdminApiController(KanvasDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var bugun = DateTime.UtcNow.Date;

            var bugunkuCiro = await _context.Siparisler
                .Where(x => x.OlusturulmaTarihi >= bugun && x.Durum >= 1 && !x.SilindiMi)
                .SumAsync(x => (decimal?)x.ToplamTutar) ?? 0;

            var bekleyenSiparis = await _context.Siparisler
                .CountAsync(x => x.Durum == 1 && !x.SilindiMi);

            var toplamUye = await _context.Users.CountAsync();
            var toplamUrun = await _context.Urunler.CountAsync(x => !x.SilindiMi);

            return Ok(new
            {
                GunlukCiro = bugunkuCiro,
                BekleyenSiparisSayisi = bekleyenSiparis,
                ToplamUye = toplamUye,
                ToplamUrun = toplamUrun,
                SonGuncelleme = DateTime.Now.ToString("HH:mm")
            });
        }
    }
}
