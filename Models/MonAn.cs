using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NhaHangThanhThao.Models;

public partial class MonAn
{
    [Required(ErrorMessage = "Vui lòng nhập mã món")]
    public string MaMon { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập tên món")]
    public string TenMon { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập đơn vị tính")]
    public string DonViTinh { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn danh mục")]
    public string MaDanhMuc { get; set; } = null!;

    [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không được âm")]
    public decimal DonGia { get; set; }

    public string? HinhAnh { get; set; }

    public string? MoTa { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
    public string TrangThai { get; set; } = null!;

    public virtual ICollection<ChiTietDonGoiMon> ChiTietDonGoiMons { get; set; } = new List<ChiTietDonGoiMon>();

    public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; } = new List<HoaDonChiTiet>();

    public virtual DanhMucMon MaDanhMucNavigation { get; set; } = null!;
}
