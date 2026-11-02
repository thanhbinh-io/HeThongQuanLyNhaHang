using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.Models;
using NhaHangThanhThao.Services;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Controllers
{
    public class NguoiDungController : Controller
    {
        private const string TrangThaiHoatDong = "HoatDong";
        private const string TrangThaiKhoa = "Khoa";

        private readonly NhaHangThanhThaoContext _context;
        private readonly IMatKhauService _matKhauService;

        public NguoiDungController(NhaHangThanhThaoContext context, IMatKhauService matKhauService)
        {
            _context = context;
            _matKhauService = matKhauService;
        }

        private bool LaQuanLy()
        {
            return HttpContext.Session.GetString("MaVaiTro") == "QL";
        }

        private int? MaNguoiDungHienTai()
        {
            return HttpContext.Session.GetInt32("MaNguoiDung");
        }

        private static string ChuanHoaChuoi(string value)
        {
            return value.Trim();
        }

        private static bool TrangThaiHopLe(string trangThai)
        {
            return trangThai == TrangThaiHoatDong || trangThai == TrangThaiKhoa;
        }

        private async Task NapVaiTro()
        {
            ViewBag.VaiTros = new SelectList(
                await _context.VaiTros.OrderBy(x => x.TenVaiTro).ToListAsync(),
                "MaVaiTro",
                "TenVaiTro"
            );
        }

        private void KiemTraTrangThai(string trangThai)
        {
            if (!TrangThaiHopLe(trangThai))
            {
                ModelState.AddModelError("TrangThai", "Trạng thái tài khoản không hợp lệ");
            }
        }

        private async Task KiemTraTrungTaiKhoanAsync(string tenDangNhap, string soDienThoai, int? maNguoiDungBoQua = null)
        {
            bool trungTenDangNhap = await _context.NguoiDungs
                .AnyAsync(x => x.TenDangNhap == tenDangNhap
                               && (!maNguoiDungBoQua.HasValue || x.MaNguoiDung != maNguoiDungBoQua.Value));

            if (trungTenDangNhap)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã tồn tại");
            }

            bool trungSoDienThoai = await _context.NguoiDungs
                .AnyAsync(x => x.SoDienThoai == soDienThoai
                               && (!maNguoiDungBoQua.HasValue || x.MaNguoiDung != maNguoiDungBoQua.Value));

            if (trungSoDienThoai)
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã tồn tại");
            }
        }

        public async Task<IActionResult> Index()
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            int? maNguoiDungHienTai = MaNguoiDungHienTai();

            var danhSach = await _context.NguoiDungs
                .Include(x => x.MaVaiTroNavigation)
                .OrderBy(x => x.MaVaiTro)
                .ThenBy(x => x.HoTen)
                .Select(x => new NguoiDungIndexViewModel
                {
                    MaNguoiDung = x.MaNguoiDung,
                    HoTen = x.HoTen,
                    SoDienThoai = x.SoDienThoai,
                    TenDangNhap = x.TenDangNhap,
                    MaVaiTro = x.MaVaiTro,
                    TenVaiTro = x.MaVaiTroNavigation.TenVaiTro,
                    TrangThai = x.TrangThai,
                    LaTaiKhoanHienTai = maNguoiDungHienTai.HasValue && x.MaNguoiDung == maNguoiDungHienTai.Value
                })
                .ToListAsync();

            return View(danhSach);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            await NapVaiTro();
            return View(new NguoiDungCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NguoiDungCreateViewModel model)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            model.HoTen = ChuanHoaChuoi(model.HoTen);
            model.SoDienThoai = ChuanHoaChuoi(model.SoDienThoai);
            model.TenDangNhap = ChuanHoaChuoi(model.TenDangNhap);
            model.MatKhau = ChuanHoaChuoi(model.MatKhau);

            KiemTraTrangThai(model.TrangThai);
            await KiemTraTrungTaiKhoanAsync(model.TenDangNhap, model.SoDienThoai);

            if (!ModelState.IsValid)
            {
                await NapVaiTro();
                return View(model);
            }

            var nguoiDung = new NguoiDung
            {
                HoTen = model.HoTen,
                SoDienThoai = model.SoDienThoai,
                TenDangNhap = model.TenDangNhap,
                MaVaiTro = model.MaVaiTro,
                TrangThai = model.TrangThai,
                MatKhau = string.Empty
            };

            nguoiDung.MatKhau = _matKhauService.BamMatKhau(nguoiDung, model.MatKhau);

            _context.NguoiDungs.Add(nguoiDung);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã thêm tài khoản nhân viên.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            var nguoiDung = await _context.NguoiDungs.FindAsync(id);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            var model = new NguoiDungEditViewModel
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                HoTen = nguoiDung.HoTen,
                SoDienThoai = nguoiDung.SoDienThoai,
                TenDangNhap = nguoiDung.TenDangNhap,
                MaVaiTro = nguoiDung.MaVaiTro,
                TrangThai = nguoiDung.TrangThai,
                LaTaiKhoanHienTai = MaNguoiDungHienTai() == nguoiDung.MaNguoiDung
            };

            await NapVaiTro();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NguoiDungEditViewModel model)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.MaNguoiDung)
            {
                return NotFound();
            }

            model.HoTen = ChuanHoaChuoi(model.HoTen);
            model.SoDienThoai = ChuanHoaChuoi(model.SoDienThoai);
            model.TenDangNhap = ChuanHoaChuoi(model.TenDangNhap);
            model.MatKhauMoi = string.IsNullOrWhiteSpace(model.MatKhauMoi)
                ? null
                : ChuanHoaChuoi(model.MatKhauMoi);
            model.LaTaiKhoanHienTai = MaNguoiDungHienTai() == model.MaNguoiDung;

            KiemTraTrangThai(model.TrangThai);
            await KiemTraTrungTaiKhoanAsync(model.TenDangNhap, model.SoDienThoai, model.MaNguoiDung);

            if (model.LaTaiKhoanHienTai && (model.TrangThai != TrangThaiHoatDong || model.MaVaiTro != "QL"))
            {
                ModelState.AddModelError("", "Không thể tự khóa hoặc bỏ quyền quản lý của tài khoản đang đăng nhập.");
            }

            if (!ModelState.IsValid)
            {
                await NapVaiTro();
                return View(model);
            }

            var nguoiDung = await _context.NguoiDungs.FindAsync(id);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            nguoiDung.HoTen = model.HoTen;
            nguoiDung.SoDienThoai = model.SoDienThoai;
            nguoiDung.TenDangNhap = model.TenDangNhap;
            nguoiDung.MaVaiTro = model.MaVaiTro;
            nguoiDung.TrangThai = model.TrangThai;

            if (!string.IsNullOrEmpty(model.MatKhauMoi))
            {
                nguoiDung.MatKhau = _matKhauService.BamMatKhau(nguoiDung, model.MatKhauMoi);
            }

            await _context.SaveChangesAsync();

            if (model.LaTaiKhoanHienTai)
            {
                HttpContext.Session.SetString("HoTen", nguoiDung.HoTen);
                HttpContext.Session.SetString("MaVaiTro", nguoiDung.MaVaiTro);
            }

            TempData["Success"] = "Đã cập nhật tài khoản nhân viên.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KhoaMo(int id)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (MaNguoiDungHienTai() == id)
            {
                TempData["Error"] = "Không thể tự khóa tài khoản đang đăng nhập.";
                return RedirectToAction(nameof(Index));
            }

            var nguoiDung = await _context.NguoiDungs.FindAsync(id);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            nguoiDung.TrangThai = nguoiDung.TrangThai == TrangThaiHoatDong
                ? TrangThaiKhoa
                : TrangThaiHoatDong;

            await _context.SaveChangesAsync();

            TempData["Success"] = nguoiDung.TrangThai == TrangThaiHoatDong
                ? "Đã mở khóa tài khoản."
                : "Đã khóa tài khoản.";

            return RedirectToAction(nameof(Index));
        }
    }
}
