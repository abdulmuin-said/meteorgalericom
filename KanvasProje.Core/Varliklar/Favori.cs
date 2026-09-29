using System.ComponentModel.DataAnnotations.Schema;

namespace KanvasProje.Core.Varliklar
{
    public class Favori : BaseEntity
    {
        public string AppUserId { get; set; } = default!;

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { get; set; } = default!;

        public int UrunId { get; set; }

        [ForeignKey("UrunId")]
        public virtual Urun Urun { get; set; } = default!;

        // Fiyat düşünce bildirimi için alanlar
        public bool FiyatDustugundaBildir { get; set; }
        public decimal? EskiFiyat { get; set; }
        public DateTime? SonBildirimTarihi { get; set; }
    }
}