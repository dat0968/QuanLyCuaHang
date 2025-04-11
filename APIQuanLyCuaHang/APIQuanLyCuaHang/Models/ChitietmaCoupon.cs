using System.ComponentModel.DataAnnotations.Schema;

namespace APIQuanLyCuaHang.Models
{
    public class ChitietmaCoupon
    {
        [ForeignKey("MaCode")]
        public string MaCode { get; set; }
        [ForeignKey("MaKh")]
        public int MaKh { get; set; }
        public DateTime NgaySuDung { get; set; }

        public virtual MaCoupon MaCodeNavigation { get; set; }
        public virtual Khachhang MaKhNavigation { get; set; }
    }
}
