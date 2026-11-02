using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.Models;
using NhaHangThanhThao.Services;

namespace NhaHangThanhThao.Repositories
{
    public interface IBanChoiRepository
    {
        Task<List<BanChoi>> LayTatCaAsync();
        Task<List<BanChoi>> LayBanTrongAsync();
        Task<List<BanChoi>> LayBanCoTheGoiMonAsync();
        Task<BanChoi?> TimTheoMaAsync(string maBan);
        Task<BanChoi?> TimTheoQrTokenAsync(string qrToken);
    }

    public class BanChoiRepository : IBanChoiRepository
    {
        private readonly NhaHangThanhThaoContext _context;

        public BanChoiRepository(NhaHangThanhThaoContext context)
        {
            _context = context;
        }

        public Task<List<BanChoi>> LayTatCaAsync()
        {
            return _context.BanChois
                .OrderBy(x => x.KhuVuc)
                .ThenBy(x => x.MaBan)
                .ToListAsync();
        }

        public Task<List<BanChoi>> LayBanTrongAsync()
        {
            return _context.BanChois
                .Where(x => x.TrangThai == TrangThaiNhaHang.BanTrong)
                .OrderBy(x => x.KhuVuc)
                .ThenBy(x => x.MaBan)
                .ToListAsync();
        }

        public Task<List<BanChoi>> LayBanCoTheGoiMonAsync()
        {
            return _context.BanChois
                .Where(x => x.TrangThai == TrangThaiNhaHang.BanTrong
                            || x.TrangThai == TrangThaiNhaHang.BanDaDatTruoc)
                .OrderBy(x => x.KhuVuc)
                .ThenBy(x => x.MaBan)
                .ToListAsync();
        }

        public Task<BanChoi?> TimTheoMaAsync(string maBan)
        {
            return _context.BanChois.FindAsync(maBan).AsTask();
        }

        public Task<BanChoi?> TimTheoQrTokenAsync(string qrToken)
        {
            return _context.BanChois.FirstOrDefaultAsync(x => x.Qrtoken == qrToken);
        }
    }
}
