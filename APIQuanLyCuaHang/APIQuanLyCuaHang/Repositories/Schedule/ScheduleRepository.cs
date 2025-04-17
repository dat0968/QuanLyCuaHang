using APIQuanLyCuaHang.Constants;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.DTO.Requests;
using APIQuanLyCuaHang.Helpers.Handlers;
using APIQuanLyCuaHang.Helpers.Utils;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Repository;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Repositories.Schedule
{
    public class ScheduleRepository(QuanLyCuaHangContext db) : Repository<Lichsulamviec>(db), IScheduleRepository
    {
        private readonly QuanLyCuaHangContext _db = db;
        #region [PUBLIC METHOD]

        #region [MAIN METHOD] 
        public async Task<ResponseAPI<List<ScheduleDTO>>> SignUpScheduleWorkAsync(int? maNv, int maCaKip, DateOnly? ngayLam)
        {
            ResponseAPI<List<ScheduleDTO>> response = new();

            try
            {
                if (!maNv.HasValue)
                {
                    throw new Exception("Không nhận được mã nhân viên.");
                }

                bool existStaff = await _db.Nhanviens.AnyAsync(nv => nv.MaNv == maNv);
                if (!existStaff)
                {
                    throw new Exception("Không tìm thấy nhân viên trong hệ thống.");
                }

                var caKip = await _db.Cakips.FindAsync(maCaKip);
                if (caKip == null || caKip.IsDelete == true)
                {
                    throw new Exception("Ca kíp không hợp lệ hoặc đã bị xóa.");
                }
                if (caKip.IsDelete!.Value) throw new Exception("Ca này đã bị vô hiệu hóa.");


                if (caKip.SoNguoiHienTai >= caKip.SoNguoiToiDa)
                {
                    throw new Exception("Ca kíp đã đầy.");
                }

                bool daDangKy = await _db.Lichsulamviecs
                    .AnyAsync(l => l.MaNv == maNv && l.MaCaKip == maCaKip && l.NgayThangNam == ngayLam);
                if (daDangKy)
                {
                    throw new Exception("Nhân viên đã đăng ký ca này vào ngày hôm nay.");
                }

                var schedule = new Lichsulamviec
                {
                    MaNv = maNv.Value,
                    MaCaKip = maCaKip,
                    NgayThangNam = ngayLam ?? DateOnly.FromDateTime(DateTime.Today),
                    SoGioLam = 0,
                    TongLuong = null,
                    IsDelete = false,
                    TrangThai = TrangThaiLichLamViec.ChoXacNhan
                };

                await _db.Lichsulamviecs.AddAsync(schedule);
                caKip.SoNguoiHienTai++;

                await _db.SaveChangesAsync();

                var scheduleData = await GetSchedulesActiveOfShift(caKip.MaCaKip);
                response.SetSuccessResponse("Đăng ký thành công!");
                response.SetData(scheduleData);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }

            return response;
        }

        public async Task<ResponseAPI<List<ScheduleDTO>>> TimeKeepingAsync(int maNv, string qrCodeData)
        {
            ResponseAPI<List<ScheduleDTO>> response = new();

            try
            {
                if (!await _db.Nhanviens.AnyAsync(nv => nv.MaNv == maNv)) throw new Exception("Không tìm thấy nhân viên trong hệ thống.");

                var (maCaKip, ngayLam) = QrCodeUtils.ParseQrCodeData(qrCodeData);

                var caKip = await _db.Cakips.FindAsync(maCaKip);
                if (caKip == null) throw new Exception("Ca kíp không hợp lệ.");
                if (caKip.IsDelete!.Value) throw new Exception("Ca này đã bị vô hiệu hóa.");

                var lichLam = await _db.Lichsulamviecs
                    .FirstOrDefaultAsync(l => l.MaNv == maNv && l.MaCaKip == maCaKip && l.NgayThangNam == ngayLam && l.TrangThai == TrangThaiLichLamViec.ChoXacNhan);

                if (lichLam == null) // Tạo mới
                {
                    lichLam = new Lichsulamviec
                    {
                        MaNv = maNv,
                        MaCaKip = maCaKip,
                        NgayThangNam = ngayLam,
                        SoGioLam = 0,
                        IsDelete = false,
                        TrangThai = TrangThaiLichLamViec.ChoXacNhan
                    };
                    await _db.Lichsulamviecs.AddAsync(lichLam);
                    response.SetSuccessResponse("Check-in không hợp lệ.");
                }
                else //Sửa lại
                {
                    if (lichLam.TrangThai == TrangThaiLichLamViec.ChoXacNhan)
                    {
                        throw new Exception("Lịch làm việc vẫn đang chờ xác nhận, không thể thay đổi trạng thái.");
                    }

                    DateTime gioBatDau = DateTime.Today.AddHours(caKip.GioBatDau.Hour).AddMinutes(caKip.GioBatDau.Minute);
                    if (caKip.GioBatDau.Hour >= 22 && DateTime.Now.Hour <= 6)
                    {
                        gioBatDau = gioBatDau.AddDays(-1);
                    }

                    double soGioLam = (DateTime.Now - gioBatDau).TotalHours;
                    lichLam.SoGioLam = Math.Max(soGioLam, 0);
                    lichLam.TrangThai = soGioLam != 0 ? TrangThaiLichLamViec.KetThucCa : TrangThaiLichLamViec.Tre;

                    response.SetSuccessResponse(soGioLam >= 0.5 ? "Check-out thành công." : "Bạn đã trễ.");
                }

                var scheduleData = await GetSchedulesActiveOfShift(caKip.MaCaKip);

                await _db.SaveChangesAsync();

                response.SetSuccessResponse("Fine");
                response.SetData(scheduleData);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }

            return response;
        }

        public async Task<ResponseAPI<dynamic>> GetAllAsync()
        {
            ResponseAPI<dynamic> response = new();
            try
            {
                var data = await _db.Lichsulamviecs.ToListAsync();
                response.SetSuccessResponse("Ok");
                response.SetData(data);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }
            return response;
        }
        public async Task<ResponseAPI<dynamic>> SetStatusList(SetStatusListRequest request, int? managerUserId, bool? isCreate)
        {
            ResponseAPI<dynamic> response = new();
            try
            {
                if (!managerUserId.HasValue)
                {
                    throw new ArgumentException("Hiện mã quản lý của bạn không được xác định.");
                }

                if (request.MaNvs == null || request.MaNvs.Length == 0)
                {
                    throw new ArgumentException("Danh sách nhân viên không hợp lệ.");
                }

                if (string.IsNullOrWhiteSpace(request.TrangThaiCapNhap))
                {
                    throw new ArgumentException("Trạng thái cập nhật không hợp lệ.");
                }

                var caKip = await _db.Cakips
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ck => ck.MaCaKip == request.MaCaKip);

                if (caKip == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy CaKip.");
                }
                // Nếu không có ca kíp thì không thể thêm lịch làm việc
                if (caKip.IsDelete == true)
                {
                    throw new KeyNotFoundException("Ca kíp đã bị vô hiệu hóa.");
                }
                if (isCreate.HasValue && isCreate.Value)
                {
                    var existStaff = await _db.Nhanviens.AnyAsync(nv => request.MaNvs.Contains(nv.MaNv));
                    if (!existStaff)
                    {
                        throw new Exception("Không tìm thấy nhân viên trong hệ thống.");
                    }

                    foreach (var maNv in request.MaNvs)
                    {
                        var schedule = new Lichsulamviec
                        {
                            MaNv = maNv,
                            MaCaKip = request.MaCaKip,
                            NgayThangNam = DateOnly.FromDateTime(DateTime.Today),
                            SoGioLam = 0,
                            TongLuong = null,
                            IsDelete = false,
                            TrangThai = request.TrangThaiCapNhap
                        };

                        await _db.Lichsulamviecs.AddAsync(schedule);
                    }
                    caKip.SoNguoiHienTai += request.MaNvs.Length;
                    response.SetSuccessResponse($"Đã thêm {request.MaNvs.Length} nhân viên vào lịch làm việc.");
                }
                else // Cập nhập trạng thái
                {
                    var schedules = await _db.Lichsulamviecs
                        .Where(ls => request.MaNvs.Contains(ls.MaNv) && ls.MaCaKip == request.MaCaKip)
                        .ToListAsync();

                    if (!schedules.Any())
                    {
                        throw new KeyNotFoundException("Không tìm thấy lịch làm việc phù hợp.");
                    }

                    int currentEmployees = caKip.SoNguoiHienTai;

                    foreach (var schedule in schedules)
                    {
                        schedule.NguoiXacNhan = managerUserId;

                        TrangThaiLichLamViec.XuLyTrangThaiLichLamViec(schedule, request, caKip, currentEmployees);

                        schedule.TrangThai = request.TrangThaiCapNhap;
                    }
                    _db.UpdateRange(schedules);
                    response.SetSuccessResponse($"Đã cập nhật trạng thái {request.TrangThaiCapNhap} cho {schedules.Count} nhân viên.");
                }

                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }

            return response;
        }
        public async Task<ResponseAPI<dynamic>> SetStatusOne(SetStatusOneRequest request, int? managerUserId)
        {
            ResponseAPI<dynamic> response = new();
            try
            {
                if (!managerUserId.HasValue)
                {
                    throw new ArgumentException("Hiện mã quản lý của bạn không được xác định.");
                }

                if (!request.MaNv.HasValue)
                {
                    throw new ArgumentException("Mã nhân viên không hợp lệ.");
                }

                if (string.IsNullOrWhiteSpace(request.TrangThaiCapNhap))
                {
                    throw new ArgumentException("Trạng thái cập nhật không hợp lệ.");
                }

                var caKip = await _db.Cakips
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ck => ck.MaCaKip == request.MaCaKip);

                if (caKip == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy CaKip.");
                }

                var schedule = await _db.Lichsulamviecs
                    .FirstOrDefaultAsync(ls => ls.MaNv == request.MaNv && ls.MaCaKip == request.MaCaKip);

                if (schedule == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy lịch làm việc phù hợp.");
                }

                int currentEmployees = caKip.SoNguoiHienTai;

                schedule.NguoiXacNhan = managerUserId;

                TrangThaiLichLamViec.XuLyTrangThaiLichLamViec(schedule, request, caKip, currentEmployees);

                schedule.TrangThai = request.TrangThaiCapNhap;

                if (!string.IsNullOrEmpty(request.GhiChu))
                {
                    schedule.GhiChu = request.GhiChu;
                }

                _db.Update(schedule);
                await _db.SaveChangesAsync();

                response.SetSuccessResponse($"Đã cập nhật trạng thái {request.TrangThaiCapNhap} cho nhân viên {request.MaNv}.");
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }

            return response;
        }
        #endregion

        #region [SUB METHOD] 
        public async Task<ResponseAPI<List<UserIdDTO>>> GetAllUserIdAsync()
        {
            ResponseAPI<List<UserIdDTO>> response = new();
            try
            {
                var data = await _db.Nhanviens
                    .Where(nv => nv.IsDelete == false)
                    .Select(nv => new UserIdDTO() { MaNv = nv.MaNv, HoTen = nv.HoTen })
                    .ToListAsync();

                response.SetSuccessResponse("Ok");
                response.SetData(data);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }
            return response;
        }

        #endregion

        #endregion

        public async Task<ResponseAPI<List<ScheduleDTO>>> GetScheduleOfUser(int? userId)
        {
            ResponseAPI<List<ScheduleDTO>> response = new();
            try
            {
                userId ??= userId ?? throw new Exception("Không nhận được thông tin người dùng.");

                var scheduleUserInNow = await base.GetAsync(x => x.MaNv == userId && (x.TrangThai == TrangThaiLichLamViec.DiLam || x.TrangThai == TrangThaiLichLamViec.ChoXacNhan)) ?? throw new Exception("Không tìm thấy dữ liệu tìm kiếm.");

                List<ScheduleDTO> scheduleData = await GetSchedulesActiveOfShift(scheduleUserInNow.MaCaKip);

                response.SetSuccessResponse("Đã nhận được danh sách.");
                response.SetData(scheduleData);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }
            return response;
        }
        public async Task<ResponseAPI<List<ScheduleDTO>>> GetScheduleActiveOfShift(int? shiftId)
        {
            ResponseAPI<List<ScheduleDTO>> response = new();
            try
            {
                shiftId ??= shiftId ?? throw new Exception("Không nhận được thông tin ca làm việc.");

                var scheduleData = await GetSchedulesActiveOfShift(shiftId.Value);

                response.SetSuccessResponse("Đã nhận được danh sách.");
                response.SetData(scheduleData);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }
            return response;
        }


        #region [PRIVATE METHOD]
        private async Task<List<ScheduleDTO>> GetSchedulesActiveOfShift(int shiftId)
        {
            var dataOrigin = await base.GetAllAsync(x => x.MaCaKip == shiftId && (x.TrangThai == TrangThaiLichLamViec.DiLam || x.TrangThai == TrangThaiLichLamViec.ChoXacNhan), "MaNvNavigation");

            List<ScheduleDTO> scheduleData = dataOrigin.Select(d => new ScheduleDTO
            {
                Id = d.Id,
                MaNv = d.MaNv,
                TenNhanVien = d.MaNvNavigation.HoTen,
                MaCaKip = d.MaCaKip,
                NgayThangNam = d.NgayThangNam,
                SoGioLam = d.SoGioLam,
                LyDoNghi = d.LyDoNghi,
                TongLuong = d.TongLuong,
                GioVao = d.GioVao,
                GioRa = d.GioRa,
                TrangThai = d.TrangThai,
                GhiChu = d.GhiChu,
                IsDelete = d.IsDelete,
            }).ToList();
            return scheduleData;
        }
        #endregion
    }
}
