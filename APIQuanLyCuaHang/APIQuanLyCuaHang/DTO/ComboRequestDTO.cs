namespace APIQuanLyCuaHang.DTO
{
    public class ComboRequestDTO
    {
        public string TenCombo { get; set; }

        public IFormFile? Hinh { get; set; }

        public decimal? SoTienGiam { get; set; }
        public float? PhanTramGiam { get; set; }
        public int SoLuong { get; set; }    

        public string? MoTa { get; set; }

        public bool? IsDelete { get; set; }

        public List<DetaisComboRequestDTO> Chitietcombos { get; set; } = new List<DetaisComboRequestDTO>();
    }
    public class DetaisComboRequestDTO
    {
        public int MaSp { get; set; }
        public int? SoLuongSp { get; set; }
    }
}
