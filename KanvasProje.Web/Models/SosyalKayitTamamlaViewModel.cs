using System.ComponentModel.DataAnnotations;

namespace KanvasProje.Web.Models
{
    public class SosyalKayitTamamlaViewModel
    {
        public string AdSoyad { get; set; } = string.Empty;
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir seçimi zorunludur")]
        public string Sehir { get; set; } = string.Empty;
    }
}
