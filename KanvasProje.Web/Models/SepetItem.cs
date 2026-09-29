namespace KanvasProje.Web.Models
{
    public class SepetItem
    {
        public int UrunSecenekId { get; set; } // Hangi boyutu aldı? (Kritik ID)
        public int UrunId { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string ResimUrl { get; set; } = string.Empty;
        public string Ozellik { get; set; } = string.Empty; // Örn: 50x70 - Çerçevesiz
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
        
        // Toplam Fiyat (Adet x Fiyat)
        public decimal Toplam => Fiyat * Adet;

        public int VaryantId { get; internal set; }
        // BUNU EKLE: Özel tasarımın ID'sini tutmak için (Controller'da kullanabilirsin)
        public int? SenTasarlaId { get; set; }
    }
}