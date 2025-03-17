using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Danhmuc
{
    public int MaDanhMuc { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public bool? IsDelete { get; set; }

    public virtual ICollection<Sanpham> Sanphams { get; set; } = new List<Sanpham>();
}
