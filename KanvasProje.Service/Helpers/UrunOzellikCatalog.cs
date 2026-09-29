using KanvasProje.Core.Varliklar;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KanvasProje.Service.Helpers
{
    public sealed class VarsayilanUrunOzellikTanimi
    {
        public string Ad { get; init; } = string.Empty;
        public string Kod { get; init; } = string.Empty;
        public string UrunTipi { get; init; } = string.Empty;
        public string AlanTipi { get; init; } = "text";
        public string YardimMetni { get; init; } = string.Empty;
        public string Secenekler { get; init; } = string.Empty;
        public bool FiltredeGoster { get; init; }
        public bool DetaySayfasindaGoster { get; init; } = true;
        public bool TeknikTablodaGoster { get; init; } = true;
        public int Sira { get; init; }
    }

    public static class UrunOzellikCatalog
    {
        public const string Genel = "Genel";
        public const string Kanvas = "Kanvas";
        public const string Ayna = "Ayna";
        public const string Hali = "Hali";
        public const string CamTablo = "CamTablo";
        public const string DuvarKagidi = "DuvarKagidi";
        public const string SehpaMobilya = "SehpaMobilya";

        private static readonly IReadOnlyList<(string Value, string Label)> ProductTypes = new[]
        {
            (Genel, "Genel"),
            (Kanvas, "Kanvas"),
            (Ayna, "Ayna"),
            (Hali, "Hali"),
            (CamTablo, "Cam Tablo"),
            (DuvarKagidi, "Duvar Kagidi"),
            (SehpaMobilya, "Sehpa / Mobilya")
        };

        public static string NormalizeProductType(string? value)
        {
            var normalized = value?.Trim();
            return ProductTypes.Any(x => string.Equals(x.Value, normalized, StringComparison.OrdinalIgnoreCase))
                ? ProductTypes.First(x => string.Equals(x.Value, normalized, StringComparison.OrdinalIgnoreCase)).Value
                : Genel;
        }

        public static IReadOnlyList<SelectListItem> GetProductTypeSelectList(string? selectedValue = null)
        {
            var normalizedSelected = NormalizeProductType(selectedValue);
            return ProductTypes
                .Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Label,
                    Selected = string.Equals(x.Value, normalizedSelected, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
        }

        public static IReadOnlyList<VarsayilanUrunOzellikTanimi> GetDefaultDefinitions() => new[]
        {
            new VarsayilanUrunOzellikTanimi { Ad = "Renk", Kod = "genel_renk", UrunTipi = Genel, AlanTipi = "text", FiltredeGoster = true, Sira = 10 },
            new VarsayilanUrunOzellikTanimi { Ad = "Stil", Kod = "genel_stil", UrunTipi = Genel, AlanTipi = "select", Secenekler = "Modern\nMinimal\nKlasik\nRustik\nEndustriyel", FiltredeGoster = true, Sira = 20 },
            new VarsayilanUrunOzellikTanimi { Ad = "Kullanim Alani", Kod = "genel_kullanim_alani", UrunTipi = Genel, AlanTipi = "select", Secenekler = "Salon\nYatak Odasi\nOfis\nMutfak\nKoridor", FiltredeGoster = true, Sira = 30 },

            new VarsayilanUrunOzellikTanimi { Ad = "Baski Teknigi", Kod = "kanvas_baski_teknigi", UrunTipi = Kanvas, AlanTipi = "text", FiltredeGoster = true, Sira = 110 },
            new VarsayilanUrunOzellikTanimi { Ad = "Kanvas Kumasi", Kod = "kanvas_kumas", UrunTipi = Kanvas, AlanTipi = "text", FiltredeGoster = true, Sira = 120 },
            new VarsayilanUrunOzellikTanimi { Ad = "Sase Kalinligi", Kod = "kanvas_sase_kalinligi", UrunTipi = Kanvas, AlanTipi = "text", Sira = 130 },
            new VarsayilanUrunOzellikTanimi { Ad = "Asma Sekli", Kod = "kanvas_asma_sekli", UrunTipi = Kanvas, AlanTipi = "select", Secenekler = "Hazir Aparat\nDuvara Asilabilir Halka\nCercevesiz", Sira = 140 },

            new VarsayilanUrunOzellikTanimi { Ad = "Ayna Tipi", Kod = "ayna_tipi", UrunTipi = Ayna, AlanTipi = "select", Secenekler = "Flotal\nBronz\nFume\nDekoratif", FiltredeGoster = true, Sira = 210 },
            new VarsayilanUrunOzellikTanimi { Ad = "Cam Kalinligi", Kod = "ayna_cam_kalinligi", UrunTipi = Ayna, AlanTipi = "text", Sira = 220 },
            new VarsayilanUrunOzellikTanimi { Ad = "Kenar Isciligi", Kod = "ayna_kenar_isciligi", UrunTipi = Ayna, AlanTipi = "text", Sira = 230 },
            new VarsayilanUrunOzellikTanimi { Ad = "Montaj Yonu", Kod = "ayna_montaj_yonu", UrunTipi = Ayna, AlanTipi = "select", Secenekler = "Dikey\nYatay\nHer Iki Yonde", Sira = 240 },

            new VarsayilanUrunOzellikTanimi { Ad = "Dokuma Tipi", Kod = "hali_dokuma_tipi", UrunTipi = Hali, AlanTipi = "select", Secenekler = "Makine Dokuma\nEl Dokuma\nDijital Baski", FiltredeGoster = true, Sira = 310 },
            new VarsayilanUrunOzellikTanimi { Ad = "Taban Tipi", Kod = "hali_taban_tipi", UrunTipi = Hali, AlanTipi = "text", Sira = 320 },
            new VarsayilanUrunOzellikTanimi { Ad = "Hav Yuksekligi", Kod = "hali_hav_yuksekligi", UrunTipi = Hali, AlanTipi = "text", Sira = 330 },
            new VarsayilanUrunOzellikTanimi { Ad = "Yikanabilir Mi", Kod = "hali_yikanabilir", UrunTipi = Hali, AlanTipi = "select", Secenekler = "true\nfalse", FiltredeGoster = true, Sira = 340 },

            new VarsayilanUrunOzellikTanimi { Ad = "Cam Tipi", Kod = "camtablo_cam_tipi", UrunTipi = CamTablo, AlanTipi = "select", Secenekler = "Temperli\nParlak\nMat", FiltredeGoster = true, Sira = 410 },
            new VarsayilanUrunOzellikTanimi { Ad = "Yuzey Isciligi", Kod = "camtablo_yuzey", UrunTipi = CamTablo, AlanTipi = "text", Sira = 420 },
            new VarsayilanUrunOzellikTanimi { Ad = "Montaj Tipi", Kod = "camtablo_montaj", UrunTipi = CamTablo, AlanTipi = "text", Sira = 430 },
            new VarsayilanUrunOzellikTanimi { Ad = "Kenar Formu", Kod = "camtablo_kenar", UrunTipi = CamTablo, AlanTipi = "text", Sira = 440 },

            new VarsayilanUrunOzellikTanimi { Ad = "Rulo Olcusu", Kod = "duvarkagidi_rulo_olcusu", UrunTipi = DuvarKagidi, AlanTipi = "text", Sira = 510 },
            new VarsayilanUrunOzellikTanimi { Ad = "Uygulama Tipi", Kod = "duvarkagidi_uygulama", UrunTipi = DuvarKagidi, AlanTipi = "select", Secenekler = "Yapiskanli\nTutkalli\nKolay Sokülebilir", FiltredeGoster = true, Sira = 520 },
            new VarsayilanUrunOzellikTanimi { Ad = "Silinebilirlik", Kod = "duvarkagidi_silinebilirlik", UrunTipi = DuvarKagidi, AlanTipi = "select", Secenekler = "true\nfalse", FiltredeGoster = true, Sira = 530 },
            new VarsayilanUrunOzellikTanimi { Ad = "Desen Tekrari", Kod = "duvarkagidi_desen_tekrari", UrunTipi = DuvarKagidi, AlanTipi = "text", Sira = 540 },

            new VarsayilanUrunOzellikTanimi { Ad = "Govde Malzemesi", Kod = "mobilya_govde_malzeme", UrunTipi = SehpaMobilya, AlanTipi = "text", FiltredeGoster = true, Sira = 610 },
            new VarsayilanUrunOzellikTanimi { Ad = "Ayak Malzemesi", Kod = "mobilya_ayak_malzeme", UrunTipi = SehpaMobilya, AlanTipi = "text", FiltredeGoster = true, Sira = 620 },
            new VarsayilanUrunOzellikTanimi { Ad = "Tasima Kapasitesi", Kod = "mobilya_tasima_kapasitesi", UrunTipi = SehpaMobilya, AlanTipi = "text", Sira = 630 },
            new VarsayilanUrunOzellikTanimi { Ad = "Kurulum Durumu", Kod = "mobilya_kurulum", UrunTipi = SehpaMobilya, AlanTipi = "select", Secenekler = "Demonte\nHazir Kurulu", FiltredeGoster = true, Sira = 640 }
        };

        public static void ApplySeed(UrunOzellikTanimi target, VarsayilanUrunOzellikTanimi source)
        {
            target.Ad = source.Ad;
            target.Kod = source.Kod;
            target.UrunTipi = NormalizeProductType(source.UrunTipi);
            target.AlanTipi = source.AlanTipi;
            target.YardimMetni = source.YardimMetni;
            target.Secenekler = source.Secenekler;
            target.FiltredeGoster = source.FiltredeGoster;
            target.DetaySayfasindaGoster = source.DetaySayfasindaGoster;
            target.TeknikTablodaGoster = source.TeknikTablodaGoster;
            target.AktifMi = true;
            target.Sira = source.Sira;
        }
    }
}
