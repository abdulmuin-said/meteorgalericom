using System;

namespace KanvasProje.Core.Varliklar
{
    // Bütün tablolarımız bu sınıftan miras alacak.
    // Böylece her tabloya tek tek Id yazmak zorunda kalmayacağız.
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        
        // Kayıt ne zaman eklendi?
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
        
        // Veriyi silersek tamamen yok etmeyelim, sadece 'Görünmez' yapalım.
        // True ise silinmiş demektir.
        public bool SilindiMi { get; set; } = false;
    }
}