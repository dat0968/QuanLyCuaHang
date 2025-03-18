using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.ViewModels.DataChart
{
    public class EarningData
    {
        public string? Name { get; set; }
        public List<decimal>? Data { get; set; }
        public List<string>? Categories { get; set; }
    }
}