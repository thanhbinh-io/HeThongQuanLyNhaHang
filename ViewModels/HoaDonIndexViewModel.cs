namespace NhaHangThanhThao.ViewModels
{
    public class HoaDonIndexViewModel
    {
        public string MaHoaDon { get; set; } = string.Empty;
        public string MaDonGoiMon { get; set; } = string.Empty;
        public string TenBan { get; set; } = string.Empty;

        public DateTime GioVao { get; set; }
        public DateTime GioRa { get; set; }

        public decimal TongTienMon { get; set; }
        public decimal PhuThu { get; set; }
        public decimal TienCocKhauTru { get; set; }
        public decimal TongThanhToan { get; set; }
    }
}