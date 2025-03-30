namespace APIQuanLyCuaHang.DTO
{
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
