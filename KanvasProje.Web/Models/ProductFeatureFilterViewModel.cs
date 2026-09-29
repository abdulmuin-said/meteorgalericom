namespace KanvasProje.Web.Models
{
    public class ProductFeatureFilterViewModel
    {
        public int DefinitionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public IReadOnlyList<ProductFeatureFilterOptionViewModel> Options { get; set; } = Array.Empty<ProductFeatureFilterOptionViewModel>();
    }

    public class ProductFeatureFilterOptionViewModel
    {
        public string Value { get; set; } = string.Empty;
        public string DisplayValue { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
