namespace KanvasProje.Core.Models
{
    public class HomePageSettingsModel
    {
        public HomeHeroBlockSettings Hero { get; set; } = new();
        public HomeSimpleBlockSettings Categories { get; set; } = new();
        public HomeProductBlockSettings FeaturedProducts { get; set; } = new();
        public HomeProductBlockSettings BestSellers { get; set; } = new();
        public HomeProductBlockSettings Deals { get; set; } = new();
        public HomeFeaturesBlockSettings Features { get; set; } = new();
        public HomeNewsletterBlockSettings Newsletter { get; set; } = new();
    }

    public abstract class HomeBlockSettingsBase
    {
        public bool Enabled { get; set; } = true;
        public int SortOrder { get; set; }
    }

    public class HomeSimpleBlockSettings : HomeBlockSettingsBase
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
    }

    public class HomeProductBlockSettings : HomeSimpleBlockSettings
    {
        public string ViewAllText { get; set; } = string.Empty;
        public string ViewAllUrl { get; set; } = string.Empty;
    }

    public class HomeHeroBlockSettings : HomeBlockSettingsBase
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string PrimaryButtonText { get; set; } = string.Empty;
        public string PrimaryButtonUrl { get; set; } = string.Empty;
        public string SecondaryButtonText { get; set; } = string.Empty;
        public string SecondaryButtonUrl { get; set; } = string.Empty;
        public List<HomeHeroSlideSettings> DesktopSlides { get; set; } = new();
        public List<HomeHeroSlideSettings> MobileSlides { get; set; } = new();
    }

    public class HomeHeroSlideSettings
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string ImageUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ButtonText { get; set; } = string.Empty;
        public string ButtonUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }

    public class HomeFeaturesBlockSettings : HomeSimpleBlockSettings
    {
        public List<HomeFeatureItemSettings> Items { get; set; } = new();
    }

    public class HomeFeatureItemSettings
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class HomeNewsletterBlockSettings : HomeSimpleBlockSettings
    {
        public string PlaceholderText { get; set; } = string.Empty;
        public string ButtonText { get; set; } = string.Empty;
    }
}
