namespace APIQuanLyCuaHang.DTO
{
    public class HoaDonCreateDTO
    {
        public int? MaKh { get; set; }
        public int? MaNv { get; set; }
        public string TinhTrang { get; set; }
        public DateTime NgayTao { get; set; }
        public string HinhThucTt { get; set; }
        public decimal TienGoc { get; set; }
        public decimal PhiVanChuyen { get; set; }
        public List<ChiTietHoaDonCreateDTO> Cthoadons { get; set; }
    }

    public class ChiTietHoaDonCreateDTO
    {
        public int? MaCtsp { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; } // Thêm để khớp với frontend
    }
}