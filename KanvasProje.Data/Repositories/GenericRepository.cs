using System.Linq.Expressions;
using KanvasProje.Core.Interfaces;
using KanvasProje.Core.Varliklar;
using Microsoft.EntityFrameworkCore;

namespace KanvasProje.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly KanvasDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(KanvasDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Remove(T entity)
        {
            entity.SilindiMi = true;
            _dbSet.Update(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }
    }
}
