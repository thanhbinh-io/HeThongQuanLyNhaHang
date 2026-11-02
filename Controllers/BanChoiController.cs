using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.Models;

using QRCoder;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Controllers
{
    public class BanChoiController : Controller
    {
        private readonly NhaHangThanhThaoContext _context;

        public BanChoiController(NhaHangThanhThaoContext context)
        {
            _context = context;
        }

        private bool LaQuanLy()
        {
            return HttpContext.Session.GetString("MaVaiTro") == "QL";
        }

        public async Task<IActionResult> Index()
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            var danhSachBan = await _context.BanChois
                .OrderBy(x => x.KhuVuc)
                .ThenBy(x => x.MaBan)
                .ToListAsync();

            return View(danhSachBan);
        }
        [HttpGet]
        public IActionResult Create()
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BanChoi banChoi)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(banChoi);
            }

            _context.BanChois.Add(banChoi);
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

            var banChoi = await _context.BanChois.FindAsync(id);

            if (banChoi == null)
            {
                return NotFound();
            }

            return View(banChoi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, BanChoi model)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.MaBan)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var banChoi = await _context.BanChois.FindAsync(id);

            if (banChoi == null)
            {
                return NotFound();
            }

            banChoi.TenBan = model.TenBan;
            banChoi.SucChua = model.SucChua;
            banChoi.KhuVuc = model.KhuVuc;
            banChoi.TrangThai = model.TrangThai;
            banChoi.Qrtoken = model.Qrtoken;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> InQr(string id)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var ban = await _context.BanChois.FindAsync(id);

            if (ban == null)
            {
                return NotFound();
            }

            string urlMenu = Url.Action(
                "Menu",
                "KhachGoiMon",
                new { token = ban.Qrtoken },
                Request.Scheme
            ) ?? "";

            var model = new BanChoiQrViewModel
            {
                MaBan = ban.MaBan,
                TenBan = ban.TenBan,
                QRToken = ban.Qrtoken,
                UrlMenu = urlMenu
            };

            return View(model);
        }

        public async Task<IActionResult> QrImage(string id)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var ban = await _context.BanChois.FindAsync(id);

            if (ban == null)
            {
                return NotFound();
            }

            string urlMenu = Url.Action(
                "Menu",
                "KhachGoiMon",
                new { token = ban.Qrtoken },
                Request.Scheme
            ) ?? "";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(urlMenu, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);

            byte[] qrCodeImage = qrCode.GetGraphic(20);

            return File(qrCodeImage, "image/png");
        }
    }
}