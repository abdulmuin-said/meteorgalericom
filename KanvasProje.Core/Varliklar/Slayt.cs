namespace KanvasProje.Core.Varliklar
{
    public class Slayt
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string AltBaslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string? ResimUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string Tur { get; set; } = "Resim";
        public int Sira { get; set; }
        public bool AktifMi { get; set; } = true;
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    }
}