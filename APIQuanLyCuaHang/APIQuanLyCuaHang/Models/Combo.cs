using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Combo
{
    public int MaCombo { get; set; }

    public string TenCombo { get; set; } = null!;

    public string? Hinh { get; set; }

    public decimal? GiaCombo { get; set; }

    public string? MoTa { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Chitietcombo> Chitietcombos { get; set; } = new List<Chitietcombo>();
}
