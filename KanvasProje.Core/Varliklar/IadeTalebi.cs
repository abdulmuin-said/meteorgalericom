using System.ComponentModel.DataAnnotations;

namespace KanvasProje.Core.Varliklar
{
    public class IadeTalebi : BaseEntity
    {
        public int SiparisId { get; set; }
        public virtual Siparis Siparis { get; set; } = default!;

        public string MusteriId { get; set; } = string.Empty; // Hangi müşteri
        
        [Required]
        public string Neden { get; set; } = string.Empty; // Ürün hasarlı, Yanlış ürün, Beğenmedim vb.

        public string? Aciklama { get; set; } // Müşterinin yazdığı detay

        public string? IBAN { get; set; } // Para iadesi için

        // Durumlar: 0: Bekliyor, 1: Onaylandı (Kargo Bekleniyor), 2: İade Tamamlandı, 3: Reddedildi
        public int Durum { get; set; } 
        
        public string? AdminNotu { get; set; } // Reddedilirse neden reddedildiği
    }
}