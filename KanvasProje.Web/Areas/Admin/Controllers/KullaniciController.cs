using System.Text;
using KanvasProje.Core.Models;
using KanvasProje.Core.Varliklar;
using KanvasProje.Service.Services;
using KanvasProje.Web.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanvasProje.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KullaniciController : AdminBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAdminSessionStateService _adminSessionStateService;
        private readonly IAdminSecurityAuditService _auditService;

        public KullaniciController(
            UserManager<AppUser> userManager,
            IAdminSessionStateService adminSessionStateService,
            IAdminSecurityAuditService auditService)
        {
            _userManager = userManager;
            _adminSessionStateService = adminSessionStateService;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _userManager.Users.AsQueryable();
            var term = search?.Trim();

            if (!string.IsNullOrWhiteSpace(term))
            {
                var normalized = term.ToLower();
                query = query.Where(u =>
                    (u.AdSoyad ?? string.Empty).ToLower().Contains(normalized) ||
                    (u.Email ?? string.Empty).ToLower().Contains(normalized) ||
                    (u.PhoneNumber ?? string.Empty).Contains(normalized));
            }

            var users = await query
                .OrderByDescending(u => u.Id)
                .ToListAsync();

            var states = await _adminSessionStateService.GetStatesAsync(users.Select(x => x.Id));
            var currentUserId = _userManager.GetUserId(User);
            var items = new List<KullaniciListItemViewModel>(users.Count);

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.Count == 0
                    ? AdminSecurityRoles.Uye
                    : AdminSecurityRoles.GetPrimaryRole(roles);

                states.TryGetValue(user.Id, out var sessionState);

                items.Add(new KullaniciListItemViewModel
                {
                    Id = user.Id,
                    AdSoyad = string.IsNullOrWhiteSpace(user.AdSoyad) ? (user.Email ?? "Adsız kullanıcı") : user.AdSoyad,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Sehir = user.Sehir ?? string.Empty,
                    PrimaryRole = primaryRole,
                    RoleLabel = AdminSecurityRoles.GetRoleLabel(primaryRole),
                    IsAdmin = AdminSecurityRoles.IsAdminRole(primaryRole),
                    IsBanned = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow,
                    IsCurrentUser = string.Equals(user.Id, currentUserId, StringComparison.Ordinal),
                    LastAdminLoginUtc = sessionState?.CurrentLoginUtc,
                    PreviousAdminLoginUtc = sessionState?.PreviousLoginUtc
                });
            }

            var model = new KullaniciIndexViewModel
            {
                Search = term ?? string.Empty,
                TotalCount = items.Count,
                AdminCount = items.Count(x => x.IsAdmin),
                CustomerCount = items.Count(x => !x.IsAdmin),
                Kullanicilar = items
                    .OrderByDescending(x => x.IsAdmin)
                    .ThenBy(x => x.AdSoyad)
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DurumDegistir(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (string.Equals(user.Id, _userManager.GetUserId(User), StringComparison.Ordinal))
            {
                TempData["Hata"] = "Kendi hesabınızın durumunu bu ekrandan değiştiremezsiniz.";
                return RedirectToAction(nameof(Index));
            }

            var isBanned = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow;
            user.LockoutEnd = isBanned ? null : DateTimeOffset.UtcNow.AddYears(100);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["Hata"] = string.Join(" ", result.Errors.Select(x => x.Description));
                return RedirectToAction(nameof(Index));
            }

            await _adminSessionStateService.ClearSessionAsync(user.Id);

            await _auditService.LogAsync(
                HttpContext,
                isBanned ? "user_unblocked" : "user_blocked",
                isBanned ? "Kullanıcı engeli kaldırıldı." : "Kullanıcı engellendi.",
                target: user.Id);

            TempData["Basari"] = isBanned
                ? "Kullanıcı engeli kaldırıldı."
                : "Kullanıcı engellendi.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> SifreSifirla(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Hata"] = "Şifresi değiştirilecek kullanıcı seçilmedi.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Hata"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SifreSifirla(string id, string yeniSifre, string sifreTekrar)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Hata"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(yeniSifre))
            {
                TempData["Hata"] = "Yeni şifre boş bırakılamaz.";
                return View(user);
            }

            if (!string.Equals(yeniSifre, sifreTekrar, StringComparison.Ordinal))
            {
                TempData["Hata"] = "Yeni şifre ve tekrar alanı aynı olmalıdır.";
                return View(user);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, yeniSifre);

            if (!result.Succeeded)
            {
                TempData["Hata"] = string.Join(" ", result.Errors.Select(x => x.Description));
                return View(user);
            }

            await _adminSessionStateService.ClearSessionAsync(user.Id);

            await _auditService.LogAsync(
                HttpContext,
                "user_password_reset",
                "Kullanıcı şifresi admin tarafından sıfırlandı.",
                target: user.Id);

            TempData["Basari"] = "Şifre başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Duzenle(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Hata"] = "Düzenlenecek kullanıcı seçilmedi.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Hata"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditModelAsync(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(KullaniciDuzenleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var currentPrimaryRole = currentRoles.Count == 0
                ? AdminSecurityRoles.Uye
                : AdminSecurityRoles.GetPrimaryRole(currentRoles);
            var currentEditableRole = NormalizeEditableRole(currentPrimaryRole);

            var selectedRole = string.IsNullOrWhiteSpace(model.SelectedRole)
                ? AdminSecurityRoles.Uye
                : model.SelectedRole;
            var allowedRoles = AdminSecurityRoles.GetAssignableRoleOptions()
                .Select(x => x.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!allowedRoles.Contains(selectedRole))
            {
                ModelState.AddModelError(string.Empty, "Seçilen rol geçersiz.");
                var invalidRoleModel = await BuildEditModelAsync(user);
                invalidRoleModel.SelectedRole = currentEditableRole;
                return View(invalidRoleModel);
            }

            var currentUserId = _userManager.GetUserId(User);
            if (string.Equals(user.Id, currentUserId, StringComparison.Ordinal) &&
                !string.Equals(selectedRole, currentEditableRole, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Hata"] = "Kendi rolünüzü bu ekrandan değiştiremezsiniz.";
                return RedirectToAction(nameof(Duzenle), new { id = user.Id });
            }

            user.AdSoyad = model.AdSoyad?.Trim() ?? string.Empty;
            user.PhoneNumber = model.PhoneNumber?.Trim();
            user.Sehir = model.Sehir?.Trim() ?? string.Empty;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                AddErrors(updateResult);
                var failedModel = await BuildEditModelAsync(user);
                failedModel.SelectedRole = selectedRole;
                return View(failedModel);
            }

            var rolesToRemove = currentRoles
                .Where(x => AdminSecurityRoles.IsAdminRole(x) || string.Equals(x, AdminSecurityRoles.Uye, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (rolesToRemove.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    AddErrors(removeResult);
                    var failedModel = await BuildEditModelAsync(user);
                    failedModel.SelectedRole = currentEditableRole;
                    return View(failedModel);
                }
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, selectedRole);
            if (!addRoleResult.Succeeded)
            {
                AddErrors(addRoleResult);
                var failedModel = await BuildEditModelAsync(user);
                failedModel.SelectedRole = currentEditableRole;
                return View(failedModel);
            }

            var roleChanged = !string.Equals(currentEditableRole, selectedRole, StringComparison.OrdinalIgnoreCase);
            if (roleChanged)
            {
                await _adminSessionStateService.ClearSessionAsync(user.Id);

                await _auditService.LogAsync(
                    HttpContext,
                    "user_role_updated",
                    $"Kullanıcı rolü {AdminSecurityRoles.GetRoleLabel(currentEditableRole)} rolünden {AdminSecurityRoles.GetRoleLabel(selectedRole)} rolüne güncellendi.",
                    target: user.Id);
            }

            TempData["Basari"] = roleChanged
                ? "Kullanıcı bilgileri ve rolü güncellendi."
                : "Kullanıcı bilgileri güncellendi.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (string.Equals(user.Id, _userManager.GetUserId(User), StringComparison.Ordinal))
            {
                TempData["Hata"] = "Kendi hesabınızı silemezsiniz.";
                return RedirectToAction(nameof(Index));
            }

            await _adminSessionStateService.ClearSessionAsync(user.Id);
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                TempData["Hata"] = string.Join(" ", result.Errors.Select(x => x.Description));
                return RedirectToAction(nameof(Index));
            }

            await _auditService.LogAsync(
                HttpContext,
                "user_deleted",
                "Kullanıcı kaydı silindi.",
                target: user.Id);

            TempData["Basari"] = "Kullanıcı kaydı silindi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExcelExport()
        {
            var users = await _userManager.Users.ToListAsync();
            var states = await _adminSessionStateService.GetStatesAsync(users.Select(x => x.Id));
            var builder = new StringBuilder();

            builder.AppendLine("Id,AdSoyad,Email,Telefon,Sehir,Rol,SonAdminGirisi");

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.Count == 0
                    ? AdminSecurityRoles.Uye
                    : AdminSecurityRoles.GetPrimaryRole(roles);

                states.TryGetValue(user.Id, out var sessionState);
                var lastLogin = sessionState?.CurrentLoginUtc?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;

                builder.AppendLine(string.Join(",",
                    EscapeCsv(user.Id),
                    EscapeCsv(user.AdSoyad),
                    EscapeCsv(user.Email),
                    EscapeCsv(user.PhoneNumber),
                    EscapeCsv(user.Sehir),
                    EscapeCsv(AdminSecurityRoles.GetRoleLabel(primaryRole)),
                    EscapeCsv(lastLogin)));
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "kullanicilar.csv");
        }

        private async Task<KullaniciDuzenleViewModel> BuildEditModelAsync(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.Count == 0
                ? AdminSecurityRoles.Uye
                : AdminSecurityRoles.GetPrimaryRole(roles);
            var editableRole = NormalizeEditableRole(primaryRole);

            var roleOption = AdminSecurityRoles.GetRoleOption(editableRole);
            var sessionState = await _adminSessionStateService.GetStateAsync(user.Id);

            return new KullaniciDuzenleViewModel
            {
                Id = user.Id,
                AdSoyad = user.AdSoyad,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Sehir = user.Sehir ?? string.Empty,
                SelectedRole = editableRole,
                CurrentRoleLabel = roleOption.Label,
                CurrentRoleDescription = roleOption.Description,
                IsCurrentUser = string.Equals(user.Id, _userManager.GetUserId(User), StringComparison.Ordinal),
                IsBanned = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow,
                LastAdminLoginUtc = sessionState?.CurrentLoginUtc,
                PreviousAdminLoginUtc = sessionState?.PreviousLoginUtc,
                RoleOptions = AdminSecurityRoles.GetAssignableRoleOptions().ToList()
            };
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private static string NormalizeEditableRole(string roleName)
        {
            return string.Equals(roleName, AdminSecurityRoles.LegacyAdmin, StringComparison.OrdinalIgnoreCase)
                ? AdminSecurityRoles.SuperAdmin
                : roleName;
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
    }
}
