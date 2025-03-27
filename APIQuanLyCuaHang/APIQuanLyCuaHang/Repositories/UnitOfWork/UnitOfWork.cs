using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.CaKip;
using APIQuanLyCuaHang.Repositories.Dashboard;
using APIQuanLyCuaHang.Repositories.HoaDonKhach;
using APIQuanLyCuaHang.Repositories.LichLamViec;
using Microsoft.Net.Http.Headers;

namespace APIQuanLyCuaHang.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QuanLyCuaHangContext _context;
        public IDashboardRepository Dashboard { get; private set; }
        public ILichLamViecRepository LichLamViecs { get; private set; }
        public ICaKipRepository CaKips { get; private set; }
        public IHoaDonKhachRepository HoaDonKhachs { get; private set; }
        public UnitOfWork(QuanLyCuaHangContext context)
        {
            _context = context;
            Dashboard = new DashboardRepository(_context);
            LichLamViecs = new LichLamViecRepository(_context);
            CaKips = new CaKipRepository(_context);
            HoaDonKhachs = new HoaDonKhachRepository(_context);
        }
    }
}