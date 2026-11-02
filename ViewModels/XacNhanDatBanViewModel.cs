using System.ComponentModel.DataAnnotations;

namespace NhaHangThanhThao.ViewModels
{
    public class XacNhanDatBanViewModel
    {
        public string QRToken { get; set; } = string.Empty;

        public string MaBan { get; set; } = string.Empty;
         
        public string TenBan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại đã đặt bàn")]
        public string SoDienThoai { get; set; } = string.Empty;
    }
}