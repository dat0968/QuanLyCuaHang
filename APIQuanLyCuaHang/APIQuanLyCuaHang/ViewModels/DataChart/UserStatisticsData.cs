using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.ViewModels.DataChart
{
    public class UserStatisticsData
    {
        public List<UserStatistics>? Users { get; set; }
    }

    public class UserStatistics
    {
        public string? UserType { get; set; }
        public int Active { get; set; }
        public int Inactive { get; set; }
    }
}