namespace APIQuanLyCuaHang.DTO
{
    public class ProductEditRequestDTO
    {
        public int MaDanhMuc { get; set; }
        public string? TenSanPham { get; set; }

        public string? MoTa { get; set; }

        public bool? IsDelete { get; set; }
        public List<DetailProductEditRequestDTO> DetailProductEditRequestDTOs { get; set; } = new List<DetailProductEditRequestDTO>();
    }
    public class DetailProductEditRequestDTO
    {
        public int MaCtsp { get; set; }
        public string? KichThuoc { get; set; }

        public string? HuongVi { get; set; }

        public int? SoLuongTon { get; set; }

        public decimal? DonGia { get; set; }
        public List<ImageProductRequestDTO> ImageProductRequestDTOs { get; set; } = new List<ImageProductRequestDTO>();
    }
}
