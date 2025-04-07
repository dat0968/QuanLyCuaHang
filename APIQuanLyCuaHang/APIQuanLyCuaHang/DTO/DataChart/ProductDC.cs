using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO.DataChart
{
    public class ProductDC
    {
        public int MaSp { get; set; }

        public int MaDanhMuc { get; set; }

        public string TenSanPham { get; set; } = null!;

        public string? MoTa { get; set; }
        public List<DetailProductDC> ChiTietSanPhams { get; set; } = new List<DetailProductDC>();

    }
}