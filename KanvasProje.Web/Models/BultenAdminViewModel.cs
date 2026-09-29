using System;

namespace KanvasProje.Web.Models
{
    public class BultenAdminViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime KayitTarihi { get; set; }
        public string IpAdresi { get; set; } = string.Empty;
        
        // ZiyaretciLog Tablosundan Gelecek Veriler
        public string Sehir { get; set; } = string.Empty;
        public string Ulke { get; set; } = string.Empty;
        public string Cihaz { get; set; } = string.Empty; // PC mi Mobil mi?
        public string Tarayici { get; set; } = string.Empty; // Chrome?
        public string IsletimSistemi { get; set; } = string.Empty; // Windows?
    }
}