﻿using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Hoadon
{
    public int MaHd { get; set; }

    public int? MaKh { get; set; }

    public int? MaNv { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? BatDauGiao { get; set; }

    public DateTime? NgayNhan { get; set; }

    public string? DiaChiNhanHang { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public string? HinhThucTt { get; set; }

    public string? TinhTrang { get; set; }

    public string? MoTa { get; set; }

    public string? HoTen { get; set; } = null!;

    public string? Sdt { get; set; }

    public string? LyDoHuy { get; set; }

    public bool? IsDelete { get; set; }

    public decimal PhiVanChuyen { get; set; }

    public decimal TienGoc { get; set; }
    public string? MaCoupon { get; set; }
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    public virtual Khachhang? MaKhNavigation { get; set; }

    public virtual Nhanvien? MaNvNavigation { get; set; }
    public virtual MaCoupon? MaCouponNavigation { get; set; }
    public virtual ICollection<Chitietcombohoadon> Chitietcombohoadons { get; set; } = new List<Chitietcombohoadon>();
}
