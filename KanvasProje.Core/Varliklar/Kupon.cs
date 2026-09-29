using System;
using System.ComponentModel.DataAnnotations;

namespace KanvasProje.Core.Varliklar
{
    public class Kupon : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string Kod { get; set; } = string.Empty; // Örn: YAZ50

        public int Tip { get; set; } // 0: Yüzde, 1: Tutar (TL)
        
        public decimal Deger { get; set; } // %10 veya 100 TL
        
        public decimal MinSepetTutari { get; set; } // Alt limit (Örn: 500 TL üzeri)
        
        public DateTime SonKullanmaTarihi { get; set; }
        
        public int KullanimLimiti { get; set; } // Kaç kişi kullanabilir?
        public int KullanilanMiktar { get; set; } // Şu ana kadar kaç kişi kullandı?
        
        public bool AktifMi { get; set; }
    }
}