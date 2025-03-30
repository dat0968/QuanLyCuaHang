namespace APIQuanLyCuaHang.DTO
{
    public class KhachhangDTO
    {
        public int MaKh { get; set; }
        public string HoTen { get; set; } = null!;
        public string GioiTinh { get; set; } = null!;
        public DateOnly? NgaySinh { get; set; }
        public string DiaChi { get; set; } = null!;
        public string Cccd { get; set; } = null!;
        public string Sdt { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string TenTaiKhoan { get; set; } = null!;
        public string? MatKhau { get; set; } 
        public string? HinhDaiDien { get; set; } 
        public IFormFile? Anh { get; set; } 
        public DateTime NgayTao { get; set; }
        public string TinhTrang { get; set; } = null!;
        public bool? IsDelete { get; set; }
    }
}