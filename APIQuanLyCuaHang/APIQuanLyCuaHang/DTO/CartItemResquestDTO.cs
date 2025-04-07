using System.ComponentModel.DataAnnotations.Schema;

namespace APIQuanLyCuaHang.DTO
{
    public class CartItemResquestDTO
    {
    

        public int MaKh { get; set; }

        public int? MaCtsp { get; set; }
        [Column("MaComboNavigation")]
        public int? MaCombo { get; set; }


        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }
        public virtual List<CartDetailRequestCombo> CartDetailRequestCombos { get; set; } = new List<CartDetailRequestCombo>();
    }
    public class CartDetailRequestCombo
    {
        public int MaCTSp { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }

    }
}
