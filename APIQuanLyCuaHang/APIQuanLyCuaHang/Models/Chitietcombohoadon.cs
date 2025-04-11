using System.ComponentModel.DataAnnotations.Schema;

namespace APIQuanLyCuaHang.Models
{
    public class Chitietcombohoadon
    {
        public int MaHd { get; set; }

        public int MaCombo { get; set; }

        public int MaCTSp { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; } 

        [ForeignKey("MaHd")]
        public virtual Hoadon MaHdNavigation { get; set; } = null!;

        [ForeignKey("MaCombo")]
        public virtual Combo MaComboNavigation { get; set; } = null!;

        [ForeignKey("MaCTSp")]
        public virtual Chitietsanpham MaCTSpNavigation { get; set; } = null!;
    }
}
