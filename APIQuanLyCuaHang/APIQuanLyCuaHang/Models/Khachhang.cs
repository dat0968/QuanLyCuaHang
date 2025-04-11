using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Khachhang
{
    public int MaKh { get; set; }

    public string HoTen { get; set; } = null!;

    public string? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? DiaChi { get; set; }

    public string? Cccd { get; set; }

    public string? Sdt { get; set; }

    public string Email { get; set; } = null!;

    public string? TenTaiKhoan { get; set; }

    public string? MatKhau { get; set; }

    public string? HinhDaiDien { get; set; }

    public DateTime NgayTao { get; set; }

    public string? TinhTrang { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Giohang> Giohangs { get; set; } = new List<Giohang>();

    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();
    public virtual ICollection<ChitietmaCoupon> ChitietmaCoupons { get; set; } = new List<ChitietmaCoupon>();

}
