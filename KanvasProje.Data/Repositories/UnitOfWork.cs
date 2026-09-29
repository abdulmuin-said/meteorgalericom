using KanvasProje.Core.Interfaces;

namespace KanvasProje.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly KanvasDbContext _context;

        public UnitOfWork(KanvasDbContext context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}