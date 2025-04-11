using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Combo
{
    public int MaCombo { get; set; }

    public string TenCombo { get; set; } = null!;

    public string? Hinh { get; set; }
    public int SoLuong { get; set; } = 1;
    public float? PhanTramGiam { get; set; }
    public decimal? SoTienGiam { get; set; }

    public string? MoTa { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Chitietcombo> Chitietcombos { get; set; } = new List<Chitietcombo>();
    public virtual ICollection<Giohang> Giohangs { get; set; } = new List<Giohang>();
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();
    public virtual ICollection<Chitietcombohoadon> Chitietcombohoadons { get; set; } = new List<Chitietcombohoadon>();

}
