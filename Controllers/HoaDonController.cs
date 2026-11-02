using Microsoft.AspNetCore.Mvc;
using NhaHangThanhThao.Services;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly IHoaDonService _hoaDonService;

        public HoaDonController(IHoaDonService hoaDonService)
        {
            _hoaDonService = hoaDonService;
        }

        private bool LaQuanLy()
        {
            return HttpContext.Session.GetString("MaVaiTro") == "QL";
        }

        [HttpGet]
        public async Task<IActionResult> TamTinh(string id)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var result = await _hoaDonService.TaoPhieuTamTinhAsync(id);

            if (result.KhongTimThay || result.DuLieu == null)
            {
                return NotFound();
            }

            if (result.ChuyenVeChiTietDon)
            {
                return RedirectToAction("Details", "DonGoiMon", new { id });
            }

            return View(result.DuLieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanThanhToan(PhieuTamTinhViewModel model)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _hoaDonService.XacNhanThanhToanAsync(model);

            if (result.KhongTimThay)
            {
                return NotFound();
            }

            if (result.ChuyenVeChiTietDon)
            {
                return RedirectToAction("Details", "DonGoiMon", new { id = result.MaCanChuyen ?? model.MaDonGoiMon });
            }

            if (!result.ThanhCong)
            {
                var phieuTamTinh = await _hoaDonService.TaoPhieuTamTinhAsync(model.MaDonGoiMon, model.PhuThu);
                ModelState.AddModelError("", result.ThongBao ?? "Không thể thanh toán đơn gọi món");
                return View("TamTinh", phieuTamTinh.DuLieu ?? model);
            }

            return RedirectToAction("Details", "HoaDon", new { id = result.MaMoi });
        }

        public async Task<IActionResult> Details(string id)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var result = await _hoaDonService.LayChiTietHoaDonAsync(id);

            if (result.KhongTimThay || result.DuLieu == null)
            {
                return NotFound();
            }

            return View(result.DuLieu);
        }

        public async Task<IActionResult> Index()
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            var danhSachHoaDon = await _hoaDonService.LayDanhSachAsync();

            return View(danhSachHoaDon);
        }
    }
}
