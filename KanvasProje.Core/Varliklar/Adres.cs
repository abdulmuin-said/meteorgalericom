using System;

namespace KanvasProje.Core.Varliklar
{
    // DÜZELTME: ": TemelVarlik" yerine ": BaseEntity" yazıyoruz.
    // Çünkü senin IService yapın BaseEntity bekliyor.
    public class Adres : BaseEntity
    {
        public string Baslik { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Sehir { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string AcikAdres { get; set; } = string.Empty;

        public string AppUserId { get; set; } = string.Empty;

public AppUser AppUser { get; set; } = default!;
    }
}