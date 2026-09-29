using KanvasProje.Core.Varliklar;
using KanvasProje.Data;
using KanvasProje.Service.Interfaces;
using KanvasProje.Core.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KanvasProje.Service.Services
{
    public class UrunService : IUrunService
    {
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private readonly KanvasDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public UrunService(KanvasDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public async Task<List<Urun>> GetUrunlerListesiForAdminAsync(string? search, int? kategoriId)
        {
            var urunlerQuery = _context.Urunler.Include(u => u.Kategori).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLowerInvariant();
                urunlerQuery = urunlerQuery.Where(x =>
                    x.Baslik.ToLower().Contains(term) ||
                    x.SKU.ToLower().Contains(term) ||
                    x.Barkod.ToLower().Contains(term) ||
                    x.Id.ToString() == term);
            }

            if (kategoriId.HasValue)
            {
                urunlerQuery = urunlerQuery.Where(x =>
                    x.KategoriId == kategoriId.Value ||
                    (x.Kategori != null && x.Kategori.ParentKategoriId == kategoriId.Value));
            }

            return await urunlerQuery
                .OrderBy(x => x.Sira)
                .ThenByDescending(x => x.OlusturulmaTarihi)
                .ToListAsync();
        }

        public async Task<Urun?> GetUrunByIdAsync(int id)
        {
            return await _context.Urunler
                .Include(x => x.UrunSecenek)
                .Include(x => x.UrunOzellikleri)
                    .ThenInclude(x => x.UrunOzellikTanimi)
                .Include(x => x.UrunResimleri)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<bool> EkleAsync(Urun urun, IFormFile? resimDosyasi)
        {
            return Task.FromException<bool>(new NotImplementedException("UrunService.EkleAsync henuz uygulanmadi."));
        }

        public Task<bool> GuncelleAsync(int id, Urun model, IFormFile? anaResimDosyasi, List<IFormFile>? galeriDosyalari)
        {
            return Task.FromException<bool>(new NotImplementedException("UrunService.GuncelleAsync henuz uygulanmadi."));
        }

        public async Task SilAsync(int id)
        {
            var urun = await _context.Urunler.FirstOrDefaultAsync(x => x.Id == id);
            if (urun != null)
            {
                urun.SilindiMi = true;
                urun.AktifMi = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResimSilAsync(int resimId)
        {
            var resim = await _context.UrunResimleri
                .Include(x => x.Urun)
                    .ThenInclude(x => x.UrunResimleri)
                .FirstOrDefaultAsync(x => x.Id == resimId);
                
            if (resim != null)
            {
                _context.UrunResimleri.Remove(resim);
                resim.Urun.UrunResimleri.Remove(resim);
                await _context.SaveChangesAsync();
            }
        }
    }
}
