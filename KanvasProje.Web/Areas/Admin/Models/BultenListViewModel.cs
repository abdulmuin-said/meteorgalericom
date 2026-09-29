using System;

namespace KanvasProje.Core.Models
{
    public class BultenListViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime KayitTarihi { get; set; }
        public bool AktifMi { get; set; }
        public string IpAdresi { get; set; } = string.Empty;
        
        // ZiyaretciLog Tablosundan Gelecek Veriler
        public string Sehir { get; set; } = string.Empty;
        public string Ulke { get; set; } = string.Empty;
        public string Cihaz { get; set; } = string.Empty;
        public string Tarayici { get; set; } = string.Empty;
        public string IsletimSistemi { get; set; } = string.Empty;
    }
}
