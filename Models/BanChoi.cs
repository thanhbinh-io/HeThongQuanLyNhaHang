using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NhaHangThanhThao.Models;

public partial class BanChoi
{
    [Required(ErrorMessage = "Vui lòng nhập mã bàn/chòi")]
    public string MaBan { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập tên bàn/chòi")]
    public string TenBan { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải lớn hơn 0")]
    public int SucChua { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn khu vực")]
    public string KhuVuc { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
    public string TrangThai { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập QR Token")]
    public string Qrtoken { get; set; } = null!;

    public virtual ICollection<ChiTietDonDatBan> ChiTietDonDatBans { get; set; } = new List<ChiTietDonDatBan>();

    public virtual ICollection<DonGoiMon> DonGoiMons { get; set; } = new List<DonGoiMon>();
}
