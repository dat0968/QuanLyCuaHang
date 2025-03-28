using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Chitietcombo
{
    public int MaCombo { get; set; }
    public int MaSp { get; set; }

    public int? SoLuongSp { get; set; }

    public virtual Combo MaComboNavigation { get; set; } = null!;
    public virtual Sanpham MaSpNavigation { get; set; } = null!;

}
