using System;
using System.Collections.Generic;

namespace NhaHangThanhThao.Models;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }

    public string HoTen { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string MaVaiTro { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<DonDatBan> DonDatBans { get; set; } = new List<DonDatBan>();

    public virtual ICollection<DonGoiMon> DonGoiMons { get; set; } = new List<DonGoiMon>();

    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;
}
