namespace NhaHangThanhThao.ViewModels
{
    public class BaoCaoDoanhThuViewModel
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }

        public int TongSoHoaDon { get; set; }
        public decimal TongDoanhThu { get; set; }

        public List<BaoCaoDoanhThuChiTietViewModel> DanhSachHoaDon { get; set; } = new();
    }

    public class BaoCaoDoanhThuChiTietViewModel
    {
        public string MaHoaDon { get; set; } = string.Empty;
        public string MaDonGoiMon { get; set; } = string.Empty;
        public string TenBan { get; set; } = string.Empty;

        public DateTime GioRa { get; set; }

        public decimal TongTienMon { get; set; }
        public decimal PhuThu { get; set; }
        public decimal TienCocKhauTru { get; set; }
        public decimal TongThanhToan { get; set; }
    }
}