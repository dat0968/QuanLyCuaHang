using System.ComponentModel.DataAnnotations;

namespace APIQuanLyCuaHang.Models
{
    public class MaCoupon
    {
        [Key]
        public string MaCode { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 10);
        public string? MoTa { get; set; }
        public decimal? PhanTramGiam { get; set; }
        public decimal? SoTienGiam { get; set; }
        public decimal? DonHangToiThieu { get; set; }
        [Required]
        public DateTime NgayBatDau { get; set; }
        [Required]
        public DateTime NgayKetThuc { get; set; }
        [Required]
        public int SoLuong { get; set; }
        [Required]
        public int SoLuongDaDung { get; set; }
        [Required]
        public bool TrangThai { get; set; }
    }
}
