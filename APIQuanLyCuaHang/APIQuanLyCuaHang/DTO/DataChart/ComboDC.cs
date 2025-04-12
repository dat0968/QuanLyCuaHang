using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO.DataChart
{
    public class ComboDC
    {
        public int MaCombo { get; set; }

        public string TenCombo { get; set; } = null!;

        public string? Hinh { get; set; }
        public int SoLuong { get; set; } = 1;
        public decimal DonGia { get; set; }
        public decimal? PhanTramGiam { get; set; }
        public decimal? SoTienGiam { get; set; }
        public decimal? TienGoc { get; set; }
        public decimal? TongTien { get; set; }
        public string? MoTa { get; set; }

        public bool? IsDelete { get; set; }
        public List<DetailComboDC>? ChiTietCombos { get; set; }
    }
}