namespace NhaHangThanhThao.ViewModels
{
    public class KhachGoiMonViewModel
    {
        public string QRToken { get; set; } = string.Empty;
        public string MaBan { get; set; } = string.Empty;
        public string TenBan { get; set; } = string.Empty;
        public string TrangThaiBan { get; set; } = string.Empty;

        public string? MaDanhMucDangChon { get; set; }

        public List<KhachGoiMonDanhMucViewModel> DanhSachDanhMuc { get; set; } = new();

        public List<KhachGoiMonMonAnViewModel> DanhSachMon { get; set; } = new();

        public List<KhachGoiMonMonDaGoiViewModel> DanhSachMonDaGoi { get; set; } = new();
    }

    public class KhachGoiMonDanhMucViewModel
    {
        public string MaDanhMuc { get; set; } = string.Empty;
        public string TenDanhMuc { get; set; } = string.Empty;
    }

    public class KhachGoiMonMonAnViewModel
    {
        public string MaMon { get; set; } = string.Empty;
        public string TenMon { get; set; } = string.Empty;
        public string TenDanhMuc { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public string? MoTa { get; set; }
        public string? HinhAnh { get; set; }
    }

    public class KhachGoiMonMonDaGoiViewModel
    {
        public string TenMon { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
    }
}
