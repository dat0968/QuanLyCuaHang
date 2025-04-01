using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Repository;

namespace APIQuanLyCuaHang.Repositories.HoaDonKhach
{
    public interface IHoaDonKhachRepository : IRepository<Hoadon>
    {
        Task<ResponseAPI<List<HoaDonKhachDTO>>> GetAllInvoiceByUserId(int? userId);
        Task<ResponseAPI<dynamic>> UpdateStatusOrderOfUser(int? userId, int? orderId, string? statusChange, string? reasonCancel);
    }
}