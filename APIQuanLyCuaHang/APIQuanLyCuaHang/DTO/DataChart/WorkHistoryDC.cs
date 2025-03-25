using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO.DataChart
{
    public class WorkHistoryDC
    {
        public int MaNv { get; set; }

        public string? TenNhanVien { get; set; }

        public int MaCaKip { get; set; }

        public double TongSoGioLam { get; set; }

        public decimal? TongLuong { get; set; }
    }
}