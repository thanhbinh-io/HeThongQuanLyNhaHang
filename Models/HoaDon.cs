using System;
using System.Collections.Generic;

namespace NhaHangThanhThao.Models;

public partial class HoaDon
{
    public string MaHoaDon { get; set; } = null!;

    public string MaDonGoiMon { get; set; } = null!;

    public DateTime GioVao { get; set; }

    public DateTime GioRa { get; set; }

    public decimal TongTienMon { get; set; }

    public decimal PhuThu { get; set; }

    public decimal TienCocKhauTru { get; set; }

    public decimal? TongThanhToan { get; set; }

    public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; } = new List<HoaDonChiTiet>();

    public virtual DonGoiMon MaDonGoiMonNavigation { get; set; } = null!;
}
