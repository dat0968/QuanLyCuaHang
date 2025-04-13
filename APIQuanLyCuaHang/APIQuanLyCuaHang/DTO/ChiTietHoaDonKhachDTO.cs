using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO
{
    public class ChiTietHoaDonKhachDTO
    {
        public int MaHd { get; set; }

        public int MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; } = null!;
        public required string LoaiDoiTuong { get; set; }

        public int SoLuong { get; set; }
        public string? KichThuoc { get; set; }
        public string? HuongVi { get; set; }
        public decimal? DonGia { get; set; }

        public string? HinhAnh { get; set; }
        public string? MoTa { get; set; }
    }
}