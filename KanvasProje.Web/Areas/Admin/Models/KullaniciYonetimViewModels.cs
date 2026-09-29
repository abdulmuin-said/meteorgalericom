using KanvasProje.Web.Security;

namespace KanvasProje.Core.Models
{
    public class KullaniciIndexViewModel
    {
        public string Search { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public int AdminCount { get; set; }
        public int CustomerCount { get; set; }
        public List<KullaniciListItemViewModel> Kullanicilar { get; set; } = new();
    }

    public class KullaniciListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Sehir { get; set; } = string.Empty;
        public string PrimaryRole { get; set; } = AdminSecurityRoles.Uye;
        public string RoleLabel { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; }
        public bool IsCurrentUser { get; set; }
        public DateTime? LastAdminLoginUtc { get; set; }
        public DateTime? PreviousAdminLoginUtc { get; set; }
    }

    public class KullaniciDuzenleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Sehir { get; set; } = string.Empty;
        public string SelectedRole { get; set; } = AdminSecurityRoles.Uye;
        public string CurrentRoleLabel { get; set; } = string.Empty;
        public string CurrentRoleDescription { get; set; } = string.Empty;
        public bool IsCurrentUser { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? LastAdminLoginUtc { get; set; }
        public DateTime? PreviousAdminLoginUtc { get; set; }
        public List<AdminRoleOption> RoleOptions { get; set; } = new();
    }
}
