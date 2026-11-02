using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.Models;
using NhaHangThanhThao.Services;

namespace NhaHangThanhThao.Repositories
{
    public interface IMonAnRepository
    {
        Task<List<MonAn>> LayMonDangKinhDoanhAsync();
        Task<MonAn?> TimTheoMaAsync(string maMon);
        Task<MonAn?> TimMonCoTheGoiAsync(string maMon);
    }

    public class MonAnRepository : IMonAnRepository
    {
        private readonly NhaHangThanhThaoContext _context;

        public MonAnRepository(NhaHangThanhThaoContext context)
        {
            _context = context;
        }

        public Task<List<MonAn>> LayMonDangKinhDoanhAsync()
        {
            return _context.MonAns
                .Where(x => x.TrangThai == TrangThaiNhaHang.MonDangKinhDoanh)
                .OrderBy(x => x.MaDanhMuc)
                .ThenBy(x => x.TenMon)
                .ToListAsync();
        }

        public Task<MonAn?> TimTheoMaAsync(string maMon)
        {
            return _context.MonAns.FindAsync(maMon).AsTask();
        }

        public Task<MonAn?> TimMonCoTheGoiAsync(string maMon)
        {
            return _context.MonAns
                .FirstOrDefaultAsync(x => x.MaMon == maMon
                                          && x.TrangThai == TrangThaiNhaHang.MonDangKinhDoanh);
        }
    }
}
