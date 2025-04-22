using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO
{
    public class ScheduleDTO
    {
        public int Id { get; set; }

        public int MaNv { get; set; }

        public string? TenCa { get; set; }

        public string? TenNhanVien { get; set; }

        public int MaCaKip { get; set; }

        public DateOnly NgayThangNam { get; set; }

        public double SoGioLam { get; set; }

        public string? LyDoNghi { get; set; }

        public decimal? TongLuong { get; set; }
        public TimeOnly? GioVao { get; set; } // Giờ nhân viên bắt đầu làm
        public TimeOnly? GioRa { get; set; } // Giờ nhân viên kết thúc ca làm
        public string? TrangThai { get; set; } // Trạng thái làm việc
        public string? GhiChu { get; set; } // Ghi chú đặc biệt
        public bool? IsDelete { get; set; }

    }
}