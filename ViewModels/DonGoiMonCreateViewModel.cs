using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NhaHangThanhThao.ViewModels
{
    public class DonGoiMonCreateViewModel
    {
        public string? MaDonGoiMon { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bàn/chòi")]
        public string MaBan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn món")]
        public string MaMon { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; } = 1;

        public string? GhiChu { get; set; }
    }
}