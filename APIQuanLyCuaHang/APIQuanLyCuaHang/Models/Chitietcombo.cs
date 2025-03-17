using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Chitietcombo
{
    public int MaCombo { get; set; }

    public int MaCtsp { get; set; }

    public int? SoLuongSp { get; set; }

    public virtual Combo MaComboNavigation { get; set; } = null!;

    public virtual Chitietsanpham MaCtspNavigation { get; set; } = null!;
}
