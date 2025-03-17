using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Hinhanh
{
    public int MaHinhAnh { get; set; }

    public int MaCtsp { get; set; }

    public string? TenHinhAnh { get; set; }

    public virtual Chitietsanpham MaCtspNavigation { get; set; } = null!;
}
