using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.CaKip;
using APIQuanLyCuaHang.Repositories.Dashboard;
using APIQuanLyCuaHang.Repositories.LichLamViec;

namespace APIQuanLyCuaHang.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QuanLyCuaHangContext _context;
        public IDashboardRepository Dashboard { get; private set; }
        public ILichLamViecRepository LichLamViecs { get; private set; }
        public ICaKipRepository CaKips { get; private set; }
        public UnitOfWork(QuanLyCuaHangContext context)
        {
            _context = context;
            Dashboard = new DashboardRepository(_context);
            LichLamViecs = new LichLamViecRepository(_context);
            CaKips = new CaKipRepository(_context);
        }
    }
}