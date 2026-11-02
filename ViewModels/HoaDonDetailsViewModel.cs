namespace NhaHangThanhThao.ViewModels
{
    public class HoaDonDetailsViewModel
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

        public List<HoaDonChiTietViewModel> ChiTietMons { get; set; } = new();
    }

    public class HoaDonChiTietViewModel
    {
        public string MaMon { get; set; } = string.Empty;
        public string TenMon { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}