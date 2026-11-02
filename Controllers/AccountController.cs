using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.Services;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Controllers
{
    public class AccountController : Controller
    {
        private readonly NhaHangThanhThaoContext _context;
        private readonly IMatKhauService _matKhauService;

        public AccountController(NhaHangThanhThaoContext context, IMatKhauService matKhauService)
        {
            _context = context;
            _matKhauService = matKhauService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var tenDangNhap = model.TenDangNhap.Trim();
            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(x =>
                    x.TenDangNhap == tenDangNhap &&
                    x.TrangThai == "HoatDong");

            if (nguoiDung == null || !_matKhauService.KiemTraMatKhau(nguoiDung, model.MatKhau))
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View(model);
            }

            if (_matKhauService.CanNangCapMatKhau(nguoiDung, model.MatKhau))
            {
                nguoiDung.MatKhau = _matKhauService.BamMatKhau(nguoiDung, model.MatKhau);
                await _context.SaveChangesAsync();
            }

            HttpContext.Session.SetInt32("MaNguoiDung", nguoiDung.MaNguoiDung);
            HttpContext.Session.SetString("HoTen", nguoiDung.HoTen);
            HttpContext.Session.SetString("MaVaiTro", nguoiDung.MaVaiTro);

            if (nguoiDung.MaVaiTro == "QL")
            {
                return RedirectToAction("Index", "QuanLy");
            }

            if (nguoiDung.MaVaiTro == "NV")
            {
                return RedirectToAction("Index", "PhucVu");
            }

            ViewBag.Error = "Tài khoản chưa được phân quyền phù hợp";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
