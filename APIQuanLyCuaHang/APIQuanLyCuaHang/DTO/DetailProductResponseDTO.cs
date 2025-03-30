using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.DTO
{
    public class DetailProductResponseDTO
    {
        public int MaCtsp { get; set; }
        public int MaSp { get; set; }
        public string? TenSanPham { get; set; }
        public string? KichThuoc { get; set; }

        public string? HuongVi { get; set; }

        public int? SoLuongTon { get; set; }
        public string? AnhDaiDien { get; set; }

        public decimal? DonGia { get; set; }
        public List<ImageProductResponseDTO> Hinhanhs { get; set; } = new List<ImageProductResponseDTO>();
    }
}
