using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.Models;
using NhaHangThanhThao.Services;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Repositories
{
    public interface IHoaDonRepository
    {
        Task<bool> CoHoaDonChoDonAsync(string maDonGoiMon);
        Task<HoaDon?> TimTheoMaAsync(string maHoaDon);
        Task<List<HoaDonIndexViewModel>> LayDanhSachAsync();
        Task<List<HoaDonChiTietViewModel>> LayChiTietHoaDonAsync(string maHoaDon);
        Task<List<PhieuTamTinhChiTietViewModel>> LayChiTietTamTinhAsync(string maDonGoiMon);
        Task<decimal> LayTienCocAsync(string maDonDatBan, string maBan);
        void ThemHoaDon(HoaDon hoaDon);
        void ThemChiTiet(HoaDonChiTiet chiTiet);
    }

    public class HoaDonRepository : IHoaDonRepository
    {
        private readonly NhaHangThanhThaoContext _context;

        public HoaDonRepository(NhaHangThanhThaoContext context)
        {
            _context = context;
        }

        public Task<bool> CoHoaDonChoDonAsync(string maDonGoiMon)
        {
            return _context.HoaDons.AnyAsync(x => x.MaDonGoiMon == maDonGoiMon);
        }

        public Task<HoaDon?> TimTheoMaAsync(string maHoaDon)
        {
            return _context.HoaDons.FirstOrDefaultAsync(x => x.MaHoaDon == maHoaDon);
        }

        public Task<List<HoaDonIndexViewModel>> LayDanhSachAsync()
        {
            return (
                from hd in _context.HoaDons
                join dgm in _context.DonGoiMons
                    on hd.MaDonGoiMon equals dgm.MaDonGoiMon
                join ban in _context.BanChois
                    on dgm.MaBan equals ban.MaBan
                orderby hd.GioRa descending
                select new HoaDonIndexViewModel
                {
                    MaHoaDon = hd.MaHoaDon,
                    MaDonGoiMon = hd.MaDonGoiMon,
                    TenBan = ban.TenBan,
                    GioVao = hd.GioVao,
                    GioRa = hd.GioRa,
                    TongTienMon = hd.TongTienMon,
                    PhuThu = hd.PhuThu,
                    TienCocKhauTru = hd.TienCocKhauTru,
                    TongThanhToan = hd.TongThanhToan ?? 0
                }
            ).ToListAsync();
        }

        public Task<List<HoaDonChiTietViewModel>> LayChiTietHoaDonAsync(string maHoaDon)
        {
            return _context.HoaDonChiTiets
                .Where(x => x.MaHoaDon == maHoaDon)
                .Select(x => new HoaDonChiTietViewModel
                {
                    MaMon = x.MaMon,
                    TenMon = x.TenMon,
                    SoLuong = x.SoLuong,
                    DonGia = x.DonGia,
                    ThanhTien = x.SoLuong * x.DonGia
                })
                .ToListAsync();
        }

        public Task<List<PhieuTamTinhChiTietViewModel>> LayChiTietTamTinhAsync(string maDonGoiMon)
        {
            return (
                from ct in _context.ChiTietDonGoiMons
                join mon in _context.MonAns
                    on ct.MaMon equals mon.MaMon
                where ct.MaDonGoiMon == maDonGoiMon
                      && ct.TrangThai != TrangThaiNhaHang.ChiTietDaHuy
                      && ct.TrangThai != TrangThaiNhaHang.ChiTietTraMon
                group ct by new
                {
                    ct.MaMon,
                    mon.TenMon,
                    ct.DonGia
                }
                into g
                select new PhieuTamTinhChiTietViewModel
                {
                    MaMon = g.Key.MaMon,
                    TenMon = g.Key.TenMon,
                    DonGia = g.Key.DonGia,
                    SoLuong = g.Sum(x => x.SoLuong)
                }
            ).ToListAsync();
        }

        public async Task<decimal> LayTienCocAsync(string maDonDatBan, string maBan)
        {
            return await _context.ChiTietDonDatBans
                .Where(x => x.MaDonDatBan == maDonDatBan && x.MaBan == maBan)
                .SumAsync(x => (decimal?)x.TienCoc) ?? 0;
        }

        public void ThemHoaDon(HoaDon hoaDon)
        {
            _context.HoaDons.Add(hoaDon);
        }

        public void ThemChiTiet(HoaDonChiTiet chiTiet)
        {
            _context.HoaDonChiTiets.Add(chiTiet);
        }
    }
}
