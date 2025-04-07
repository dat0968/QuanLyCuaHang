using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.Table
{
    public interface ITableRepository
    {
        Task<List<BanDTO>> GetAllTables();
        Task<BanDTO> GetTableById(int id);
        Task<Ban> AddTable(Ban ban);
        Task<BanDTO> UpdateTable(int id, BanDTO banDTO); // Giữ nguyên chữ ký
        Task<bool> DeleteTable(int id);
        Task<(IEnumerable<BanDTO>, int)> FilterTablesByStatus(string tinhTrang, int page, int pageSize);
    }
}