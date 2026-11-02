using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.Models;

namespace NhaHangThanhThao.Controllers
{
    public class DanhMucMonController : Controller
    {
        private readonly NhaHangThanhThaoContext _context;

        public DanhMucMonController(NhaHangThanhThaoContext context)
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

            var danhMucMons = await _context.DanhMucMons
                .OrderBy(x => x.MaDanhMuc)
                .ToListAsync();

            return View(danhMucMons);
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
        public async Task<IActionResult> Create(DanhMucMon danhMucMon)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(danhMucMon);
            }

            _context.DanhMucMons.Add(danhMucMon);
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

            var danhMucMon = await _context.DanhMucMons.FindAsync(id);

            if (danhMucMon == null)
            {
                return NotFound();
            }

            return View(danhMucMon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, DanhMucMon model)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.MaDanhMuc)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var danhMucMon = await _context.DanhMucMons.FindAsync(id);

            if (danhMucMon == null)
            {
                return NotFound();
            }

            danhMucMon.TenDanhMuc = model.TenDanhMuc;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}