using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO.DataChart
{
    public class OrderStatusData
    {
        public List<string>? Labels { get; set; }
        public List<int>? Data { get; set; }
    }
}