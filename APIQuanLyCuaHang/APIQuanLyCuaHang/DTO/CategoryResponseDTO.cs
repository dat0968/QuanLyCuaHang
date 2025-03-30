namespace APIQuanLyCuaHang.DTO
{
    public class CategoryResponseDTO
    {
        public int MaDanhMuc { get; set; }

        public string TenDanhMuc { get; set; }
        public string HinhDaiDien { get; set; }

        public bool? IsDelete { get; set; }
    }
}
