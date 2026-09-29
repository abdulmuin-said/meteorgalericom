using System;

namespace KanvasProje.Core.Varliklar
{
    public class ZiyaretciLog : BaseEntity // BaseEntity'den ID ve Oluşturulma Tarihi geliyor
    {
        public string IpAdresi { get; set; } = string.Empty;      // Örn: 192.168.1.1
        public string Url { get; set; } = string.Empty;           // Örn: /Urun/Detay/5
        public string CihazBilgisi { get; set; } = string.Empty;  // Örn: Mozilla/5.0 (iPhone; CPU iPhone OS 14...)
        public string? ReferansUrl { get; set; }  // Örn: google.com (Nereden geldi?)
        public string? KullaniciAdi { get; set; } // Eğer giriş yapmışsa kim olduğu
        public string Metod { get; set; } = string.Empty;         // GET mi POST mu?
        public string? Sehir { get; set; }
        public string? Ulke { get; set; }
        public string? Tarayici { get; set; } // Chrome, Safari vb.
        public string? CihazModeli { get; set; } // Örn: iPhone, SM-G990E, PC
        public string? IsletimSistemi { get; set; } // Windows 10, Android vb.
    }
}