using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO.DataChart
{
    public class DetailProductDC
    {
        public int MaCtsp { get; set; }

        public int MaSp { get; set; }

        public string? KichThuoc { get; set; }

        public string? HuongVi { get; set; }

        public int? SoLuongTon { get; set; }

        public decimal? DonGia { get; set; }
        public string? TenHinhAnh { get; set; }
    }
}