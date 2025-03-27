using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.DTO.Requests;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Repository;

namespace APIQuanLyCuaHang.Repositories.LichLamViec
{
    public interface ILichLamViecRepository : IRepository<Lichsulamviec>
    {
        Task<ResponseAPI<dynamic>> GetAllAsync();
        Task<ResponseAPI<dynamic>> DangKyCaLamViecAsync(int? maNv, int maCaKip, DateOnly? ngayLam);
        Task<ResponseAPI<dynamic>> TimeKeepingAsync(int maNv, string qrCodeData);
        Task<ResponseAPI<dynamic>> SetStatusOne(SetStatusOneRequest request, int? managerUserId);
        Task<ResponseAPI<dynamic>> SetStatusList(SetStatusListRequest request, int? managerUserId);
    }
}