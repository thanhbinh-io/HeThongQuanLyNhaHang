using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.Models;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Controllers
{
    public class KhachGoiMonController : Controller
    {
        private readonly NhaHangThanhThaoContext _context;

        public KhachGoiMonController(NhaHangThanhThaoContext context)
        {
            _context = context;
        }

        [HttpGet("qr/{token}")]
        public async Task<IActionResult> Menu(string token, string? maDanhMuc)
        {
            var ban = await _context.BanChois
                .FirstOrDefaultAsync(x => x.Qrtoken == token);

            if (ban == null)
            {
                return NotFound("Mã QR không hợp lệ.");
            }

            if (ban.TrangThai == "NgungSuDung")
            {
                return Content("Bàn/chòi này đang ngừng sử dụng, không thể gọi món.");
            }
            if (ban.TrangThai == "DaDatTruoc")
            {
                var daCoDonDangPhucVu = await _context.DonGoiMons
                    .AnyAsync(x => x.MaBan == ban.MaBan && x.TrangThai == "DangPhucVu");

                if (!daCoDonDangPhucVu)
                {
                    return RedirectToAction(nameof(XacNhanDatBan), new { token });
                }
            }
            var danhSachDanhMuc = await _context.DanhMucMons
                .OrderBy(x => x.TenDanhMuc)
                .Select(x => new KhachGoiMonDanhMucViewModel
                {
                    MaDanhMuc = x.MaDanhMuc,
                    TenDanhMuc = x.TenDanhMuc
                })
                .ToListAsync();

            var query =
                from mon in _context.MonAns
                join dm in _context.DanhMucMons
                    on mon.MaDanhMuc equals dm.MaDanhMuc
                where mon.TrangThai == "DangKinhDoanh"
                select new
                {
                    mon.MaMon,
                    mon.TenMon,
                    mon.DonViTinh,
                    mon.DonGia,
                    mon.MoTa,
                    mon.HinhAnh,
                    mon.MaDanhMuc,
                    dm.TenDanhMuc
                };

            if (!string.IsNullOrEmpty(maDanhMuc))
            {
                query = query.Where(x => x.MaDanhMuc == maDanhMuc);
            }

            var danhSachMon = await query
                .OrderBy(x => x.TenDanhMuc)
                .ThenBy(x => x.TenMon)
                .Select(x => new KhachGoiMonMonAnViewModel
                {
                    MaMon = x.MaMon,
                    TenMon = x.TenMon,
                    TenDanhMuc = x.TenDanhMuc,
                    DonViTinh = x.DonViTinh,
                    DonGia = x.DonGia,
                    MoTa = x.MoTa,
                    HinhAnh = x.HinhAnh
                })
                .ToListAsync();

            var donDangPhucVu = await _context.DonGoiMons
                .Where(x => x.MaBan == ban.MaBan && x.TrangThai == "DangPhucVu")
                .OrderByDescending(x => x.ThoiGianTao)
                .FirstOrDefaultAsync();

            var danhSachMonDaGoi = new List<KhachGoiMonMonDaGoiViewModel>();

            if (donDangPhucVu != null)
            {
                danhSachMonDaGoi = await (
                    from ct in _context.ChiTietDonGoiMons
                    join mon in _context.MonAns
                        on ct.MaMon equals mon.MaMon
                    where ct.MaDonGoiMon == donDangPhucVu.MaDonGoiMon
                    orderby ct.MaChiTietDonGoiMon descending
                    select new KhachGoiMonMonDaGoiViewModel
                    {
                        TenMon = mon.TenMon,
                        SoLuong = ct.SoLuong,
                        DonGia = ct.DonGia,
                        ThanhTien = ct.ThanhTien ?? ct.SoLuong * ct.DonGia,
                        TrangThai = ct.TrangThai,
                        GhiChu = ct.GhiChu
                    }
                ).ToListAsync();
            }

            var model = new KhachGoiMonViewModel
            {
                QRToken = token,
                MaBan = ban.MaBan,
                TenBan = ban.TenBan,
                TrangThaiBan = ban.TrangThai,
                MaDanhMucDangChon = maDanhMuc,
                DanhSachDanhMuc = danhSachDanhMuc,
                DanhSachMon = danhSachMon,
                DanhSachMonDaGoi = danhSachMonDaGoi
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> XacNhanDatBan(string token)
        {
            var ban = await _context.BanChois
                .FirstOrDefaultAsync(x => x.Qrtoken == token);

            if (ban == null)
            {
                return NotFound("Mã QR không hợp lệ.");
            }

            if (ban.TrangThai != "DaDatTruoc")
            {
                return RedirectToAction(nameof(Menu), new { token });
            }

            var model = new XacNhanDatBanViewModel
            {
                QRToken = token,
                MaBan = ban.MaBan,
                TenBan = ban.TenBan
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanDatBan(XacNhanDatBanViewModel model)
        {
            var ban = await _context.BanChois
                .FirstOrDefaultAsync(x => x.Qrtoken == model.QRToken);

            if (ban == null)
            {
                return NotFound("Mã QR không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                model.MaBan = ban.MaBan;
                model.TenBan = ban.TenBan;
                return View(model);
            }

            var donDatBan = await (
                from ddb in _context.DonDatBans
                join ct in _context.ChiTietDonDatBans
                    on ddb.MaDonDatBan equals ct.MaDonDatBan
                where ct.MaBan == ban.MaBan
                      && ddb.SoDienThoai == model.SoDienThoai
                      && !_context.DonGoiMons.Any(dgm => dgm.MaDonDatBan == ddb.MaDonDatBan)
                orderby ddb.ThoiGianDen descending
                select ddb
            ).FirstOrDefaultAsync();

            if (donDatBan == null)
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại không đúng hoặc đơn đặt bàn đã được nhận.");
                model.MaBan = ban.MaBan;
                model.TenBan = ban.TenBan;
                return View(model);
            }

            string maDonGoiMon = "DGM" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

            var donGoiMon = new DonGoiMon
            {
                MaDonGoiMon = maDonGoiMon,
                MaBan = ban.MaBan,
                MaDonDatBan = donDatBan.MaDonDatBan,
                MaNguoiTao = null,
                NguonTao = "KhachQR",
                ThoiGianTao = DateTime.Now,
                TrangThai = "DangPhucVu",
                GhiChu = "Khách xác nhận đặt bàn bằng số điện thoại"
            };

            _context.DonGoiMons.Add(donGoiMon);
            ban.TrangThai = "DangPhucVu";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Menu), new { token = model.QRToken });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoiMon(string qrToken, string maMon, int soLuong, string? ghiChu)
        {
            if (string.IsNullOrEmpty(qrToken) || string.IsNullOrEmpty(maMon))
            {
                return BadRequest();
            }

            if (soLuong <= 0)
            {
                return RedirectToAction(nameof(Menu), new { token = qrToken });
            }

            var ban = await _context.BanChois
                .FirstOrDefaultAsync(x => x.Qrtoken == qrToken);

            if (ban == null)
            {
                return NotFound("Mã QR không hợp lệ.");
            }

            if (ban.TrangThai == "NgungSuDung")
            {
                return Content("Bàn/chòi này đang ngừng sử dụng, không thể gọi món.");
            }
            if (ban.TrangThai == "DaDatTruoc")
            {
                var daCoDonDangPhucVu = await _context.DonGoiMons
                    .AnyAsync(x => x.MaBan == ban.MaBan && x.TrangThai == "DangPhucVu");

                if (!daCoDonDangPhucVu)
                {
                    return RedirectToAction(nameof(XacNhanDatBan), new { token = qrToken });
                }
            }
            var monAn = await _context.MonAns
                .FirstOrDefaultAsync(x => x.MaMon == maMon && x.TrangThai == "DangKinhDoanh");

            if (monAn == null)
            {
                return RedirectToAction(nameof(Menu), new { token = qrToken });
            }

            var donGoiMon = await _context.DonGoiMons
                .FirstOrDefaultAsync(x => x.MaBan == ban.MaBan && x.TrangThai == "DangPhucVu");

            if (donGoiMon == null)
            {
                string maDonGoiMon = "DGM" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                donGoiMon = new DonGoiMon
                {
                    MaDonGoiMon = maDonGoiMon,
                    MaBan = ban.MaBan,
                    MaDonDatBan = null,
                    MaNguoiTao = null,
                    NguonTao = "KhachQR",
                    ThoiGianTao = DateTime.Now,
                    TrangThai = "DangPhucVu",
                    GhiChu = "Khách gọi món qua QR"
                };

                _context.DonGoiMons.Add(donGoiMon);

                ban.TrangThai = "DangPhucVu";
            }

            var chiTiet = new ChiTietDonGoiMon
            {
                MaDonGoiMon = donGoiMon.MaDonGoiMon,
                MaMon = monAn.MaMon,
                SoLuong = soLuong,
                DonGia = monAn.DonGia,
                GhiChu = ghiChu,
                TrangThai = "Moi"
            };

            _context.ChiTietDonGoiMons.Add(chiTiet);

            await _context.SaveChangesAsync();

            TempData["ThongBao"] = $"Đã gửi món: {monAn.TenMon}";

            return RedirectToAction(nameof(Menu), new { token = qrToken });
        }
    }
}
