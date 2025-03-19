using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.ViewModels.DataChart
{
    public class TopSellingProductsData
    {
        public List<ProductStatistics>? Products { get; set; }
    }

    public class ProductStatistics
    {
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}