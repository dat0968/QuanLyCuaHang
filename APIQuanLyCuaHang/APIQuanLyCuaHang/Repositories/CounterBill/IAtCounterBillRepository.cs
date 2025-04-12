using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Bill;

namespace APIQuanLyCuaHang.Repositories.AtCounterBill
{
    public interface IAtCounterBillRepository
    {
        Task<Hoadon> CreateOrder(Hoadon hoadon);
        Task<Cthoadon> CreateDetailOrder(Cthoadon cthoadon);
        Task<HoaDonDTOWithDetails?> GetBillDetails(int maHd); // Thêm phương thức này
    }
}