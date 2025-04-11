using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO
{
    public class HoaDonKhachDTO
    {
        public int MaHd { get; set; }
        public int MaKh { get; set; }
        public int? MaNv { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? BatDauGiao { get; set; }
        public DateTime? NgayNhan { get; set; }
        public string? DiaChiNhanHang { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public string? HinhThucTt { get; set; }
        public string? TinhTrang { get; set; }
        public string? MoTa { get; set; }
        public string HoTen { get; set; } = null!;
        public string Sdt { get; set; } = null!;
        public string? LyDoHuy { get; set; }
        public bool? IsDelete { get; set; }
        public decimal PhiVanChuyen { get; set; }
        public decimal TienGoc { get; set; }
        public decimal GiamGiaCoupon { get; set; } = 0;
        public decimal TongTien { get; set; }
        public List<ChiTietHoaDonKhachDTO>? ChiTietHoaDonKhachs { get; set; }
    }
}