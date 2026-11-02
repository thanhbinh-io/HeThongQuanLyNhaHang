using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.Models;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Repositories
{
    public interface IDonDatBanRepository
    {
        Task<List<DonDatBanIndexViewModel>> LayDanhSachAsync(DateTime? ngayDat = null);
        Task<bool> BanDaCoDonTrongNgayAsync(string maBan, DateTime thoiGianDen, string? maDonBoQua = null);
        Task<bool> DaPhatSinhDonGoiMonAsync(string maDonDatBan);
        Task<DonDatBan?> TimTheoMaAsync(string maDonDatBan);
        Task<PhieuDatBanViewModel?> LayPhieuDatBanAsync(string maDonDatBan);
        Task<List<ChiTietDonDatBan>> LayChiTietTheoDonAsync(string maDonDatBan);
        void ThemDonDatBan(DonDatBan donDatBan);
        void ThemChiTiet(ChiTietDonDatBan chiTiet);
        void XoaDonDatBan(DonDatBan donDatBan);
        void XoaChiTiet(IEnumerable<ChiTietDonDatBan> chiTiets);
    }

    public class DonDatBanRepository : IDonDatBanRepository
    {
        private readonly NhaHangThanhThaoContext _context;

        public DonDatBanRepository(NhaHangThanhThaoContext context)
        {
            _context = context;
        }

        public Task<List<DonDatBanIndexViewModel>> LayDanhSachAsync(DateTime? ngayDat = null)
        {
            var query =
                from ddb in _context.DonDatBans
                join ct in _context.ChiTietDonDatBans
                    on ddb.MaDonDatBan equals ct.MaDonDatBan
                join ban in _context.BanChois
                    on ct.MaBan equals ban.MaBan
                select new
                {
                    ddb.MaDonDatBan,
                    ddb.TenKhachHang,
                    ddb.SoDienThoai,
                    ddb.ThoiGianDen,
                    ddb.SoLuongKhach,
                    ban.MaBan,
                    ban.TenBan,
                    ct.TienCoc,
                    ddb.GhiChu
                };

            if (ngayDat.HasValue)
            {
                DateTime batDau = ngayDat.Value.Date;
                DateTime ketThuc = batDau.AddDays(1);
                query = query.Where(x => x.ThoiGianDen >= batDau && x.ThoiGianDen < ketThuc);
            }

            return query
                .OrderByDescending(x => x.ThoiGianDen)
                .Select(x => new DonDatBanIndexViewModel
                {
                    MaDonDatBan = x.MaDonDatBan,
                    TenKhachHang = x.TenKhachHang,
                    SoDienThoai = x.SoDienThoai,
                    ThoiGianDen = x.ThoiGianDen,
                    SoLuongKhach = x.SoLuongKhach,
                    MaBan = x.MaBan,
                    TenBan = x.TenBan,
                    TienCoc = x.TienCoc,
                    GhiChu = x.GhiChu
                })
                .ToListAsync();
        }

        public Task<bool> BanDaCoDonTrongNgayAsync(string maBan, DateTime thoiGianDen, string? maDonBoQua = null)
        {
            DateTime batDau = thoiGianDen.Date;
            DateTime ketThuc = batDau.AddDays(1);

            return (
                from ddb in _context.DonDatBans
                join ct in _context.ChiTietDonDatBans
                    on ddb.MaDonDatBan equals ct.MaDonDatBan
                where ct.MaBan == maBan
                      && ddb.ThoiGianDen >= batDau
                      && ddb.ThoiGianDen < ketThuc
                      && (maDonBoQua == null || ddb.MaDonDatBan != maDonBoQua)
                select ddb
            ).AnyAsync();
        }

        public Task<DonDatBan?> TimTheoMaAsync(string maDonDatBan)
        {
            return _context.DonDatBans.FindAsync(maDonDatBan).AsTask();
        }

        public Task<bool> DaPhatSinhDonGoiMonAsync(string maDonDatBan)
        {
            return _context.DonGoiMons.AnyAsync(x => x.MaDonDatBan == maDonDatBan);
        }

        public Task<PhieuDatBanViewModel?> LayPhieuDatBanAsync(string maDonDatBan)
        {
            return (
                from ddb in _context.DonDatBans
                join ct in _context.ChiTietDonDatBans
                    on ddb.MaDonDatBan equals ct.MaDonDatBan
                join ban in _context.BanChois
                    on ct.MaBan equals ban.MaBan
                where ddb.MaDonDatBan == maDonDatBan
                select new PhieuDatBanViewModel
                {
                    MaDonDatBan = ddb.MaDonDatBan,
                    TenKhachHang = ddb.TenKhachHang,
                    SoDienThoai = ddb.SoDienThoai,
                    NgayLap = ddb.NgayLap,
                    ThoiGianDen = ddb.ThoiGianDen,
                    SoLuongKhach = ddb.SoLuongKhach,
                    MaBan = ban.MaBan,
                    TenBan = ban.TenBan,
                    KhuVuc = ban.KhuVuc,
                    TienCoc = ct.TienCoc,
                    GhiChu = ddb.GhiChu ?? ct.GhiChu
                }
            ).FirstOrDefaultAsync();
        }

        public void ThemDonDatBan(DonDatBan donDatBan)
        {
            _context.DonDatBans.Add(donDatBan);
        }

        public void ThemChiTiet(ChiTietDonDatBan chiTiet)
        {
            _context.ChiTietDonDatBans.Add(chiTiet);
        }

        public Task<List<ChiTietDonDatBan>> LayChiTietTheoDonAsync(string maDonDatBan)
        {
            return _context.ChiTietDonDatBans
                .Where(x => x.MaDonDatBan == maDonDatBan)
                .ToListAsync();
        }

        public void XoaDonDatBan(DonDatBan donDatBan)
        {
            _context.DonDatBans.Remove(donDatBan);
        }

        public void XoaChiTiet(IEnumerable<ChiTietDonDatBan> chiTiets)
        {
            _context.ChiTietDonDatBans.RemoveRange(chiTiets);
        }
    }
}
