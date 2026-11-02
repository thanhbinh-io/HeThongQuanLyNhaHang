using System;
using System.Collections.Generic;

namespace NhaHangThanhThao.Models;

public partial class ChiTietDonGoiMon
{
    public int MaChiTietDonGoiMon { get; set; }

    public string MaDonGoiMon { get; set; } = null!;

    public string MaMon { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public string? GhiChu { get; set; }

    public string TrangThai { get; set; } = null!;

    public decimal? ThanhTien { get; set; }

    public virtual DonGoiMon MaDonGoiMonNavigation { get; set; } = null!;

    public virtual MonAn MaMonNavigation { get; set; } = null!;
}
