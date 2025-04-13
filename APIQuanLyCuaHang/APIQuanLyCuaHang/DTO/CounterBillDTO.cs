namespace APIQuanLyCuaHang.DTO
{
    public class CounterBillDTO
    {
        public int MaHd { get; set; }
        public int? MaKh { get; set; }
        public int? MaNv { get; set; }
        public string TinhTrang { get; set; }
        public DateTime NgayTao { get; set; }
        public string HinhThucTt { get; set; }
        public decimal TienGoc { get; set; }
        public decimal PhiVanChuyen { get; set; }
        public string HoTen { get; set; }
        public string Sdt { get; set; }
    }
}