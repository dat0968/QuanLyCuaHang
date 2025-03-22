using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Cakip
{
    public int MaCaKip { get; set; }

    public int SoNguoiToiDa { get; set; }

    public int SoNguoiHienTai { get; set; }

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Lichsulamviec> Lichsulamviecs { get; set; } = new List<Lichsulamviec>();
}
