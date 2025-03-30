using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.DTO
{
    public class OrderRequestDTO
    {
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

        public string HoTen { get; set; }

        public string Sdt { get; set; } 

        public string? LyDoHuy { get; set; }

        public bool? IsDelete { get; set; }

        public decimal PhiVanChuyen { get; set; }

        public decimal TienGoc { get; set; }

        public List<OrderDetailRequestDTO> Cthoadons { get; set; } = new List<OrderDetailRequestDTO>();
    }
    public class OrderDetailRequestDTO
    {
        public int MaCtsp { get; set; }

        public int SoLuong { get; set; }
    }
}
