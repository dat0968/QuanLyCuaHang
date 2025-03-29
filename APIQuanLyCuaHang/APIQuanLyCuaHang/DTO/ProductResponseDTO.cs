using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.DTO
{
    public class ProductResponseDTO
    {
        public int MaSp { get; set; }
        public int MaDanhMuc { get; set; }
        public string? TenDanhMuc { get; set; }

        public string? TenSanPham { get; set; }
        public int TongSoLuong { get; set; }
        public string? KhoangGia { get; set; }

        public string? MoTa { get; set; }

        public bool? IsDelete { get; set; }
        public List<DetailProductResponseDTO> Chitietsanphams { get; set; } = new List<DetailProductResponseDTO>();
        public List<ProductResponseDTO> RelatedProducts { get; set; } = new List<ProductResponseDTO>();
    }
    public class ProductBestSellerDTO
    {
        public int MaCtsp { get; set; }
        public int MaSp { get; set; }
        public string TenSanPham { get; set; }
        public string KichThuoc { get; set; }
        public string HuongVi { get; set; }
        public decimal? DonGia { get; set; }
        public int TotalSold { get; set; }
        public string Hinh { get; set; }
        public List<ProductResponseDTO> RelatedProducts { get; set; } = new List<ProductResponseDTO>(); // Thêm trường này
    }
}
