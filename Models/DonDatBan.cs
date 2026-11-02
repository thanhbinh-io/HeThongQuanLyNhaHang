using System;
using System.Collections.Generic;

namespace NhaHangThanhThao.Models;

public partial class DonDatBan
{
    public string MaDonDatBan { get; set; } = null!;

    public string TenKhachHang { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public DateTime NgayLap { get; set; }

    public DateTime ThoiGianDen { get; set; }

    public int SoLuongKhach { get; set; }

    public int MaNguoiLap { get; set; }

    public string? GhiChu { get; set; }

    public virtual ICollection<ChiTietDonDatBan> ChiTietDonDatBans { get; set; } = new List<ChiTietDonDatBan>();

    public virtual ICollection<DonGoiMon> DonGoiMons { get; set; } = new List<DonGoiMon>();

    public virtual NguoiDung MaNguoiLapNavigation { get; set; } = null!;
}
