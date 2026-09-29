using System.ComponentModel.DataAnnotations;

namespace KanvasProje.Core.Varliklar
{
    public class KurumsalSayfa : BaseEntity
    {
        [Required]
        public string Baslik { get; set; } = string.Empty; // Örn: Mesafeli Satış Sözleşmesi

        [Required]
        public string Icerik { get; set; } = string.Empty; // HTML Formatında sözleşme metni

        [Required]
        public string UrlSlug { get; set; } = string.Empty; // Örn: mesafeli-satis-sozlesmesi (Link için)
        
        public int Sira { get; set; } // Menüdeki sırası
    }
}