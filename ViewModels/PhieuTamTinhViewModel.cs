using System.ComponentModel.DataAnnotations;

namespace NhaHangThanhThao.ViewModels
{
    public class PhieuTamTinhViewModel
    {
        public string MaDonGoiMon { get; set; } = string.Empty;
        public string TenBan { get; set; } = string.Empty;
        public DateTime GioVao { get; set; }

        public decimal TongTienMon { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Phụ thu không được âm")]
        public decimal PhuThu { get; set; }

        public decimal TienCocKhauTru { get; set; }

        public decimal TongTamTinh => TongTienMon + PhuThu - TienCocKhauTru;

        public List<PhieuTamTinhChiTietViewModel> ChiTietMons { get; set; } = new();
    }

    public class PhieuTamTinhChiTietViewModel
    {
        public string MaMon { get; set; } = string.Empty;
        public string TenMon { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
}