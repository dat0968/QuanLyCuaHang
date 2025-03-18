using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.ViewModels.DataChart
{
    public class StaffDC
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
        public decimal? DoanhThuMangLai { get; set; }
    }
}