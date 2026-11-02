using System;
using System.Collections.Generic;

namespace NhaHangThanhThao.Models;

public partial class HoaDonChiTiet
{
    public string MaHoaDon { get; set; } = null!;

    public string MaMon { get; set; } = null!;

    public string TenMon { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal? ThanhTien { get; set; }

    public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;

    public virtual MonAn MaMonNavigation { get; set; } = null!;
}
