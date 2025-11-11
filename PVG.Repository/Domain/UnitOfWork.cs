using PVG.Infrastucture.Persistence;

namespace PVG.Infrastucture.Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PVGDbContext _context;

        public UnitOfWork(PVGDbContext context)
        {
            _context = context;
        }

        public Task<int> CommitAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}