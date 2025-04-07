using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO.DataChart
{
    public class OrderOverviewData
    {
        public List<OrderOverview>? Overview { get; set; }
        public List<string>? Categories { get; set; }
        public int TotalOrders { get; set; }
    }

    public class OrderOverview
    {
        public string? Name { get; set; }
        public List<ParameterDataOrder>? Data { get; set; }
    }

    public class ParameterDataOrder
    {
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }
}