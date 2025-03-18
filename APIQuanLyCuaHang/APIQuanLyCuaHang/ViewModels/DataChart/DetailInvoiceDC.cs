using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.ViewModels.DataChart
{
    public class DetailInvoiceDC
    {
        public int MaHd { get; set; }

        public int MaCtsp { get; set; }

        public int SoLuong { get; set; }

        public int MaSp { get; set; }
        public int MaDanhMuc { get; set; }

        public string TenSanPham { get; set; } = null!;

        public string? MoTa { get; set; }

        public string? KichThuoc { get; set; }

        public string? HuongVi { get; set; }

        public int? SoLuongTon { get; set; }

        public decimal? DonGia { get; set; }
        public decimal? TongTien { get; set; }
        public string? LinkAnhDau { get; set; }
    }
}