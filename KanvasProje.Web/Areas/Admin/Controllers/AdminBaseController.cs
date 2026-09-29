using KanvasProje.Core.Varliklar;
using KanvasProje.Data;
using KanvasProje.Web.Security;
using KanvasProje.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace KanvasProje.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin,Yonetici,SiparisYoneticisi,UrunYoneticisi,IcerikYoneticisi,KargoYoneticisi,Goruntuleyici")]
    public class AdminBaseController : Controller
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!AdminSecurityRoles.IsAdminUser(User))
            {
                await next();
                return;
            }

            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var httpContext = context.HttpContext;
            var auditService = httpContext.RequestServices.GetRequiredService<IAdminSecurityAuditService>();

            if (!AdminPermissionMatrix.CanAccess(User, controllerName, httpContext.Request.Method))
            {
                await auditService.LogAsync(
                    httpContext,
                    "admin_permission_denied",
                    "Bu role izin verilmeyen bir admin islemi denendi.",
                    controllerName);

                context.Result = RedirectToAction("ErisimEngellendi", "Hesap", new { area = string.Empty });
                return;
            }

            var userManager = httpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
            var signInManager = httpContext.RequestServices.GetRequiredService<SignInManager<AppUser>>();
            var sessionStateService = httpContext.RequestServices.GetRequiredService<IAdminSessionStateService>();
            var dbContext = httpContext.RequestServices.GetService<KanvasDbContext>();

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                await signInManager.SignOutAsync();
                httpContext.Session.Remove(AdminSessionConstants.SessionKey);
                context.Result = RedirectToAction("GirisYap", "Hesap", new { area = string.Empty });
                return;
            }

            if (await userManager.IsLockedOutAsync(user))
            {
                await auditService.LogAsync(
                    httpContext,
                    "admin_locked_out",
                    "Kilitli admin hesabi panel erisimi denedi.",
                    controllerName,
                    user.Id,
                    user.UserName ?? user.Email);

                await sessionStateService.ClearSessionAsync(user.Id);
                httpContext.Session.Remove(AdminSessionConstants.SessionKey);
                await signInManager.SignOutAsync();
                context.Result = RedirectToAction("GirisYap", "Hesap", new { area = string.Empty });
                return;
            }

            var roles = await userManager.GetRolesAsync(user);
            var primaryRole = AdminSecurityRoles.GetPrimaryRole(roles);
            var primaryRoleLabel = AdminSecurityRoles.GetRoleLabel(primaryRole);
            var sessionToken = httpContext.Session.GetString(AdminSessionConstants.SessionKey);
            var sessionState = await sessionStateService.GetStateAsync(user.Id);

            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                sessionState = await sessionStateService.RegisterSessionAsync(
                    user,
                    primaryRoleLabel,
                    httpContext.Connection.RemoteIpAddress?.ToString());

                httpContext.Session.SetString(AdminSessionConstants.SessionKey, sessionState.CurrentSessionToken);

                await auditService.LogAsync(
                    httpContext,
                    "admin_session_started",
                    "Admin panel oturumu baslatildi.",
                    controllerName,
                    user.Id,
                    user.UserName ?? user.Email);
            }
            else if (!await sessionStateService.ValidateSessionAsync(user.Id, sessionToken))
            {
                await auditService.LogAsync(
                    httpContext,
                    "admin_session_invalidated",
                    "Admin oturumu baska bir giris nedeniyle gecersiz hale geldi.",
                    controllerName,
                    user.Id,
                    user.UserName ?? user.Email);

                httpContext.Session.Remove(AdminSessionConstants.SessionKey);
                await signInManager.SignOutAsync();

                var returnUrl = $"{httpContext.Request.Path}{httpContext.Request.QueryString}";
                context.Result = RedirectToAction("GirisYap", "Hesap", new { area = string.Empty, returnUrl });
                return;
            }

            ViewBag.AdminName = string.IsNullOrWhiteSpace(user.AdSoyad) ? (user.Email ?? user.UserName ?? "Admin") : user.AdSoyad;
            ViewBag.AdminRoleLabel = primaryRoleLabel;
            ViewBag.AdminLastLogin = sessionState?.PreviousLoginUtc;
            ViewBag.AdminCurrentLogin = sessionState?.CurrentLoginUtc;
            ViewBag.IsReadOnlyAdmin = AdminSecurityRoles.IsReadOnlyAdmin(User);
            ViewBag.CanViewDashboard = AdminPermissionMatrix.CanViewDashboard(User);
            ViewBag.CanManageOrders = AdminPermissionMatrix.CanManageOrders(User);
            ViewBag.CanManageProducts = AdminPermissionMatrix.CanManageProducts(User);
            ViewBag.CanManageContent = AdminPermissionMatrix.CanManageContent(User);
            ViewBag.CanManageUsers = AdminPermissionMatrix.CanManageUsers(User);
            ViewBag.CanViewReports = AdminPermissionMatrix.CanViewReports(User);
            ViewBag.CanManageSettings = AdminPermissionMatrix.CanManageSettings(User);

            if (dbContext != null)
            {
                ViewBag.BekleyenSiparis = await dbContext.Siparisler
                    .CountAsync(x => !x.SilindiMi && (int)x.Durum == 0);

                ViewBag.OkunmamisIletisim = await dbContext.IletisimMesajlari
                    .CountAsync(x => !x.OkunduMu);

                var son10Dakika = DateTime.UtcNow.AddMinutes(-10);
                ViewBag.OnlineZiyaretci = await dbContext.ZiyaretciLoglari
                    .Where(x => x.OlusturulmaTarihi >= son10Dakika)
                    .GroupBy(x => x.IpAdresi)
                    .CountAsync();
            }

            await next();
        }
    }
}
