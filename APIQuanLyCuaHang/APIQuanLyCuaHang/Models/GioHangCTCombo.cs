using System.ComponentModel.DataAnnotations.Schema;

namespace APIQuanLyCuaHang.Models
{
    public class GioHangCTCombo
    {
        public int Id { get; set; }
        [ForeignKey("MaGioHangNavigation")]
        public int MaGioHang { get; set; }
        [ForeignKey("MaCTSpNavigation")]
        public int MaCTSp { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public virtual Giohang MaGioHangNavigation { get; set; }
        public virtual Chitietsanpham MaCTSpNavigation { get; set; }
    }
}
