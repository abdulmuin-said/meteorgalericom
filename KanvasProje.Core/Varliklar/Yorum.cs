using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KanvasProje.Core.Varliklar
{
    public class Yorum : BaseEntity
    {
        public int UrunId { get; set; }
        [ForeignKey("UrunId")]
        public Urun Urun { get; set; } = default!;

        public string? AppUserId { get; set; } // Gerçek kullanıcı ise dolar
        
        [Required]
        [StringLength(50)]
        public string AdSoyad { get; set; } = string.Empty; // Bot ise "Ahmet K." gibi yazacağız

        [Required]
        public string YorumMetni { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Puan { get; set; } // 1 ile 5 arası

        public bool OnayliMi { get; set; } // Admin onayı
    }
}