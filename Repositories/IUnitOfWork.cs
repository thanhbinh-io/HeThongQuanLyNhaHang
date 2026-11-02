using NhaHangThanhThao.Data;

namespace NhaHangThanhThao.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly NhaHangThanhThaoContext _context;

        public UnitOfWork(NhaHangThanhThaoContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
