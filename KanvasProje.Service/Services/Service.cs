using System.Linq.Expressions;
using KanvasProje.Core.Interfaces;
using KanvasProje.Core.Varliklar;
using KanvasProje.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KanvasProje.Service.Services
{
    public class Service<T> : IService<T> where T : BaseEntity
    {
        private readonly IGenericRepository<T> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService? _cacheService;

        public Service(IGenericRepository<T> repository, IUnitOfWork unitOfWork, ICacheService? cacheService = null)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task RemoveAsync(T entity)
        {
            _repository.Remove(entity);
            await _unitOfWork.CommitAsync();
            InvalidateCacheForEntity(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
            InvalidateCacheForEntity(entity);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _repository.Where(expression);
        }

        private void InvalidateCacheForEntity(T entity)
        {
            if (_cacheService == null) return;
            
            var entityType = typeof(T).Name;
            var cacheKeys = new[]
            {
                $"urunler",
                $"urunler_listesi",
                $"kategoriler",
                $"kategoriler_tree",
                $"anasayfa_urunleri",
                $"kategori_{entityType.ToLower()}"
            };

            foreach (var key in cacheKeys)
            {
                try { _cacheService.RemoveAsync(key).Wait(); } catch { }
            }
        }
    }
}