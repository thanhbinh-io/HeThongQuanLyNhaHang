using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.Models;

namespace NhaHangThanhThao.Controllers
{
    public class MonAnController : Controller
    {
        private static readonly HashSet<string> DinhDangAnhChoPhep = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        private readonly NhaHangThanhThaoContext _context;
        private readonly IWebHostEnvironment _environment;

        public MonAnController(NhaHangThanhThaoContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private bool LaQuanLy()
        {
            return HttpContext.Session.GetString("MaVaiTro") == "QL";
        }

        private async Task NapDanhMucMon()
        {
            ViewBag.DanhMucMons = new SelectList(
                await _context.DanhMucMons.OrderBy(x => x.TenDanhMuc).ToListAsync(),
                "MaDanhMuc",
                "TenDanhMuc"
            );
        }

        private static string LamSachTenFile(string value)
        {
            var chars = value
                .Trim()
                .ToLowerInvariant()
                .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
                .ToArray();

            string safeName = string.Join("-", new string(chars).Split('-', StringSplitOptions.RemoveEmptyEntries));

            return string.IsNullOrWhiteSpace(safeName) ? "mon-an" : safeName;
        }

        private static string LayAnhMacDinhTheoDanhMuc(string? maDanhMuc)
        {
            return maDanhMuc switch
            {
                "BE_BE" or "GOI" or "CO_TRAI" or "MONG_TAY" or "TOM" or "CUA_GHE" => "/images/mon-an/hai-san.jpg",
                "DO_UONG" => "/images/mon-an/do-uong.jpg",
                "VIT_TROI" or "GA_TA" or "DAC_BIET" or "NAM_BE" or "NGONG" or "KHAI_VI" or "LAU" => "/images/mon-an/mon-nuong.jpg",
                _ => "/images/mon-an/default.jpg"
            };
        }

        private async Task<string?> LuuAnhMonAsync(IFormFile? hinhAnhFile, string maMon)
        {
            if (hinhAnhFile == null || hinhAnhFile.Length == 0)
            {
                return null;
            }

            string extension = Path.GetExtension(hinhAnhFile.FileName);

            if (!DinhDangAnhChoPhep.Contains(extension))
            {
                ModelState.AddModelError("HinhAnh", "Chỉ cho phép ảnh .jpg, .jpeg, .png hoặc .webp");
                return null;
            }

            if (hinhAnhFile.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("HinhAnh", "Ảnh món không được vượt quá 2MB");
                return null;
            }

            string folder = Path.Combine(_environment.WebRootPath, "images", "mon-an");
            Directory.CreateDirectory(folder);

            string safeName = LamSachTenFile(maMon);
            string fileName = $"{safeName}-{DateTime.Now:yyyyMMddHHmmssfff}{extension.ToLowerInvariant()}";
            string filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await hinhAnhFile.CopyToAsync(stream);
            }

            return $"/images/mon-an/{fileName}";
        }

        public async Task<IActionResult> Index()
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            var danhSachMon = await _context.MonAns
                .Include(x => x.MaDanhMucNavigation)
                .OrderBy(x => x.MaDanhMuc)
                .ThenBy(x => x.MaMon)
                .ToListAsync();

            return View(danhSachMon);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            await NapDanhMucMon();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonAn monAn, IFormFile? hinhAnhFile)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                await NapDanhMucMon();
                return View(monAn);
            }

            string? anhUpload = await LuuAnhMonAsync(hinhAnhFile, monAn.MaMon);

            if (!ModelState.IsValid)
            {
                await NapDanhMucMon();
                return View(monAn);
            }

            monAn.HinhAnh = anhUpload ?? LayAnhMacDinhTheoDanhMuc(monAn.MaDanhMuc);

            _context.MonAns.Add(monAn);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var monAn = await _context.MonAns.FindAsync(id);

            if (monAn == null)
            {
                return NotFound();
            }

            await NapDanhMucMon();
            return View(monAn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, MonAn model, IFormFile? hinhAnhFile)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.MaMon)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await NapDanhMucMon();
                return View(model);
            }

            string? anhUpload = await LuuAnhMonAsync(hinhAnhFile, model.MaMon);

            if (!ModelState.IsValid)
            {
                await NapDanhMucMon();
                return View(model);
            }

            var monAn = await _context.MonAns.FindAsync(id);

            if (monAn == null)
            {
                return NotFound();
            }

            monAn.TenMon = model.TenMon;
            monAn.DonViTinh = model.DonViTinh;
            monAn.MaDanhMuc = model.MaDanhMuc;
            monAn.DonGia = model.DonGia;
            monAn.HinhAnh = anhUpload ?? monAn.HinhAnh ?? LayAnhMacDinhTheoDanhMuc(model.MaDanhMuc);
            monAn.MoTa = model.MoTa;
            monAn.TrangThai = model.TrangThai;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
