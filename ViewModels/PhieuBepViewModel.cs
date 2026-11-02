namespace NhaHangThanhThao.ViewModels
{
    public class PhieuBepViewModel
    {
        public string MaDonGoiMon { get; set; } = string.Empty;
        public string MaBan { get; set; } = string.Empty;
        public string TenBan { get; set; } = string.Empty;
        public DateTime ThoiGianTao { get; set; }
        public DateTime ThoiGianIn { get; set; } = DateTime.Now;
        public bool LaPhieuBoSung { get; set; }
        public bool DaChuyenBep { get; set; }

        public List<PhieuBepMonViewModel> DanhSachMon { get; set; } = new();
    }

    public class PhieuBepMonViewModel
    {
        public int MaChiTietDonGoiMon { get; set; }
        public string TenMon { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public string? GhiChu { get; set; }
    }
}
