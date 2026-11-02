namespace NhaHangThanhThao.ViewModels
{
    public class DonGoiMonDetailsViewModel
    {
        public string MaDonGoiMon { get; set; } = string.Empty;
        public string MaBan { get; set; } = string.Empty;
        public string TenBan { get; set; } = string.Empty;
        public DateTime ThoiGianTao { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public decimal TongTien { get; set; }

        public List<ChiTietDonGoiMonViewModel> ChiTietMons { get; set; } = new();
    }

    public class ChiTietDonGoiMonViewModel
    {
        public int MaChiTietDonGoiMon { get; set; }
        public string MaMon { get; set; } = string.Empty;
        public string TenMon { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
    }
}