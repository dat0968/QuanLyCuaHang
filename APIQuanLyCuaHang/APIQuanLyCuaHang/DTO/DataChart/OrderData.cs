using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DTO.DataChart
{
    public class OrderData
    {
        public int ApprovedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int InProgressOrders { get; set; }
    }
}