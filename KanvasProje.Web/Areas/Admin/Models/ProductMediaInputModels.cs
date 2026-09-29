using KanvasProje.Service.Helpers;

namespace KanvasProje.Core.Models
{
    public abstract class ProductMediaInputModelBase
    {
        public int UrunId { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string AltMetin { get; set; } = string.Empty;
        public string MedyaTipi { get; set; } = UrunMedyaCatalog.Gorsel;
        public string MedyaAlani { get; set; } = UrunMedyaCatalog.Galeri;
        public string VideoUrl { get; set; } = string.Empty;
        public string Etiketler { get; set; } = string.Empty;
        public int Sira { get; set; }
        public bool VarsayilanMi { get; set; }
        public int? UrunSecenekId { get; set; }
    }

    public class ProductMediaCreateInputModel : ProductMediaInputModelBase
    {
    }

    public class ProductMediaUpdateInputModel : ProductMediaInputModelBase
    {
        public int Id { get; set; }
    }
}
