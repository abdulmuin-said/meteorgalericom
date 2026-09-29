using System.ComponentModel.DataAnnotations;

namespace KanvasProje.Web.Models
{
    public class SifreSifirlaViewModel
    {
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifre zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalı")]
        public string YeniSifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur")]
        [Compare("YeniSifre", ErrorMessage = "Şifreler uyuşmuyor")]
        public string SifreTekrar { get; set; } = string.Empty;
    }
}
