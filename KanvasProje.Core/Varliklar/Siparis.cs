using System;
using System.Collections.Generic;

namespace KanvasProje.Core.Varliklar
{
    public class Siparis : BaseEntity
    {
        // --- YENİ EKLENEN KISIM ---
        // Hangi üyeye ait? (Identity tablosundaki Id string olduğu için string yapıyoruz)
        // Soru işareti (?) koyduk çünkü eski siparişlerde bu alan boş kalacak, patlamasın.
        public string? AppUserId { get; set; } 
        // 🔥 EKSİK OLAN BU SATIRI EKLE:
        public string SiparisNo { get; set; } = string.Empty; // Örn: 20260130123456
        public virtual AppUser? AppUser { get; set; } // Nullable yapıldı


        public string? KuponKodu { get; set; } // Kullanılan kupon (Opsiyonel)
        public decimal IndirimTutari { get; set; } // Kaç TL düştü?



        public string MusteriAdSoyad { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Eposta { get; set; } = string.Empty;
        public string Sehir { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string AcikAdres { get; set; } = string.Empty;
        
        public decimal ToplamTutar { get; set; }

        // Odeme durumu, operasyon durumundan ayri tutulur. Aksi halde henuz
        // tahsil edilmemis bir siparis yanlislikla uretime alinabilir.
        public string OdemeSaglayici { get; set; } = string.Empty;
        public string OdemeDurumu { get; set; } = "Bekliyor";
        public bool OdemeTamamlandiMi { get; set; }
        public DateTime? OdemeOnayTarihi { get; set; }
        public string? OdemeHataKodu { get; set; }
        public string? OdemeHataMesaji { get; set; }
        
        // Operasyon durumu; odeme durumu yukaridaki alanlarda ayrica izlenir.
        public int Durum { get; set; } 
        
        public string? KargoTakipNo { get; set; } 
        public int? KargoFirmasiId { get; set; }
        public string? KargoFirmasi { get; set; }
        public decimal KargoUcreti { get; set; } = 0;
        public string? Aciklama { get; set; }
        
        // GUEST CHECKOUT: Email ile sipariş takibi için hash kodu
        public string? EmailHashKodu { get; set; } // Örn: abc123def456 (unique)

        // --- FATURA ALANLARI ---
        public string? FaturaDosyaYolu { get; set; } // Örn: /uploads/invoices/12345_abc123.pdf
        public string? FaturaDosyaAdi { get; set; } // Örn: 12345_abc123.pdf
        public bool FaturaYuklendiMi { get; set; } = false;
        public DateTime? FaturaYuklenmeTarihi { get; set; }
        public bool FaturaMailGonderildiMi { get; set; } = false;
        public DateTime? FaturaMailGonderimTarihi { get; set; }

        public ICollection<SiparisDetay> SiparisDetaylari { get; set; } = new List<SiparisDetay>();
    }
}
