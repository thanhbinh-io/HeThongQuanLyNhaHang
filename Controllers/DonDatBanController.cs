using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NhaHangThanhThao.Services;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Controllers
{
    public class DonDatBanController : Controller
    {
        private readonly IDonDatBanService _donDatBanService;

        public DonDatBanController(IDonDatBanService donDatBanService)
        {
            _donDatBanService = donDatBanService;
        }

        private bool LaQuanLy()
        {
            return HttpContext.Session.GetString("MaVaiTro") == "QL";
        }

        private async Task NapDanhSachBan()
        {
            var danhSachBan = await _donDatBanService.LayTatCaBanAsync();

            ViewBag.BanChois = danhSachBan.Select(ban => new SelectListItem
            {
                Value = ban.MaBan,
                Text = $"{ban.TenBan} - {ban.KhuVuc}"
            }).ToList();
        }

        private void GanLoiVaoModelState(ServiceResult result)
        {
            if (!string.IsNullOrEmpty(result.ThongBao))
            {
                ModelState.AddModelError(result.TruongLoi ?? "", result.ThongBao);
            }
        }

        public async Task<IActionResult> Index(DateTime? ngayDat)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            var danhSach = await _donDatBanService.LayDanhSachAsync(ngayDat);
            ViewBag.NgayDat = ngayDat;

            return View(danhSach);
        }

        [HttpGet]
        public async Task<IActionResult> PhieuDatBan(string id)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var result = await _donDatBanService.LayPhieuDatBanAsync(id);

            if (result.KhongTimThay || result.DuLieu == null)
            {
                return NotFound();
            }

            return View(result.DuLieu);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            await NapDanhSachBan();

            return View(new DonDatBanCreateViewModel
            {
                MaDonDatBan = "Tự động tạo khi lưu"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonDatBanCreateViewModel model)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            ModelState.Remove(nameof(DonDatBanCreateViewModel.MaDonDatBan));

            if (!ModelState.IsValid)
            {
                await NapDanhSachBan();
                return View(model);
            }

            var maNguoiLap = HttpContext.Session.GetInt32("MaNguoiDung");
            var result = await _donDatBanService.TaoDonDatBanAsync(model, maNguoiLap);

            if (result.ChuyenVeDangNhap)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!result.ThanhCong)
            {
                GanLoiVaoModelState(result);
                await NapDanhSachBan();
                return View(model);
            }

            TempData["Success"] = $"Đã lập đơn đặt bàn {result.MaMoi}.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChuyenSangDaDat(string maDonDatBan, string maBan)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _donDatBanService.ChuyenSangDaDatAsync(maDonDatBan, maBan);

            if (result.KhongTimThay)
            {
                return NotFound();
            }

            if (!result.ThanhCong)
            {
                TempData["Error"] = result.ThongBao ?? "Không thể chuyển bàn/chòi sang trạng thái đã đặt.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Đã chuyển bàn/chòi sang trạng thái đã đặt trước.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(string id)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var result = await _donDatBanService.XoaDonDatBanAsync(id);

            if (result.KhongTimThay)
            {
                return NotFound();
            }

            if (!result.ThanhCong)
            {
                TempData["Error"] = result.ThongBao ?? "Không thể xóa đơn đặt bàn.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Đã xóa đơn đặt bàn.";

            return RedirectToAction(nameof(Index));
        }
    }
}
