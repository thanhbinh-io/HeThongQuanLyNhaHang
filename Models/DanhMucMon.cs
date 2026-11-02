using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NhaHangThanhThao.Models;

public partial class DanhMucMon
{
    [Required(ErrorMessage = "Vui lòng nhập mã danh mục")]
    public string MaDanhMuc { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
    public string TenDanhMuc { get; set; } = null!;

    public virtual ICollection<MonAn> MonAns { get; set; } = new List<MonAn>();
}
