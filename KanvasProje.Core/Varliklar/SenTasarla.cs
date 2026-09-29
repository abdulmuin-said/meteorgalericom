using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KanvasProje.Core.Varliklar
{
    public class SenTasarla
    {
        [Key]
        public int Id { get; set; }
        
        // [Required] <-- BUNU KALDIRIYORUZ (Misafirler için)
        public string? KullaniciId { get; set; } // Soru işareti ile nullable yaptık
        
        [Required]
        [StringLength(200)]
        public string DosyaYolu { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Olcu { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Fiyat { get; set; }
        
        [StringLength(20)]
        public string Efekt { get; set; } = string.Empty;
        
        public bool CercveliMi { get; set; }
        
        [StringLength(50)]
        public string ParcaSayisi { get; set; } = string.Empty;
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
        
        public bool SepeteEklendi { get; set; } = false;
        
        [StringLength(50)]
        public string SessionId { get; set; } = string.Empty;
    }
}