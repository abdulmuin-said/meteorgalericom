using KanvasProje.Data;
using KanvasProje.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanvasProje.Web.ViewComponents
{
    public class CategoryMenu : ViewComponent
    {
        private readonly KanvasDbContext _context;
        private readonly KanvasProje.Service.Interfaces.ICacheService _cacheService;

        public CategoryMenu(KanvasDbContext context, KanvasProje.Service.Interfaces.ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _cacheService.GetOrAddAsync("home_categories", async () =>
            {
                var categories = await _context.Kategoriler
                    .AsNoTracking()
                    .Where(x => x.AktifMi && !x.SilindiMi && x.ParentKategoriId == null)
                    .OrderBy(x => x.Sira)
                    .ThenBy(x => x.Ad)
                    .Take(9)
                    .ToListAsync();

                return categories
                    .Select(CategoryPresentationHelper.ToViewModel)
                    .ToList();
            }, TimeSpan.FromHours(12));

            return View(model);
        }
    }

    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string BgColor { get; set; } = string.Empty;
    }
}
