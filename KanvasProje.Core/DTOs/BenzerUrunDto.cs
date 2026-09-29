namespace KanvasProje.Core.DTOs
{
    public class BenzerUrunDto
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? AnaGorselUrl { get; set; }
        public decimal Fiyat { get; set; }
        public decimal? IndirimliFiyat { get; set; }
    }
}