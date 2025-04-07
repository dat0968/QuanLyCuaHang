using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Repositories.CaKip;
using APIQuanLyCuaHang.Repositories.Dashboard;
using APIQuanLyCuaHang.Repositories.OrderClient;
using APIQuanLyCuaHang.Repositories.Schedule;

namespace APIQuanLyCuaHang.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        IScheduleRepository Schedules { get; }
        IShiftRepository CaKips { get; }
        IOrderClientRepository HoaDonKhachs { get; }
    }
}