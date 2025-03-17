using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Lichsulamviec
{
    public int Id { get; set; }

    public int MaNv { get; set; }

    public int MaCaKip { get; set; }

    public DateOnly NgayThangNam { get; set; }

    public double SoGioLam { get; set; }

    public string? LyDoNghi { get; set; }

    public decimal? TongLuong { get; set; }

    public bool? IsDelete { get; set; }

    public virtual Cakip MaCaKipNavigation { get; set; } = null!;

    public virtual Nhanvien MaNvNavigation { get; set; } = null!;
}
