using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.DetailComboOrder
{
    public interface IDetailComboOrderRepository
    {
        Task<Chitietcombohoadon> AddDetailComboOrder(Chitietcombohoadon model);
    }
}
