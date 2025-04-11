namespace APIQuanLyCuaHang.DTO
{
    public class ChiTietHoaDonDTO
    {
        public string? TenSanPham { get; set; }
        public string? KichThuoc { get; set; }  
        public string? HuongVi { get; set; }   
        public decimal DonGia { get; set; }  
        public string? HinhAnh { get; set; } 
        public int SoLuong { get; set; }
        public int? MaCombo { get; set; }
        public string? TenCombo { get; set; }
        public decimal? GiamGia { get; set; }
        public decimal TienGoc { get; set; }
        public decimal TongTien { get; set; }
    }
}
