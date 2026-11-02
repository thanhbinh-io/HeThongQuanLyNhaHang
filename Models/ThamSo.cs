using System;
using System.Collections.Generic;

namespace NhaHangThanhThao.Models;

public partial class ThamSo
{
    public string TenThamSo { get; set; } = null!;

    public decimal GiaTri { get; set; }

    public string? MoTa { get; set; }
}
