using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Controllers
{
    public class PhucVuController : Controller
    {
        private readonly NhaHangThanhThaoContext _context;

        public PhucVuController(NhaHangThanhThaoContext context)
        {
            _context = context;
        }

        private bool DuocTruyCap()
        {
            var maVaiTro = HttpContext.Session.GetString("MaVaiTro");
            return maVaiTro == "NV" || maVaiTro == "QL";
        }

        public async Task<IActionResult> Index()
        {
            if (!DuocTruyCap())
            {
                return RedirectToAction("Login", "Account");
            }

            var danhSachBan = await _context.BanChois
                .OrderBy(x => x.KhuVuc)
                .ThenBy(x => x.MaBan)
                .ToListAsync();

            var donDangPhucVu = await _context.DonGoiMons
                .Where(x => x.TrangThai == "DangPhucVu")
                .ToListAsync();

            var model = danhSachBan.Select(ban =>
            {
                var don = donDangPhucVu.FirstOrDefault(x => x.MaBan == ban.MaBan);

                return new PhucVuBanViewModel
                {
                    MaBan = ban.MaBan,
                    TenBan = ban.TenBan,
                    KhuVuc = ban.KhuVuc,
                    SucChua = ban.SucChua,
                    TrangThai = ban.TrangThai,
                    MaDonGoiMon = don?.MaDonGoiMon
                };
            }).ToList();

            return View(model);
        }
    }
}