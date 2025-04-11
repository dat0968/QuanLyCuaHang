using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIQuanLyCuaHang.Models;

public partial class Cthoadon
{
    public int Id { get; set; }
    public int MaHd { get; set; }

    public int? MaCtsp { get; set; }

    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal? GiamGia { get; set; }
    public int? MaCombo { get; set; }
    [ForeignKey("MaCombo")]
    public virtual Combo? Combo { get; set; }
    public virtual Chitietsanpham? MaCtspNavigation { get; set; } = null!;

    public virtual Hoadon MaHdNavigation { get; set; } = null!;
}
