using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO.DataChart
{
    public class DetailComboDC
    {
        public int MaCombo { get; set; }
        public int MaSp { get; set; }
        public string TenSanPham { get; set; } = null!;
        public bool? IsProductDelete { get; set; }
        public int? SoLuongSp { get; set; }

        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; } = null!;
        public bool? IsCategoryDelete { get; set; }

        public string? MoTa { get; set; }
    }
}