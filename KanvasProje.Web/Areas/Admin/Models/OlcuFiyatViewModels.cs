namespace KanvasProje.Web.Areas.Admin.Models
{
    public class OlcuFiyatModel
    {
        public string Olcu { get; set; } = string.Empty;
        public string CerceveTipi { get; set; } = string.Empty;
        public int UrunSayisi { get; set; }
        public int VaryasyonSayisi { get; set; }
        public decimal MevcutFiyat { get; set; }
    }

    public class OlcuFiyatGuncelleModel
    {
        public string Olcu { get; set; } = string.Empty;
        public string CerceveTipi { get; set; } = string.Empty;
        public decimal? YeniFiyat { get; set; }
    }
}
