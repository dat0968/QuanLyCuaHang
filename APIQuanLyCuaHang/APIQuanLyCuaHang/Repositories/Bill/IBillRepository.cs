using APIQuanLyCuaHang.Models;
﻿using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
namespace APIQuanLyCuaHang.Repositories.Bill
{
    public interface IBillRepository
    {
        Task<Hoadon> CreateOrder(Hoadon hoadon);
        Task<IEnumerable<HoaDonDTO>> GetAllBill();
        Task<HoaDonDTO?> GetById(int id);
        Task UpdateStatus(int maHd, string tinhTrang, int? maNv, string? lydohuy);
        Task<(IEnumerable<HoaDonDTO>, int)> GetFilteredBill(string? maHD, string? hinhThucTt, string? tinhTrang, int page, int pageSize);
        Task<HoaDonDTOWithDetails?> GetBillDetails(int maHd);
    }
    public class HoaDonDTOWithDetails : HoaDonDTO
    {
        public List<ChiTietHoaDonDTO> ChiTietHoaDon { get; set; }
    }
}
