using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.DetailProduct
{
    public interface IDetailProduct
    {
        Task<Chitietsanpham> AddDetailProduct(Chitietsanpham model);
        Task UpdateDetailProduct (Chitietsanpham model);
        Task<List<Chitietsanpham>> GetDetailProductByMaSP(int MaSp);
        Task DeleteDetailProduct(int MaCtsp);
    }
}
