namespace APIQuanLyCuaHang.DTO
{
    public class ProductCreateRequestDTO
    {
        public int MaDanhMuc { get; set; }
        public string? TenSanPham { get; set; }

        public string? MoTa { get; set; }

        public bool? IsDelete { get; set; }
        public List<DetailProductCreateRequestDTO> DetailProductCreateRequestDTOs { get; set; } = new List<DetailProductCreateRequestDTO>();
    }
    public class DetailProductCreateRequestDTO
    {
        public string? KichThuoc { get; set; }

        public string? HuongVi { get; set; }

        public int? SoLuongTon { get; set; }

        public decimal? DonGia { get; set; }
        public List<ImageProductRequestDTO> ImageProductRequestDTOs { get; set; } = new List<ImageProductRequestDTO>();
    }
    public class ImageProductRequestDTO
    {
        public string? TenHinhAnh { get; set; }
    }
}
