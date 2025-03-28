using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.Bill
{
    public interface IBillRepository
    {
        Task<Hoadon> CreateOrder(Hoadon hoadon);
    }
}
