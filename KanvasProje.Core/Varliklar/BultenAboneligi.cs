using System;
using System.ComponentModel.DataAnnotations;

namespace KanvasProje.Core.Varliklar
{
    public class BultenAboneligi
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;

        public bool AktifMi { get; set; } = true; // İlerde abonelikten çıkarsa false yaparız
        public string? IpAdresi { get; set; }
    }
}