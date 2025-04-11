using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Chitietsanpham
{
    public int MaCtsp { get; set; }

    public int MaSp { get; set; }

    public string? KichThuoc { get; set; }

    public string? HuongVi { get; set; }

    public int? SoLuongTon { get; set; }

    public decimal? DonGia { get; set; }
    public bool IsDelete { get; set; }

    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    public virtual ICollection<Giohang> Giohangs { get; set; } = new List<Giohang>();

    public virtual ICollection<Hinhanh> Hinhanhs { get; set; } = new List<Hinhanh>();
    public virtual ICollection<GioHangCTCombo> GioHangCTCombos { get; set; } = new List<GioHangCTCombo>();

    public virtual Sanpham MaSpNavigation { get; set; } = null!;
    public virtual ICollection<Chitietcombohoadon> Chitietcombohoadons { get; set; } = new List<Chitietcombohoadon>();

}
