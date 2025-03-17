using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Cthoadon
{
    public int MaHd { get; set; }

    public int MaCtsp { get; set; }

    public int SoLuong { get; set; }

    public virtual Chitietsanpham MaCtspNavigation { get; set; } = null!;

    public virtual Hoadon MaHdNavigation { get; set; } = null!;
}
