using KanvasProje.Core.Varliklar;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KanvasProje.Service.Interfaces
{
    public interface IUrunService
    {
        Task<List<Urun>> GetUrunlerListesiForAdminAsync(string? search, int? kategoriId);
        Task<Urun?> GetUrunByIdAsync(int id);
        
        // This accepts complex objects and handles IFormFile logic
        Task<bool> EkleAsync(Urun urun, IFormFile? resimDosyasi);
        Task<bool> GuncelleAsync(int id, Urun model, IFormFile? anaResimDosyasi, List<IFormFile>? galeriDosyalari);
        Task SilAsync(int id);
        Task ResimSilAsync(int resimId);
    }
}
