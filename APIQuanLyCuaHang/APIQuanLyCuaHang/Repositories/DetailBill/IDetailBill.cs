using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.DetailBill
{
    public interface IDetailBill
    {
        Task<Cthoadon> CreateDetailOrder(Cthoadon model);
    }
}
