using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.DTO.Requests;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Repository;

namespace APIQuanLyCuaHang.Repositories.Schedule
{
    public interface IScheduleRepository : IRepository<Lichsulamviec>
    {
        Task<ResponseAPI<dynamic>> GetAllAsync();
        Task<ResponseAPI<List<ScheduleDTO>>> SignUpScheduleWorkAsync(int? maNv, int maCaKip, DateOnly? ngayLam);
        Task<ResponseAPI<List<ScheduleDTO>>> TimeKeepingAsync(int maNv, string qrCodeData);
        Task<ResponseAPI<dynamic>> SetStatusOne(SetStatusOneRequest request, int? managerUserId);
        Task<ResponseAPI<dynamic>> SetStatusList(SetStatusListRequest request, int? managerUserId, bool? isCreate);
        Task<ResponseAPI<List<ScheduleDTO>>> GetScheduleOfUser(int? userId);
        Task<ResponseAPI<List<ScheduleDTO>>> GetScheduleActiveOfShift(int? shiftId);
        Task<ResponseAPI<List<UserIdDTO>>> GetAllUserIdAsync();
    }
}