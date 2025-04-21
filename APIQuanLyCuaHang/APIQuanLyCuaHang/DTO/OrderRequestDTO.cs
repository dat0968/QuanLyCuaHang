using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.DTO
{
    public class OrderRequestDTO
    {
        public int? MaKh { get; set; }
        public int? MaNv { get; set; }

        public string? DiaChiNhanHang { get; set; }

        public string? HinhThucTt { get; set; }

        public string? MoTa { get; set; }

        public string HoTen { get; set; }

        public string Sdt { get; set; } 

        public decimal PhiVanChuyen { get; set; }

        public decimal TienGoc { get; set; }
        public string? MaCoupon { get; set; }
        public virtual List<DetailCombo_OrderResquest> DetailCombo_OrderResquests { get; set; } = new List<DetailCombo_OrderResquest>();
        public List<OrderDetailRequestDTO> Cthoadons { get; set; } = new List<OrderDetailRequestDTO>();
    }
    public class OrderDetailRequestDTO
    {
        public int? MaCtsp { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal? GiamGia { get; set; }
        public int? MaCombo { get; set; }
    }
    public class DetailCombo_OrderResquest
    {
        public int MaCombo { get; set; }
        public int MaCTSp { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }
}
