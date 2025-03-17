using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Chucvu
{
    public int MaChucVu { get; set; }

    public string TenChucVu { get; set; } = null!;

    public decimal? LuongTheoGio { get; set; }

    public decimal? LuongCung { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Nhanvien> Nhanviens { get; set; } = new List<Nhanvien>();
}
