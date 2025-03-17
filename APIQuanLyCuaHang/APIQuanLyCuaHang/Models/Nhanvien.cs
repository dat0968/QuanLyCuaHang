using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Nhanvien
{
    public int MaNv { get; set; }

    public string HoTen { get; set; } = null!;

    public string GioiTinh { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? DiaChi { get; set; }

    public string? Cccd { get; set; }

    public string Sdt { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly NgayVaoLam { get; set; }

    public string? TenTaiKhoan { get; set; }

    public string MatKhau { get; set; } = null!;

    public string? TinhTrang { get; set; }

    public bool? IsDelete { get; set; }

    public int? MaChucVu { get; set; }

    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    public virtual ICollection<Lichsulamviec> Lichsulamviecs { get; set; } = new List<Lichsulamviec>();

    public virtual Chucvu? MaChucVuNavigation { get; set; }
}
