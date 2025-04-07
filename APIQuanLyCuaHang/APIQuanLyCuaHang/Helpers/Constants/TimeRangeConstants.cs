using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Helpers.Constants
{
    public class TimeRangeConstants
    {
        public const string Day = "day";
        public const string Week = "week";
        public const string Month = "month";
        public const string Year = "year";
        public const string All = "all";

        public static readonly string[] DaysOfWeek = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        public static readonly int[] WeeksInMonth = Enumerable.Range(1, 4).ToArray();

        public static readonly int[] MonthsInYear = Enumerable.Range(1, 12).ToArray();

        public static readonly int[] RecentYears = Enumerable.Range(DateTime.Now.Year - 5, 5).ToArray();
    }
}