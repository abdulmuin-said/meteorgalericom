using System;
using System.ComponentModel.DataAnnotations;

namespace KanvasProje.Core.Varliklar
{
    public class IletisimMesaj
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AdSoyad { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Konu { get; set; } = string.Empty;

        [Required]
        public string Mesaj { get; set; } = string.Empty;

        public DateTime Tarih { get; set; } = DateTime.UtcNow.AddHours(3);
        public string? IpAdresi { get; set; }
        public bool OkunduMu { get; set; } = false; // Admin okuyunca true yaparız
    }
}