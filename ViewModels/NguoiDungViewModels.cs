using System.ComponentModel.DataAnnotations;

namespace NhaHangThanhThao.ViewModels
{
    public class NguoiDungIndexViewModel
    {
        public int MaNguoiDung { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string TenDangNhap { get; set; } = string.Empty;
        public string MaVaiTro { get; set; } = string.Empty;
        public string TenVaiTro { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public bool LaTaiKhoanHienTai { get; set; }
    }

    public class NguoiDungCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(150, ErrorMessage = "Họ tên tối đa 150 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(100, ErrorMessage = "Tên đăng nhập tối đa 100 ký tự")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Mật khẩu từ 3 đến 255 ký tự")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        public string MaVaiTro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public string TrangThai { get; set; } = "HoatDong";
    }

    public class NguoiDungEditViewModel
    {
        public int MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(150, ErrorMessage = "Họ tên tối đa 150 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(100, ErrorMessage = "Tên đăng nhập tối đa 100 ký tự")]
        public string TenDangNhap { get; set; } = string.Empty;

        [StringLength(255, MinimumLength = 3, ErrorMessage = "Mật khẩu từ 3 đến 255 ký tự")]
        public string? MatKhauMoi { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        public string MaVaiTro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public string TrangThai { get; set; } = "HoatDong";

        public bool LaTaiKhoanHienTai { get; set; }
    }
}
