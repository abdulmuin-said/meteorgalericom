using KanvasProje.Core.Varliklar;
using System.Collections.Generic;
using System.Linq;

namespace KanvasProje.Core.Helpers
{
    public static class CategoryTreeHelper
    {
        public static List<int> GetDescendantIds(IEnumerable<Kategori> categories, int rootCategoryId)
        {
            var categoryList = categories.ToList();
            var categoryIds = categoryList.Select(x => x.Id).ToHashSet();
            var lookup = categoryList
                .Where(x => x.ParentKategoriId.HasValue && categoryIds.Contains(x.ParentKategoriId.Value))
                .GroupBy(x => x.ParentKategoriId!.Value)
                .ToDictionary(x => x.Key, x => x.Select(c => c.Id).ToList());

            var result = new List<int>();
            var stack = new Stack<int>();
            var visited = new HashSet<int>();

            stack.Push(rootCategoryId);

            while (stack.Count > 0)
            {
                var currentId = stack.Pop();
                if (!visited.Add(currentId))
                {
                    continue;
                }

                result.Add(currentId);

                if (lookup.TryGetValue(currentId, out var children))
                {
                    foreach (var childId in children)
                    {
                        stack.Push(childId);
                    }
                }
            }

            return result;
        }

        public static List<(Kategori Category, int Depth)> FlattenHierarchy(IEnumerable<Kategori> categories, int? excludedId = null)
        {
            var allCategories = categories.ToList();
            var categoryIds = allCategories.Select(x => x.Id).ToHashSet();
            var roots = allCategories
                .Where(x => !x.ParentKategoriId.HasValue || !categoryIds.Contains(x.ParentKategoriId.Value))
                .OrderBy(c => c.Sira)
                .ThenBy(c => c.Ad)
                .ToList();
            var lookup = allCategories
                .Where(x => x.ParentKategoriId.HasValue && categoryIds.Contains(x.ParentKategoriId.Value))
                .GroupBy(x => x.ParentKategoriId!.Value)
                .ToDictionary(x => x.Key, x => x.OrderBy(c => c.Sira).ThenBy(c => c.Ad).ToList());

            var result = new List<(Kategori Category, int Depth)>();
            var visited = new HashSet<int>();

            void Walk(Kategori node, int depth)
            {
                if (!visited.Add(node.Id))
                {
                    return;
                }

                if (excludedId.HasValue && node.Id == excludedId.Value)
                {
                    return;
                }

                result.Add((node, depth));

                if (!lookup.TryGetValue(node.Id, out var children))
                {
                    return;
                }

                foreach (var child in children)
                {
                    Walk(child, depth + 1);
                }
            }

            foreach (var root in roots)
            {
                Walk(root, 0);
            }

            return result;
        }

        public static bool IsDescendant(IEnumerable<Kategori> categories, int categoryId, int potentialParentId)
        {
            var descendantIds = GetDescendantIds(categories, categoryId);
            return descendantIds.Contains(potentialParentId);
        }
    }
}
