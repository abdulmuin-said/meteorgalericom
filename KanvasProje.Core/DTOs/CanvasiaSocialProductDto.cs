namespace KanvasProje.Core.DTOs
{
    public class CanvasiaSocialProductDto
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string KisaAciklama { get; set; } = string.Empty;
        public string KategoriAdi { get; set; } = string.Empty;
        public decimal EtkinFiyat { get; set; }
        public bool IndirimVarMi { get; set; }
        public bool StoktaVarMi { get; set; }
        public string UrunUrl { get; set; } = string.Empty;
        public List<CanvasiaSocialProductImageDto> Resimler { get; set; } = new();
        public List<CanvasiaSocialProductOptionDto> Secenekler { get; set; } = new();
        public string SosyalMedyaPromptOzeti { get; set; } = string.Empty;
    }

    public class CanvasiaSocialProductImageDto
    {
        public string Url { get; set; } = string.Empty;
        public string Alt { get; set; } = string.Empty;
        public int Sira { get; set; }
        public bool AnaGorselMi { get; set; }
    }

    public class CanvasiaSocialProductOptionDto
    {
        public string Ad { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public bool StoktaVarMi { get; set; }
    }
}
