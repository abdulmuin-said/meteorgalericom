using System;

namespace KanvasProje.Core.Varliklar
{
    // Diğer tüm tablolar (Ürün, Sipariş, Adres) buradan miras alır
    public class TemelVarlik
    {
        public int Id { get; set; }
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
        public bool SilindiMi { get; set; } = false;
    }
}