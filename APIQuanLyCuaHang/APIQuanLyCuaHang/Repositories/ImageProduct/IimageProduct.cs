using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.ImageProduct
{
    public interface IimageProduct
    {
        Task<Hinhanh> AddImageProduct(Hinhanh imageProduct);
        Task DeleteImageProductByMaCtSp(int id);
    }
}
