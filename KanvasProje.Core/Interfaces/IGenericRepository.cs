using System.Linq.Expressions;
using KanvasProje.Core.Varliklar;

namespace KanvasProje.Core.Interfaces
{
    // <T> demek: Bu depo her tablo için çalışabilir (Urun, Kategori, Siparis...)
    public interface IGenericRepository<T> where T : BaseEntity
    {
        // Id'ye göre veri getir
        Task<T?> GetByIdAsync(int id);

        // Hepsini getir
        Task<IEnumerable<T>> GetAllAsync();

        // Filtreleyerek getir (Örn: Fiyatı 500'den büyük olanlar)
        IQueryable<T> Where(Expression<Func<T, bool>> expression);

        // Ekleme
        Task AddAsync(T entity);

        // Silme (Bizim sistemde aslında 'SilindiMi = true' yapacağız)
        void Remove(T entity);

        // Güncelleme
        void Update(T entity);
    }
}