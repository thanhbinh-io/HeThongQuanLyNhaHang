using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly NhaHangThanhThaoContext _context;

        public BaoCaoController(NhaHangThanhThaoContext context)
        {
            _context = context;
        }

        private bool LaQuanLy()
        {
            return HttpContext.Session.GetString("MaVaiTro") == "QL";
        }

        private async Task<BaoCaoDoanhThuViewModel> TaoBaoCaoDoanhThuAsync(DateTime? tuNgay, DateTime? denNgay)
        {
            var query =
                from hd in _context.HoaDons
                join dgm in _context.DonGoiMons
                    on hd.MaDonGoiMon equals dgm.MaDonGoiMon
                join ban in _context.BanChois
                    on dgm.MaBan equals ban.MaBan
                select new
                {
                    hd.MaHoaDon,
                    hd.MaDonGoiMon,
                    ban.TenBan,
                    hd.GioRa,
                    hd.TongTienMon,
                    hd.PhuThu,
                    hd.TienCocKhauTru,
                    hd.TongThanhToan
                };

            if (tuNgay.HasValue)
            {
                DateTime ngayBatDau = tuNgay.Value.Date;
                query = query.Where(x => x.GioRa >= ngayBatDau);
            }

            if (denNgay.HasValue)
            {
                DateTime ngayKetThuc = denNgay.Value.Date.AddDays(1);
                query = query.Where(x => x.GioRa < ngayKetThuc);
            }

            var danhSach = await query
                .OrderByDescending(x => x.GioRa)
                .Select(x => new BaoCaoDoanhThuChiTietViewModel
                {
                    MaHoaDon = x.MaHoaDon,
                    MaDonGoiMon = x.MaDonGoiMon,
                    TenBan = x.TenBan,
                    GioRa = x.GioRa,
                    TongTienMon = x.TongTienMon,
                    PhuThu = x.PhuThu,
                    TienCocKhauTru = x.TienCocKhauTru,
                    TongThanhToan = x.TongThanhToan ?? 0
                })
                .ToListAsync();

            return new BaoCaoDoanhThuViewModel
            {
                TuNgay = tuNgay,
                DenNgay = denNgay,
                TongSoHoaDon = danhSach.Count,
                TongDoanhThu = danhSach.Sum(x => x.TongThanhToan),
                DanhSachHoaDon = danhSach
            };
        }

        public async Task<IActionResult> DoanhThu(DateTime? tuNgay, DateTime? denNgay)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            var model = await TaoBaoCaoDoanhThuAsync(tuNgay, denNgay);

            return View(model);
        }

        public async Task<IActionResult> XuatExcel(DateTime? tuNgay, DateTime? denNgay)
        {
            if (!LaQuanLy())
            {
                return RedirectToAction("Login", "Account");
            }

            var model = await TaoBaoCaoDoanhThuAsync(tuNgay, denNgay);
            var html = new StringBuilder();

            html.AppendLine("<html><head><meta charset=\"utf-8\" /></head><body>");
            html.AppendLine("<table border=\"1\">");
            html.AppendLine("<tr>");
            html.AppendLine("<th>Mã hóa đơn</th>");
            html.AppendLine("<th>Mã đơn gọi món</th>");
            html.AppendLine("<th>Bàn/chòi</th>");
            html.AppendLine("<th>Thời gian thanh toán</th>");
            html.AppendLine("<th>Tiền món</th>");
            html.AppendLine("<th>Phụ thu</th>");
            html.AppendLine("<th>Cọc trừ</th>");
            html.AppendLine("<th>Tổng thanh toán</th>");
            html.AppendLine("</tr>");

            foreach (var item in model.DanhSachHoaDon)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{WebUtility.HtmlEncode(item.MaHoaDon)}</td>");
                html.AppendLine($"<td>{WebUtility.HtmlEncode(item.MaDonGoiMon)}</td>");
                html.AppendLine($"<td>{WebUtility.HtmlEncode(item.TenBan)}</td>");
                html.AppendLine($"<td>{item.GioRa:dd/MM/yyyy HH:mm}</td>");
                html.AppendLine($"<td>{item.TongTienMon:N0}</td>");
                html.AppendLine($"<td>{item.PhuThu:N0}</td>");
                html.AppendLine($"<td>{item.TienCocKhauTru:N0}</td>");
                html.AppendLine($"<td>{item.TongThanhToan:N0}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("<tr>");
            html.AppendLine("<td colspan=\"7\"><strong>Tổng doanh thu</strong></td>");
            html.AppendLine($"<td><strong>{model.TongDoanhThu:N0}</strong></td>");
            html.AppendLine("</tr>");
            html.AppendLine("</table>");
            html.AppendLine("</body></html>");

            string fileName = $"bao-cao-doanh-thu-{DateTime.Now:yyyyMMddHHmmss}.xls";
            byte[] preamble = Encoding.UTF8.GetPreamble();
            byte[] contentBytes = Encoding.UTF8.GetBytes(html.ToString());
            byte[] bytes = new byte[preamble.Length + contentBytes.Length];
            Buffer.BlockCopy(preamble, 0, bytes, 0, preamble.Length);
            Buffer.BlockCopy(contentBytes, 0, bytes, preamble.Length, contentBytes.Length);

            return File(bytes, "application/vnd.ms-excel; charset=utf-8", fileName);
        }
    }
}
