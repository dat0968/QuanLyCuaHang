using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Constants;

namespace APIQuanLyCuaHang.Helpers.Utils
{
    public static class FormatDate
    {
        public static Func<DateTime, bool> GetDateFilter(string timeRange, DateTime today)
        {
            switch (timeRange)
            {
                case "day":
                    return x => x.Date == today.Date;
                case "week":
                    return x => x.Month == today.Month && (x.Day - 1) / 7 + 1 == (today.Day - 1) / 7 + 1;
                case "month":
                    return x => x.Month == today.Month && x.Year == today.Year;
                case "year":
                    return x => x.Year == today.Year;
                default:
                    throw new ArgumentException("Invalid time range specified.");
            }
        }
    }
}