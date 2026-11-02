using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;

namespace NhaHangThanhThao.Repositories
{
    public interface IThamSoRepository
    {
        Task<decimal?> LayGiaTriAsync(string tenThamSo);
    }

    public class ThamSoRepository : IThamSoRepository
    {
        private readonly NhaHangThanhThaoContext _context;

        public ThamSoRepository(NhaHangThanhThaoContext context)
        {
            _context = context;
        }

        public async Task<decimal?> LayGiaTriAsync(string tenThamSo)
        {
            var thamSo = await _context.ThamSos
                .FirstOrDefaultAsync(x => x.TenThamSo == tenThamSo);

            return thamSo?.GiaTri;
        }
    }
}
