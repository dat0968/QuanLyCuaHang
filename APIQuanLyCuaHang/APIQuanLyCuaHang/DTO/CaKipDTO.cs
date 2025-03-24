using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO
{
    public class CaKipDTO
    {
        public int? MaCaKip { get; set; }

        public string? TenCa { get; set; } = null!; // Tên ca làm việc

        public int SoNguoiToiDa { get; set; }

        public int? SoNguoiHienTai { get; set; }

        public TimeOnly GioBatDau { get; set; }

        public TimeOnly GioKetThuc { get; set; }

        public decimal HeSoLuong { get; set; } = 1;// Hệ số lương của ca làm việc
        public bool? IsDelete { get; set; }
        public string? QrCodeData { get; set; }
        public List<LichLamViecDTO>? LichLamViecs { get; set; }
    }
}