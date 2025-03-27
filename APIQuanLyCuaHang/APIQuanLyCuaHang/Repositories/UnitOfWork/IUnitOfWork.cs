using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Repositories.CaKip;
using APIQuanLyCuaHang.Repositories.Dashboard;
using APIQuanLyCuaHang.Repositories.HoaDonKhach;
using APIQuanLyCuaHang.Repositories.LichLamViec;

namespace APIQuanLyCuaHang.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        ILichLamViecRepository LichLamViecs { get; }
        ICaKipRepository CaKips { get; }
        IHoaDonKhachRepository HoaDonKhachs { get; }
    }
}