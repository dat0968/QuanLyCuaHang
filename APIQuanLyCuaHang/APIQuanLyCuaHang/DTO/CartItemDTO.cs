using APIQuanLyCuaHang.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIQuanLyCuaHang.DTO
{
    public class CartItemDTO
    {
        public int Id { get; set; }

        public int MaKh { get; set; }

        public int? MaCtsp { get; set; }
        [Column("MaComboNavigation")]
        public int? MaCombo { get; set; }


        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }
        public string TenSanPham { get; set; }
        public string? KichThuoc { get; set; }
        public string? HuongVi { get; set; }
        public string HinhAnh { get; set; }
        public string TenCombo { get; set; }
        public virtual List<CartDetailCombo> CartDetailCombos { get; set; } = new List<CartDetailCombo>();
    }
    public class CartDetailCombo
    {
        public int Id { get; set; }
        public int MaGioHang { get; set; }
        public int MaCTSp { get; set; }
        public int MaSp { get; set; }
        public string? TenSanPham { get; set; }
        public string? KichThuoc { get; set; }
        public string? HuongVi { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }

    }
}