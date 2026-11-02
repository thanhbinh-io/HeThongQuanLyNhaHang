namespace NhaHangThanhThao.ViewModels
{
    public class PhieuDatBanViewModel
    {
        public string MaDonDatBan { get; set; } = string.Empty;
        public string TenKhachHang { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public DateTime NgayLap { get; set; }
        public DateTime ThoiGianDen { get; set; }
        public int SoLuongKhach { get; set; }
        public string MaBan { get; set; } = string.Empty;
        public string TenBan { get; set; } = string.Empty;
        public string KhuVuc { get; set; } = string.Empty;
        public decimal TienCoc { get; set; }
        public string? GhiChu { get; set; }
    }
}
