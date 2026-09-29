using KanvasProje.Core.Varliklar;

namespace KanvasProje.Core.Models
{
    public class ZiyaretciIndexViewModel
    {
        public IReadOnlyList<ZiyaretciLog> Kayitlar { get; set; } = Array.Empty<ZiyaretciLog>();
        public string Arama { get; set; } = string.Empty;
        public string Metod { get; set; } = string.Empty;
        public string Cihaz { get; set; } = string.Empty;
        public DateTime? Baslangic { get; set; }
        public DateTime? Bitis { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; } = 1;
        public int OnlineSayisi { get; set; }
        public int BugunTekil { get; set; }
        public int BugunZiyaret { get; set; }
        public int ToplamTekil { get; set; }
        public int MobilZiyaret { get; set; }
        public int MasaustuZiyaret { get; set; }
        public string EnCokGezilenSayfa { get; set; } = "-";
        public string EnGucluKaynak { get; set; } = "-";
        public IReadOnlyList<ZiyaretciStatItem> TopSayfalar { get; set; } = Array.Empty<ZiyaretciStatItem>();
        public IReadOnlyList<ZiyaretciStatItem> TopKaynaklar { get; set; } = Array.Empty<ZiyaretciStatItem>();
        public int[] PageSizeOptions { get; set; } = new[] { 20, 50, 100 };
    }

    public class ZiyaretciStatItem
    {
        public string Etiket { get; set; } = string.Empty;
        public int Adet { get; set; }
        public int Tekil { get; set; }
    }
}
