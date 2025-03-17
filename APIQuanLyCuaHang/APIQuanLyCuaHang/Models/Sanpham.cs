using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Sanpham
{
    public int MaSp { get; set; }

    public int MaDanhMuc { get; set; }

    public string TenSanPham { get; set; } = null!;

    public string? MoTa { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Chitietsanpham> Chitietsanphams { get; set; } = new List<Chitietsanpham>();

    public virtual Danhmuc MaDanhMucNavigation { get; set; } = null!;
}
