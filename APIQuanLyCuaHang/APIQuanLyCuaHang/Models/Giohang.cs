using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Giohang
{
    public int Id { get; set; }

    public int MaKh { get; set; }

    public int MaCtsp { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public virtual Chitietsanpham MaCtspNavigation { get; set; } = null!;

    public virtual Khachhang MaKhNavigation { get; set; } = null!;
}
