using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO
{
    public class CaKipDTO
    {
        public int? MaCaKip { get; set; }

        public int SoNguoiToiDa { get; set; }

        public int SoNguoiHienTai { get; set; }

        public DateTime GioBatDau { get; set; }

        public DateTime GioKetThuc { get; set; }

        public bool? IsDelete { get; set; }
    }
}