using System.Collections.Generic;

namespace KanvasProje.Core.Varliklar
{
    public enum HomePageSectionType
    {
        HeroSlider = 0,
        ProductBlock = 1,
        CategoryShowcase = 2,
        CustomBanner = 3,
        AutoVitrin = 4,
        AutoCokSatanlar = 5,
        AutoFirsatUrunleri = 6,
        AutoBesParcali = 7
    }

    public class HomePageSection
    {
        public int Id { get; set; }
        public HomePageSectionType SectionType { get; set; }

        // Genel alanlar (aktif, sıralama, başlık vs.)
        public bool Enabled { get; set; } = true;
        public int SortOrder { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;

        // Ürün blokları için ek alanlar (View All linki)
        public string? ViewAllText { get; set; }
        public string? ViewAllUrl { get; set; }

        // Custom banner alanları
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? ButtonText { get; set; }
        public string? ButtonUrl { get; set; }

        // Ürün ilişkisi (manuel ürünler)
        public ICollection<HomePageSectionProduct> SectionProducts { get; set; } = new List<HomePageSectionProduct>();
    }

    public class HomePageSectionProduct
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public int UrunId { get; set; }
        public int SortOrder { get; set; }

        public HomePageSection Section { get; set; } = null!;
        public Urun Urun { get; set; } = null!;
    }
}
