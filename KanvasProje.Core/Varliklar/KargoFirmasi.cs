namespace KanvasProje.Core.Varliklar
{
    public class KargoFirmasi : BaseEntity
    {
        public string Ad { get; set; } = string.Empty;
        public string Kod { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Telefon { get; set; }
        public string? TakipUrl { get; set; }
        public string GondericiUnvan { get; set; } = "Canvasia";
        public string GondericiAdres { get; set; } = string.Empty;
        public string GondericiTelefon { get; set; } = string.Empty;
        public bool AktifMi { get; set; } = true;
        public bool VarsayilanMi { get; set; }
        public decimal Fiyat { get; set; } = 0;
    }
}
