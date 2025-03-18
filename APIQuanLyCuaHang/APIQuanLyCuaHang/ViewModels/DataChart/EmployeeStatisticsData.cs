using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.ViewModels.DataChart
{
    public class EmployeeStatisticsData
    {
        public List<EmployeeStatistics>? Employees { get; set; }
    }

    public class EmployeeStatistics
    {
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public int ProcessedOrders { get; set; }
        public decimal RevenueGenerated { get; set; }
    }
}