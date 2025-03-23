using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.Product
{
    public interface IProduct
    {
        Task<List<ProductResponseDTO>> GetAll(string? search, int? filterCatories, string? sort, string? filterPrices);
        Task<ProductResponseDTO> GetById(int id);
        Task<Sanpham> AddProduct(Sanpham product);
        Task EditProduct(Sanpham product);
        Task CancelProduct(int id);
    }
}
