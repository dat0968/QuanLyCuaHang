using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.Role
{
    public interface IRole
    {
        Task<List<Chucvu>> GetAll();
    }
}
