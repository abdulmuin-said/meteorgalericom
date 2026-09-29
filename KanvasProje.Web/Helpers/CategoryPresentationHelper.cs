using KanvasProje.Core.Varliklar;
using KanvasProje.Web.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KanvasProje.Core.Helpers
{
    public static class CategoryPresentationHelper
    {
        private static readonly (string Icon, string Color, string BgColor)[] FallbackPalette =
        {
            ("fas fa-shapes", "#1565C0", "#E3F2FD"),
            ("fas fa-couch", "#6D4C41", "#EFEBE9"),
            ("fas fa-leaf", "#2E7D32", "#E8F5E9"),
            ("fas fa-lightbulb", "#F57C00", "#FFF3E0"),
            ("fas fa-gem", "#7B1FA2", "#F3E5F5"),
            ("fas fa-paint-roller", "#00838F", "#E0F7FA"),
            ("fas fa-border-all", "#5D4037", "#F5F5F5"),
            ("fas fa-image", "#C62828", "#FFEBEE")
        };

        public static CategoryViewModel ToViewModel(Kategori category)
        {
            var keywordMatch = TryMatchByKeyword(category.Ad);
            var visual = keywordMatch ?? GetDeterministicFallback(category.Slug ?? category.Ad);

            return new CategoryViewModel
            {
                Id = category.Id,
                Ad = category.Ad,
                Icon = visual.Icon,
                Color = visual.Color,
                BgColor = visual.BgColor
            };
        }

        public static string BuildHierarchyLabel(Kategori category, IReadOnlyDictionary<int, Kategori>? lookup = null)
        {
            if (category.ParentKategoriId == null || lookup == null || lookup.Count == 0)
            {
                return category.Ad;
            }

            var labels = new List<string> { category.Ad };
            var visited = new HashSet<int> { category.Id };
            var parentId = category.ParentKategoriId;

            while (parentId.HasValue && lookup.TryGetValue(parentId.Value, out var parent) && visited.Add(parent.Id))
            {
                labels.Add(parent.Ad);
                parentId = parent.ParentKategoriId;
            }

            labels.Reverse();
            return string.Join(" > ", labels);
        }

        private static (string Icon, string Color, string BgColor)? TryMatchByKeyword(string? categoryName)
        {
            var value = (categoryName ?? string.Empty).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (value.Contains("hali") || value.Contains("kilim")) return ("fas fa-rug", "#8D6E63", "#EFEBE9");
            if (value.Contains("ayna")) return ("fas fa-circle-notch", "#546E7A", "#ECEFF1");
            if (value.Contains("mobilya") || value.Contains("sehpa")) return ("fas fa-couch", "#6D4C41", "#EFEBE9");
            if (value.Contains("duvar kag") || value.Contains("wallpaper")) return ("fas fa-scroll", "#5E35B1", "#EDE7F6");
            if (value.Contains("cam")) return ("fas fa-clone", "#0277BD", "#E1F5FE");
            if (value.Contains("kanvas") || value.Contains("tablo")) return ("fas fa-image", "#C62828", "#FFEBEE");
            if (value.Contains("dekor")) return ("fas fa-star", "#F9A825", "#FFF8E1");
            if (value.Contains("tekstil")) return ("fas fa-feather-pointed", "#2E7D32", "#E8F5E9");
            return null;
        }

        private static (string Icon, string Color, string BgColor) GetDeterministicFallback(string seed)
        {
            var hash = (seed ?? string.Empty).Aggregate(17, (current, character) => current * 31 + character) & int.MaxValue;
            return FallbackPalette[hash % FallbackPalette.Length];
        }
    }
}
