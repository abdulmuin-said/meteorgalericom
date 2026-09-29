using Microsoft.AspNetCore.Identity;

namespace KanvasProje.Core.Varliklar
{
    // IdentityUser'dan miras alıyoruz, yani standart özelliklerin üzerine ekleme yapıyoruz
    public class AppUser : IdentityUser
    {
        public string AdSoyad { get; set; } = string.Empty;
        public string Sehir { get; set; } = string.Empty;
        // KanvasProje.Core.Varliklar.AppUser.cs içinde olmalı:

        public string? SifreSifirlamaToken { get; set; }
        public DateTime? SifreSifirlamaGecerlilik { get; set; }
        
        // İleride puan sistemi yaparsak diye:
        // public int Puan { get; set; }
    }
}