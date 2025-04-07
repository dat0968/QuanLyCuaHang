using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO.Requests
{
    public class SetStatusOneRequest
    {
        public int? MaNv { get; set; }
        public string TrangThaiCapNhap { get; set; } = string.Empty;
        public int MaCaKip { get; set; }
        public string? GhiChu { get; set; } = string.Empty;
    }
}