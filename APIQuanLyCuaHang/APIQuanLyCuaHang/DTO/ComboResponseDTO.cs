using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.DTO
{
    public class ComboResponseDTO
    {
        public int MaCombo { get; set; }
        public string TenCombo { get; set; }

        public string? Hinh { get; set; }
        public decimal? SoTienGiam { get; set; }
        public float? PhanTramGiam { get; set; }
        public int SoLuong { get; set; }

        public string? MoTa { get; set; }

        public bool? IsDelete { get; set; }
        public List<DetaisComboResponseDTO> Chitietcombos { get; set; } = new List<DetaisComboResponseDTO>();
    }
    public class DetaisComboResponseDTO
    {
        public int MaSp { get; set; }
        public string TenSp { get; set; }
        public int? SoLuongSp { get; set; }
        public List<DetailProductResponseDTO> Chitietsanphams { get; set; } = new List<DetailProductResponseDTO>(); 
    }
}
