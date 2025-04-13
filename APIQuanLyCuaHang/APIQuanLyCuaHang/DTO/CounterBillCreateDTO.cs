namespace APIQuanLyCuaHang.DTO
{
    public class CounterBillCreateDTO
    {
        public int? MaKh { get; set; }
        public int? MaNv { get; set; }
        public string TinhTrang { get; set; }
        public DateTime NgayTao { get; set; }
        public string HinhThucTt { get; set; }
        public decimal TienGoc { get; set; }
        public decimal PhiVanChuyen { get; set; }
        public List<CounterBillDetailCreateDTO> Cthoadons { get; set; }
    }

    public class CounterBillDetailCreateDTO
    {
        public int? MaCtsp { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }
}