using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace NhaHangThanhThao.ViewModels
{
    public class DonDatBanCreateViewModel
    {
        public string MaDonDatBan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
        public string TenKhachHang { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn thời gian đến")]
        public DateTime ThoiGianDen { get; set; } = DateTime.Now.AddHours(1);

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng khách phải lớn hơn 0")]
        public int SoLuongKhach { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bàn/chòi")]
        public string MaBan { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Tiền cọc không được âm")]
        public decimal TienCoc { get; set; }

        public string? GhiChu { get; set; }
    }
}
