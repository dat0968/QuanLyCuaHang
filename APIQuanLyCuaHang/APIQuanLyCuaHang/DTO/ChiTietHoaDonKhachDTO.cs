using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO
{
    public class ChiTietHoaDonKhachDTO
    {
        public int MaHd { get; set; }

        public int MaCtsp { get; set; }

        public int SoLuong { get; set; }
        public string? KichThuoc { get; set; }
        public string? HuongVi { get; set; }
        public decimal? DonGia { get; set; }
        public string TenSanPham { get; set; } = null!;
        public string? MoTa { get; set; }
    }
}