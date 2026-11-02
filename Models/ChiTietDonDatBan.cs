using System;
using System.Collections.Generic;

namespace NhaHangThanhThao.Models;

public partial class ChiTietDonDatBan
{
    public string MaDonDatBan { get; set; } = null!;

    public string MaBan { get; set; } = null!;

    public decimal TienCoc { get; set; }

    public string? GhiChu { get; set; }

    public virtual BanChoi MaBanNavigation { get; set; } = null!;

    public virtual DonDatBan MaDonDatBanNavigation { get; set; } = null!;
}
