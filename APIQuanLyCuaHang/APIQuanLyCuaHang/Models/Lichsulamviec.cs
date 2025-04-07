using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIQuanLyCuaHang.Models;

public partial class Lichsulamviec
{
    public int Id { get; set; }

    public int MaNv { get; set; }

    public int MaCaKip { get; set; }

    public DateOnly NgayThangNam { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeOnly? GioVao { get; set; } // Giờ nhân viên bắt đầu làm

    [Column(TypeName = "time(7)")]
    public TimeOnly? GioRa { get; set; } // Giờ nhân viên kết thúc ca làm

    public double SoGioLam { get; set; }

    public string? LyDoNghi { get; set; }

    [RegularExpression("Chờ xác nhận|Đi làm|Kết thúc ca|Nghỉ phép|Trễ|Nghỉ không phép|Không được xác nhận", ErrorMessage = "Trạng thái không hợp lệ.")]
    public string? TrangThai { get; set; } // Trạng thái làm việc

    public string? GhiChu { get; set; } // Ghi chú đặc biệt

    public int? NguoiXacNhan { get; set; } // Người xác nhận lịch làm việc

    public decimal? TongLuong { get; set; }

    public bool? IsDelete { get; set; }

    public virtual Cakip MaCaKipNavigation { get; set; } = null!;

    public virtual Nhanvien MaNvNavigation { get; set; } = null!;

    public virtual Nhanvien? NguoiXacNhanLich { get; set; } // Người xác nhận lịch làm việc
}
