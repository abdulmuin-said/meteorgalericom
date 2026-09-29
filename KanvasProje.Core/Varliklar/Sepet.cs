using System;
using System.Collections.Generic;

namespace KanvasProje.Core.Varliklar
{
    /// <summary>
    /// Database-backed sepet - Session yerine DB'de tutuluyor
    /// Hem üye hem misafir kullanıcılar için çalışır
    /// </summary>
    public class Sepet : BaseEntity
    {
        // Üye kullanıcı ise AppUserId dolu
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        
        // Misafir kullanıcı ise SessionId dolu (Browser session ID)
        public string? SessionId { get; set; }
        
        // Son güncelleme zamanı (abandoned cart tespiti için)
        public DateTime SonGuncellemeTarihi { get; set; }
        
        // Abandoned Cart Tracking
        public bool TerkEdildi { get; set; }
        public DateTime? TerkEdilmeTarihi { get; set; }
        public bool HatirlatmaGonderildi { get; set; } // Email gönderildi mi?
        
        // Sepetteki ürünler (OneToMany)
        public ICollection<SepetItem> SepetItems { get; set; } = new List<SepetItem>();
        
        // Helper: Sepet toplam tutarı
        public decimal ToplamTutar => SepetItems?.Sum(x => x.Fiyat * x.Adet) ?? 0;
    }
}
