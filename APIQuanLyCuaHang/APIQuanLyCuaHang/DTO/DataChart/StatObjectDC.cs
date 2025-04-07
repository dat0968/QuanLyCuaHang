using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO.DataChart
{
    public class StatObjectDC
    {
        public string NameObject { get; set; } = string.Empty;
        public int AmountActive { get; set; }
        public int AmountInactive { get; set; }
    }
}