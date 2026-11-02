using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NhaHangThanhThao.Services;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Controllers
{
    public class DonGoiMonController : Controller
    {
        private readonly IDonGoiMonService _donGoiMonService;

        public DonGoiMonController(IDonGoiMonService donGoiMonService)
        {
            _donGoiMonService = donGoiMonService;
        }

        private bool DuocGoiMon()
        {
            var maVaiTro = HttpContext.Session.GetString("MaVaiTro");
            return maVaiTro == "QL" || maVaiTro == "NV";
        }

        private async Task NapDanhSachBan()
        {
            var danhSachBan = await _donGoiMonService.LayBanCoTheGoiMonAsync();

            ViewBag.BanChois = new SelectList(
                danhSachBan,
                "MaBan",
                "TenBan"
            );
        }

        private async Task NapDanhSachMon()
        {
            var danhSachMon = await _donGoiMonService.LayMonDangKinhDoanhAsync();

            ViewBag.MonAns = new SelectList(
                danhSachMon,
                "MaMon",
                "TenMon"
            );
        }

        private void GanLoiVaoModelState(ServiceResult result)
        {
            if (!string.IsNullOrEmpty(result.ThongBao))
            {
                ModelState.AddModelError(result.TruongLoi ?? "", result.ThongBao);
            }
        }

        public async Task<IActionResult> Index()
        {
            if (!DuocGoiMon())
            {
                return RedirectToAction("Login", "Account");
            }

            var danhSach = await _donGoiMonService.LayDanhSachAsync();

            return View(danhSach);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!DuocGoiMon())
            {
                return RedirectToAction("Login", "Account");
            }

            await NapDanhSachBan();
            await NapDanhSachMon();

            return View(new DonGoiMonCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonGoiMonCreateViewModel model)
        {
            if (!DuocGoiMon())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                await NapDanhSachBan();
                await NapDanhSachMon();
                return View(model);
            }

            var maNguoiTao = HttpContext.Session.GetInt32("MaNguoiDung");
            var result = await _donGoiMonService.TaoDonGoiMonAsync(model, maNguoiTao);

            if (result.ChuyenVeDangNhap)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!result.ThanhCong)
            {
                GanLoiVaoModelState(result);
                await NapDanhSachBan();
                await NapDanhSachMon();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            if (!DuocGoiMon())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var result = await _donGoiMonService.LayChiTietAsync(id);

            if (result.KhongTimThay || result.DuLieu == null)
            {
                return NotFound();
            }

            return View(result.DuLieu);
        }

        [HttpGet]
        public async Task<IActionResult> PhieuBep(string id)
        {
            if (!DuocGoiMon())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var result = await _donGoiMonService.TaoPhieuBepAsync(id, chuyenSangDangCheBien: false);

            if (result.KhongTimThay || result.DuLieu == null)
            {
                return NotFound();
            }

            if (result.ChuyenVeChiTietDon)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(result.DuLieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InPhieuBep(string id)
        {
            if (!DuocGoiMon())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var result = await _donGoiMonService.TaoPhieuBepAsync(id, chuyenSangDangCheBien: true);

            if (result.KhongTimThay || result.DuLieu == null)
            {
                return NotFound();
            }

            if (result.ChuyenVeChiTietDon)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            return View("PhieuBep", result.DuLieu);
        }

        [HttpGet]
        public async Task<IActionResult> AddMon(string id)
        {
            if (!DuocGoiMon())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var result = await _donGoiMonService.TaoModelThemMonAsync(id);

            if (result.KhongTimThay)
            {
                return NotFound();
            }

            if (result.ChuyenVeChiTietDon)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            await NapDanhSachMon();

            return View(result.DuLieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMon(ThemMonVaoDonViewModel model)
        {
            if (!DuocGoiMon())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                await NapDanhSachMon();
                return View(model);
            }

            var result = await _donGoiMonService.ThemMonAsync(model);

            if (result.KhongTimThay)
            {
                return NotFound();
            }

            if (result.ChuyenVeChiTietDon)
            {
                return RedirectToAction(nameof(Details), new { id = model.MaDonGoiMon });
            }

            if (!result.ThanhCong)
            {
                GanLoiVaoModelState(result);
                await NapDanhSachMon();
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = model.MaDonGoiMon });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThaiMon(int id, string trangThai)
        {
            if (!DuocGoiMon())
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _donGoiMonService.CapNhatTrangThaiMonAsync(id, trangThai);

            if (result.KhongTimThay)
            {
                return NotFound();
            }

            if (result.ChuyenVeChiTietDon)
            {
                return RedirectToAction(nameof(Details), new { id = result.MaCanChuyen });
            }

            if (!result.ThanhCong)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Details), new { id = result.MaMoi });
        }
    }
}
