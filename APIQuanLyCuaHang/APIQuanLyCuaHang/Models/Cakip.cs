using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIQuanLyCuaHang.Models;

public partial class Cakip
{
    public int MaCaKip { get; set; }

    public string? TenCa { get; set; } = null!; // Tên ca làm việc

    public int SoNguoiToiDa { get; set; }

    public int SoNguoiHienTai { get; set; }
    [Column(TypeName = "time(7)")]

    public TimeOnly GioBatDau { get; set; }
    [Column(TypeName = "time(7)")]

    public TimeOnly GioKetThuc { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal HeSoLuong { get; set; } = 1;// Hệ số lương của ca làm việc

    public bool? IsDelete { get; set; }

    public virtual ICollection<Lichsulamviec> Lichsulamviecs { get; set; } = new List<Lichsulamviec>();
}
