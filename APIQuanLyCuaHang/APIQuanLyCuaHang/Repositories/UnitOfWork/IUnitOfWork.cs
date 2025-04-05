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
        ICaKipRepository CaKips { get; }
        IOrderClientRepository HoaDonKhachs { get; }
    }
}