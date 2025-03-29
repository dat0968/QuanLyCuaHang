using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.DTO
{
    public class ComboResponseDTO
    {
        public int MaCombo { get; set; }
        public string TenCombo { get; set; }
        public string? Hinh { get; set; }
        public decimal? SoTienGiam { get; set; }
        public float? PhanTramGiam { get; set; }
        public int SoLuong { get; set; }
        public string? MoTa { get; set; }
        public bool? IsDelete { get; set; }
        public int SoLuongMiengGa { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public List<DetaisComboResponseDTO> Chitietcombos { get; set; } = new List<DetaisComboResponseDTO>();
        public List<ProductResponseDTO> RelatedProducts { get; set; } = new List<ProductResponseDTO>(); // Thêm trường này
    }
    public class DetaisComboResponseDTO
    {
        public int MaSp { get; set; }
        public string TenSp { get; set; }
        public int? SoLuongSp { get; set; }
        public List<DetailProductResponseDTO> Variants { get; set; } = new List<DetailProductResponseDTO>(); // Thêm trường này
    }
}
