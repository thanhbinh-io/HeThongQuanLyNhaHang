using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.Models;
using NhaHangThanhThao.Services;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Repositories
{
    public interface IDonGoiMonRepository
    {
        Task<List<DonGoiMonIndexViewModel>> LayDanhSachAsync();
        Task<DonGoiMon?> TimTheoMaAsync(string maDonGoiMon);
        Task<bool> CoDonDangPhucVuAsync(string maBan);
        Task<List<ChiTietDonGoiMonViewModel>> LayChiTietAsync(string maDonGoiMon);
        Task<List<PhieuBepMonViewModel>> LayMonMoiPhieuBepAsync(string maDonGoiMon);
        Task<bool> CoMonDaChuyenBepAsync(string maDonGoiMon);
        Task<ChiTietDonGoiMon?> TimChiTietAsync(int id);
        Task<List<ChiTietDonGoiMon>> LayChiTietTheoIdsAsync(List<int> ids);
        void ThemDonGoiMon(DonGoiMon donGoiMon);
        void ThemChiTiet(ChiTietDonGoiMon chiTiet);
    }

    public class DonGoiMonRepository : IDonGoiMonRepository
    {
        private readonly NhaHangThanhThaoContext _context;

        public DonGoiMonRepository(NhaHangThanhThaoContext context)
        {
            _context = context;
        }

        public Task<List<DonGoiMonIndexViewModel>> LayDanhSachAsync()
        {
            return (
                from dgm in _context.DonGoiMons
                join ban in _context.BanChois
                    on dgm.MaBan equals ban.MaBan
                orderby dgm.ThoiGianTao descending
                select new DonGoiMonIndexViewModel
                {
                    MaDonGoiMon = dgm.MaDonGoiMon,
                    MaBan = ban.MaBan,
                    TenBan = ban.TenBan,
                    ThoiGianTao = dgm.ThoiGianTao,
                    TrangThai = dgm.TrangThai,
                    TongTien = _context.ChiTietDonGoiMons
                        .Where(ct => ct.MaDonGoiMon == dgm.MaDonGoiMon
                                     && ct.TrangThai != TrangThaiNhaHang.ChiTietDaHuy
                                     && ct.TrangThai != TrangThaiNhaHang.ChiTietTraMon)
                        .Sum(ct => (decimal?)(ct.SoLuong * ct.DonGia)) ?? 0
                }
            ).ToListAsync();
        }

        public Task<DonGoiMon?> TimTheoMaAsync(string maDonGoiMon)
        {
            return _context.DonGoiMons.FindAsync(maDonGoiMon).AsTask();
        }

        public Task<bool> CoDonDangPhucVuAsync(string maBan)
        {
            return _context.DonGoiMons
                .AnyAsync(x => x.MaBan == maBan
                               && x.TrangThai == TrangThaiNhaHang.DonDangPhucVu);
        }

        public Task<List<ChiTietDonGoiMonViewModel>> LayChiTietAsync(string maDonGoiMon)
        {
            return (
                from ct in _context.ChiTietDonGoiMons
                join mon in _context.MonAns
                    on ct.MaMon equals mon.MaMon
                where ct.MaDonGoiMon == maDonGoiMon
                select new ChiTietDonGoiMonViewModel
                {
                    MaChiTietDonGoiMon = ct.MaChiTietDonGoiMon,
                    MaMon = ct.MaMon,
                    TenMon = mon.TenMon,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia,
                    ThanhTien = ct.SoLuong * ct.DonGia,
                    TrangThai = ct.TrangThai,
                    GhiChu = ct.GhiChu
                }
            ).ToListAsync();
        }

        public Task<ChiTietDonGoiMon?> TimChiTietAsync(int id)
        {
            return _context.ChiTietDonGoiMons.FindAsync(id).AsTask();
        }

        public Task<List<PhieuBepMonViewModel>> LayMonMoiPhieuBepAsync(string maDonGoiMon)
        {
            return (
                from ct in _context.ChiTietDonGoiMons
                join mon in _context.MonAns
                    on ct.MaMon equals mon.MaMon
                where ct.MaDonGoiMon == maDonGoiMon
                      && ct.TrangThai == TrangThaiNhaHang.ChiTietMoi
                orderby ct.MaChiTietDonGoiMon
                select new PhieuBepMonViewModel
                {
                    MaChiTietDonGoiMon = ct.MaChiTietDonGoiMon,
                    TenMon = mon.TenMon,
                    DonViTinh = mon.DonViTinh,
                    SoLuong = ct.SoLuong,
                    GhiChu = ct.GhiChu
                }
            ).ToListAsync();
        }

        public Task<bool> CoMonDaChuyenBepAsync(string maDonGoiMon)
        {
            return _context.ChiTietDonGoiMons
                .AnyAsync(x => x.MaDonGoiMon == maDonGoiMon
                               && (x.TrangThai == TrangThaiNhaHang.ChiTietDangCheBien
                                   || x.TrangThai == TrangThaiNhaHang.ChiTietDaPhucVu));
        }

        public Task<List<ChiTietDonGoiMon>> LayChiTietTheoIdsAsync(List<int> ids)
        {
            return _context.ChiTietDonGoiMons
                .Where(x => ids.Contains(x.MaChiTietDonGoiMon))
                .ToListAsync();
        }

        public void ThemDonGoiMon(DonGoiMon donGoiMon)
        {
            _context.DonGoiMons.Add(donGoiMon);
        }

        public void ThemChiTiet(ChiTietDonGoiMon chiTiet)
        {
            _context.ChiTietDonGoiMons.Add(chiTiet);
        }
    }
}
