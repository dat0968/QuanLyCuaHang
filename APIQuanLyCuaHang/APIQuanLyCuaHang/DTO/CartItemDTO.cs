namespace APIQuanLyCuaHang.DTO
{
    public class CartItemDTO
    {
        public int MaKh { get; set; }
        public int? MaCtsp { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public string TenSanPham { get; set; }
        public string HoTenKhachHang { get; set; }
        public int? SoLuongTon { get; set; } 
        public List<string> HinhAnhUrls { get; set; }
        public string KichThuoc { get; set; }
        public string HuongVi { get; set; }
        public int? MaCombo { get; set; } 
        public string? TenCombo { get; set; }
    }
}