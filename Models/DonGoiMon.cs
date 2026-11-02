using System;
using System.Collections.Generic;

namespace NhaHangThanhThao.Models;

public partial class DonGoiMon
{
    public string MaDonGoiMon { get; set; } = null!;

    public string MaBan { get; set; } = null!;

    public string? MaDonDatBan { get; set; }

    public int? MaNguoiTao { get; set; }

    public string NguonTao { get; set; } = null!;

    public DateTime ThoiGianTao { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual ICollection<ChiTietDonGoiMon> ChiTietDonGoiMons { get; set; } = new List<ChiTietDonGoiMon>();

    public virtual HoaDon? HoaDon { get; set; }

    public virtual BanChoi MaBanNavigation { get; set; } = null!;

    public virtual DonDatBan? MaDonDatBanNavigation { get; set; }

    public virtual NguoiDung? MaNguoiTaoNavigation { get; set; }
}
