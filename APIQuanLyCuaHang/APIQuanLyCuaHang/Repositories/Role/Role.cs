using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.Role
{
    public class Role : IRole
    {
        private readonly QuanLyCuaHangContext db;
        public Role(QuanLyCuaHangContext db)
        {
            this.db = db;
        }
        public async Task<List<Chucvu>> GetAll()
        {
            var data = await db.Chucvus.AsNoTracking().ToListAsync();
            return data;
        }
    }
}
