using Microsoft.AspNetCore.Mvc.Rendering;

namespace KanvasProje.Service.Helpers
{
    public static class UrunMedyaCatalog
    {
        public const string Gorsel = "Gorsel";
        public const string Video = "Video";

        public const string Galeri = "Galeri";
        public const string KisaTanitim = "KisaTanitim";
        public const string YakinDetay = "YakinDetay";
        public const string MockupSalon = "MockupSalon";
        public const string MockupYatakOdasi = "MockupYatakOdasi";
        public const string MockupCocukOdasi = "MockupCocukOdasi";
        public const string MockupOfis = "MockupOfis";
        public const string MockupKoridor = "MockupKoridor";
        public const string MockupMutfak = "MockupMutfak";
        public const string Uretim = "Uretim";
        public const string MusteriKullanim = "MusteriKullanim";
        public const string SizdenGelenler = "SizdenGelenler";

        private static readonly IReadOnlyList<(string Value, string Label)> MediaTypeOptions =
            new List<(string Value, string Label)>
            {
                (Gorsel, "Gorsel"),
                (Video, "Video")
            };

        private static readonly IReadOnlyList<(string Value, string Label)> MediaAreaOptions =
            new List<(string Value, string Label)>
            {
                (Galeri, "Genel galeri"),
                (KisaTanitim, "Kisa tanitim"),
                (YakinDetay, "Yakin cekim detay"),
                (MockupSalon, "Salon mockup"),
                (MockupYatakOdasi, "Yatak odasi mockup"),
                (MockupCocukOdasi, "Cocuk odasi mockup"),
                (MockupOfis, "Ofis mockup"),
                (MockupKoridor, "Koridor mockup"),
                (MockupMutfak, "Mutfak / yemek alani mockup"),
                (Uretim, "Uretim"),
                (MusteriKullanim, "Musteri kullanim"),
                (SizdenGelenler, "Sizden gelenler")
            };

        private static readonly HashSet<string> MockupAreas =
            new(StringComparer.OrdinalIgnoreCase)
            {
                MockupSalon,
                MockupYatakOdasi,
                MockupCocukOdasi,
                MockupOfis,
                MockupKoridor,
                MockupMutfak
            };

        public static List<SelectListItem> GetTypeSelectList(string? selectedValue = null)
        {
            return MediaTypeOptions
                .Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Label,
                    Selected = string.Equals(x.Value, selectedValue, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
        }

        public static List<SelectListItem> GetAreaSelectList(string? selectedValue = null)
        {
            return MediaAreaOptions
                .Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Label,
                    Selected = string.Equals(x.Value, selectedValue, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
        }

        public static string GetAreaLabel(string? value)
        {
            return MediaAreaOptions.FirstOrDefault(x => string.Equals(x.Value, value, StringComparison.OrdinalIgnoreCase)).Label
                ?? "Medya";
        }

        public static bool IsMockupArea(string? value)
        {
            return !string.IsNullOrWhiteSpace(value) && MockupAreas.Contains(value);
        }
    }
}
