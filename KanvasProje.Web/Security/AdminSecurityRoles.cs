using System.Security.Claims;

namespace KanvasProje.Web.Security
{
    public static class AdminSecurityRoles
    {
        public const string LegacyAdmin = "Admin";
        public const string SuperAdmin = "SuperAdmin";
        public const string Yonetici = "Yonetici";
        public const string SiparisYoneticisi = "SiparisYoneticisi";
        public const string UrunYoneticisi = "UrunYoneticisi";
        public const string IcerikYoneticisi = "IcerikYoneticisi";
        public const string KargoYoneticisi = "KargoYoneticisi";
        public const string Goruntuleyici = "Goruntuleyici";
        public const string Uye = "Uye";

        public static readonly string[] AllAdminRoles =
        {
            LegacyAdmin,
            SuperAdmin,
            Yonetici,
            SiparisYoneticisi,
            UrunYoneticisi,
            IcerikYoneticisi,
            KargoYoneticisi,
            Goruntuleyici
        };

        public static readonly string[] AssignableRoles =
        {
            SuperAdmin,
            Yonetici,
            SiparisYoneticisi,
            UrunYoneticisi,
            IcerikYoneticisi,
            KargoYoneticisi,
            Goruntuleyici,
            Uye
        };

        private static readonly Dictionary<string, AdminRoleOption> RoleMetadata = new(StringComparer.OrdinalIgnoreCase)
        {
            [LegacyAdmin] = new AdminRoleOption(LegacyAdmin, "Admin", "Eski yönetici rolü. Tüm yetkilere sahiptir.", 0),
            [SuperAdmin] = new AdminRoleOption(SuperAdmin, "Admin", "Tüm ayarlar, kullanıcılar ve yönetim ekranları üzerinde tam yetki.", 1),
            [Yonetici] = new AdminRoleOption(Yonetici, "Yönetici", "Günlük operasyonu ve çoğu admin işlemini yönetebilir.", 2),
            [SiparisYoneticisi] = new AdminRoleOption(SiparisYoneticisi, "Sipariş Yöneticisi", "Sipariş, iade ve operasyon akışlarını yönetebilir.", 3),
            [UrunYoneticisi] = new AdminRoleOption(UrunYoneticisi, "Ürün Yöneticisi", "Ürün, kategori ve ilgili katalog alanlarını yönetebilir.", 4),
            [IcerikYoneticisi] = new AdminRoleOption(IcerikYoneticisi, "İçerik Yöneticisi", "Sayfa, yorum, bülten ve vitrin içeriklerini yönetebilir.", 5),
            [KargoYoneticisi] = new AdminRoleOption(KargoYoneticisi, "Kargo Yöneticisi", "Kargo ve iade süreçlerine odaklı operasyon rolüdür.", 6),
            [Goruntuleyici] = new AdminRoleOption(Goruntuleyici, "Sadece Görüntüleyici", "Admin panelini görür ancak değişiklik yapamaz.", 7),
            [Uye] = new AdminRoleOption(Uye, "Üye", "Sadece mağaza kullanıcısıdır.", 8)
        };

        public static bool IsAdminRole(string? roleName)
        {
            return !string.IsNullOrWhiteSpace(roleName) &&
                AllAdminRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsAdminUser(ClaimsPrincipal user)
        {
            return AllAdminRoles.Any(user.IsInRole);
        }

        public static bool UserHasAnyRole(ClaimsPrincipal user, IEnumerable<string> roles)
        {
            return roles.Any(user.IsInRole);
        }

        public static bool IsReadOnlyAdmin(ClaimsPrincipal user)
        {
            return user.IsInRole(Goruntuleyici) &&
                !user.IsInRole(LegacyAdmin) &&
                !user.IsInRole(SuperAdmin) &&
                !user.IsInRole(Yonetici);
        }

        public static IReadOnlyList<AdminRoleOption> GetAssignableRoleOptions()
        {
            return AssignableRoles
                .Select(GetRoleOption)
                .OrderBy(x => x.SortOrder)
                .ToList();
        }

        public static AdminRoleOption GetRoleOption(string roleName)
        {
            if (RoleMetadata.TryGetValue(roleName, out var option))
            {
                return option;
            }

            return new AdminRoleOption(roleName, roleName, roleName, int.MaxValue);
        }

        public static string GetRoleLabel(string roleName)
        {
            return GetRoleOption(roleName).Label;
        }

        public static string GetPrimaryRole(IEnumerable<string> roles)
        {
            var roleList = roles.ToList();

            foreach (var knownRole in new[]
            {
                SuperAdmin,
                LegacyAdmin,
                Yonetici,
                SiparisYoneticisi,
                UrunYoneticisi,
                IcerikYoneticisi,
                KargoYoneticisi,
                Goruntuleyici,
                Uye
            })
            {
                if (roleList.Contains(knownRole, StringComparer.OrdinalIgnoreCase))
                {
                    return knownRole;
                }
            }

            return Uye;
        }

        public static string GetPrimaryRoleLabel(IEnumerable<string> roles)
        {
            return GetRoleLabel(GetPrimaryRole(roles));
        }
    }

    public static class AdminPolicyNames
    {
        public const string AdminPanelAccess = "AdminPanelAccess";
    }

    public static class AdminSessionConstants
    {
        public const string SessionKey = "AdminSessionToken";
    }

    public sealed class AdminRoleOption
    {
        public AdminRoleOption(string value, string label, string description, int sortOrder)
        {
            Value = value;
            Label = label;
            Description = description;
            SortOrder = sortOrder;
        }

        public string Value { get; }
        public string Label { get; }
        public string Description { get; }
        public int SortOrder { get; }
    }
}
