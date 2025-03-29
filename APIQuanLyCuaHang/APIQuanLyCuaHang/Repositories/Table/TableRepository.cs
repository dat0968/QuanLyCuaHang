using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.Table
{
    public class TableRepository : ITableRepository
    {
        private readonly QuanLyCuaHangContext db;

        public TableRepository(QuanLyCuaHangContext db)
        {
            this.db = db;
        }

        public async Task<List<BanDTO>> GetAllTables()
        {
            return await db.Bans
                .Select(b => new BanDTO
                {
                    Id = b.Id,
                    TinhTrang = b.TinhTrang,
                })
                .ToListAsync();
        }

        public async Task<BanDTO> GetTableById(int id)
        {
            var ban = await db.Bans
                .FirstOrDefaultAsync(b => b.Id == id);

            if (ban == null) return null;

            return new BanDTO
            {
                Id = ban.Id,
                TinhTrang = ban.TinhTrang,
            };
        }

        public async Task<Ban> AddTable(Ban ban)
        {
            try
            {
                db.Bans.Add(ban);
                await db.SaveChangesAsync();
                return ban;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BanDTO> UpdateTable(int id, BanDTO banDTO)
        {
            var existingBan = await db.Bans.FindAsync(id);
            if (existingBan == null)
            {
                return null; // Trả về null nếu không tìm thấy
            }

            var immutableStatuses = new List<string> { "DangSuDung", "DangSuaChua" }; // Cập nhật trạng thái không thể thay đổi
            if (immutableStatuses.Contains(existingBan.TinhTrang))
            {
                throw new Exception($"Bàn với trạng thái '{existingBan.TinhTrang}' không thể được cập nhật.");
            }

            // Cập nhật trạng thái
            existingBan.TinhTrang = banDTO.TinhTrang;
            await db.SaveChangesAsync();

            return new BanDTO
            {
                Id = existingBan.Id,
                TinhTrang = existingBan.TinhTrang
            };
        }

        public async Task<bool> DeleteTable(int id)
        {
            var ban = await db.Bans.FindAsync(id);
            if (ban == null) return false;

            db.Bans.Remove(ban);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<BanDTO>, int)> FilterTablesByStatus(string tinhTrang, int page, int pageSize)
        {
            var query = db.Bans.AsQueryable();
            if (!string.IsNullOrEmpty(tinhTrang))
            {
                query = query.Where(hd => hd.TinhTrang == tinhTrang);
            }

            int totalItems = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(hd => new BanDTO
                {
                    Id = hd.Id,
                    TinhTrang = hd.TinhTrang
                })
                .ToListAsync();

            return (data, totalItems);
        }
    }
}