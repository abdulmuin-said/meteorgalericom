namespace KanvasProje.Core.Models
{
    public class RaporIndexViewModel
    {
        public DateTime Baslangic { get; set; }
        public DateTime Bitis { get; set; }
        public string AralikEtiketi { get; set; } = string.Empty;

        public decimal Ciro { get; set; }
        public decimal OncekiCiro { get; set; }
        public decimal IndirimToplami { get; set; }
        public decimal OrtalamaSepet { get; set; }
        public int SiparisSayisi { get; set; }
        public int OncekiSiparisSayisi { get; set; }
        public int SatilanUrunAdedi { get; set; }
        public int TekilMusteriSayisi { get; set; }
        public int ZiyaretSayisi { get; set; }
        public int TekilZiyaretciSayisi { get; set; }
        public decimal DonusumOrani { get; set; }
        public int BekleyenSiparisSayisi { get; set; }
        public int KargoyaHazirSiparisSayisi { get; set; }
        public int IadeTalebiSayisi { get; set; }
        public int TerkEdilenSepetSayisi { get; set; }
        public decimal TerkEdilenSepetTutari { get; set; }
        public int YeniMusteriSayisi { get; set; }
        public int TekrarMusteriSayisi { get; set; }
        public decimal TekrarMusteriCirosu { get; set; }
        public int IptalSiparisSayisi { get; set; }

        public IReadOnlyList<RaporDailyMetric> GunlukMetrikler { get; set; } = Array.Empty<RaporDailyMetric>();
        public IReadOnlyList<RaporHourlyMetric> SaatlikMetrikler { get; set; } = Array.Empty<RaporHourlyMetric>();
        public IReadOnlyList<RaporStatusMetric> DurumDagilimi { get; set; } = Array.Empty<RaporStatusMetric>();
        public IReadOnlyList<RaporProductMetric> EnCokSatanUrunler { get; set; } = Array.Empty<RaporProductMetric>();
        public IReadOnlyList<RaporProductMetric> EnCokTiklananUrunler { get; set; } = Array.Empty<RaporProductMetric>();
        public IReadOnlyList<RaporProductConversionMetric> UrunDonusumSorunlari { get; set; } = Array.Empty<RaporProductConversionMetric>();
        public IReadOnlyList<RaporCategoryMetric> KategoriPerformansi { get; set; } = Array.Empty<RaporCategoryMetric>();
        public IReadOnlyList<RaporCityMetric> SehirPerformansi { get; set; } = Array.Empty<RaporCityMetric>();
        public IReadOnlyList<RaporCustomerMetric> EnDegerliMusteriler { get; set; } = Array.Empty<RaporCustomerMetric>();
        public IReadOnlyList<RaporCouponMetric> KuponPerformansi { get; set; } = Array.Empty<RaporCouponMetric>();
        public IReadOnlyList<RaporTrafficMetric> TrafikKaynaklari { get; set; } = Array.Empty<RaporTrafficMetric>();
        public IReadOnlyList<RaporTrafficMetric> EnCokGezilenSayfalar { get; set; } = Array.Empty<RaporTrafficMetric>();
        public IReadOnlyList<RaporTrafficMetric> CihazDagilimi { get; set; } = Array.Empty<RaporTrafficMetric>();
        public IReadOnlyList<RaporKargoMetric> KargoPerformansi { get; set; } = Array.Empty<RaporKargoMetric>();
        public IReadOnlyList<RaporReturnReasonMetric> IadeIptalNedenleri { get; set; } = Array.Empty<RaporReturnReasonMetric>();
        public IReadOnlyList<RaporInsightItem> Oneriler { get; set; } = Array.Empty<RaporInsightItem>();
    }

    public class RaporDailyMetric
    {
        public DateTime Tarih { get; set; }
        public decimal Ciro { get; set; }
        public int Siparis { get; set; }
        public int Ziyaret { get; set; }
    }

    public class RaporHourlyMetric
    {
        public int Saat { get; set; }
        public int Siparis { get; set; }
        public decimal Ciro { get; set; }
        public int Ziyaret { get; set; }
    }

    public class RaporStatusMetric
    {
        public int Durum { get; set; }
        public string Etiket { get; set; } = string.Empty;
        public int Adet { get; set; }
        public decimal Tutar { get; set; }
    }

    public class RaporProductMetric
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string GorselUrl { get; set; } = string.Empty;
        public int Adet { get; set; }
        public decimal Ciro { get; set; }
        public int Goruntulenme { get; set; }
        public int Favori { get; set; }
    }

    public class RaporProductConversionMetric
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string GorselUrl { get; set; } = string.Empty;
        public int Goruntulenme { get; set; }
        public int SatisAdedi { get; set; }
        public decimal Ciro { get; set; }
        public decimal DonusumOrani { get; set; }
        public string RiskNotu { get; set; } = string.Empty;
    }

    public class RaporCategoryMetric
    {
        public int KategoriId { get; set; }
        public string KategoriAdi { get; set; } = string.Empty;
        public int UrunAdedi { get; set; }
        public int SiparisAdedi { get; set; }
        public decimal Ciro { get; set; }
    }

    public class RaporCityMetric
    {
        public string Sehir { get; set; } = string.Empty;
        public int SiparisAdedi { get; set; }
        public decimal Ciro { get; set; }
    }

    public class RaporCustomerMetric
    {
        public string Musteri { get; set; } = string.Empty;
        public string Eposta { get; set; } = string.Empty;
        public string Sehir { get; set; } = string.Empty;
        public int SiparisAdedi { get; set; }
        public decimal Ciro { get; set; }
        public DateTime SonSiparisTarihi { get; set; }
        public bool YeniMusteri { get; set; }
    }

    public class RaporCouponMetric
    {
        public string Kod { get; set; } = string.Empty;
        public int Kullanim { get; set; }
        public decimal Indirim { get; set; }
        public decimal Ciro { get; set; }
    }

    public class RaporTrafficMetric
    {
        public string Etiket { get; set; } = string.Empty;
        public int Adet { get; set; }
        public int Tekil { get; set; }
    }

    public class RaporKargoMetric
    {
        public string Firma { get; set; } = string.Empty;
        public int SiparisAdedi { get; set; }
        public int Kargoda { get; set; }
        public int Teslim { get; set; }
    }

    public class RaporReturnReasonMetric
    {
        public string Neden { get; set; } = string.Empty;
        public string Tip { get; set; } = string.Empty;
        public int Adet { get; set; }
        public decimal Tutar { get; set; }
    }

    public class RaporInsightItem
    {
        public string Seviye { get; set; } = "info";
        public string Baslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }
}
