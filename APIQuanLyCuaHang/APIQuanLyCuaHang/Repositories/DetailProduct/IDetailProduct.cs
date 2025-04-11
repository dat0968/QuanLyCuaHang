using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace APIQuanLyCuaHang.Repositories.DetailProduct
{
    public interface IDetailProduct
    {
        Task<Chitietsanpham> AddDetailProduct(Chitietsanpham model);
        Task UpdateDetailProduct (Chitietsanpham model);
        Task<List<Chitietsanpham>> GetDetailProductByMaSP(int MaSp);
        Task DeleteDetailProduct(int MaCtsp);
        Task<Chitietsanpham?> CheckExistDetailProduct(int MaSp, string? KichThuoc, string? HuongVi);
        Task<Chitietsanpham?> GetDetailByMaCTSp(int MaCTSp);
    }
}
