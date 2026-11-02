using Microsoft.AspNetCore.Mvc;

namespace NhaHangThanhThao.ViewModels
{
    public class DonGoiMonIndexViewModel
    {
        public string MaDonGoiMon { get; set; } = string.Empty;
        public string MaBan { get; set; } = string.Empty;
        public string TenBan { get; set; } = string.Empty;
        public DateTime ThoiGianTao { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public decimal TongTien { get; set; }
    }
}