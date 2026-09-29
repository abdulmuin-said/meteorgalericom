using System.ComponentModel.DataAnnotations;

namespace KanvasProje.Web.Models
{
    public class KayitViewModel
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        public string AdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-Posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz")]
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir zorunludur")]
        public string Sehir { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 3 karakter olmalı")]
        public string Sifre { get; set; } = string.Empty;
        
        [Compare("Sifre", ErrorMessage = "Şifreler uyuşmuyor")]
        public string SifreTekrar { get; set; } = string.Empty;
    }
}