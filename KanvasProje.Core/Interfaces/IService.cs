using System.Linq.Expressions;
using KanvasProje.Core.Varliklar;

namespace KanvasProje.Core.Interfaces
{
    // Repository'ye benziyor ama dönüş tipleri farklı olabilir.
    // Ayrıca burada veritabanı işlemleri değil, 'İş Kuralları' dönecek.
    public interface IService<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(); // Tüm listeyi getir
        
        // Filtreleme (Örn: Fiyatı 100 üstü olanlar)
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        
        Task<T> AddAsync(T entity); // Ekle ve eklenen veriyi geri döndür
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
    }
}