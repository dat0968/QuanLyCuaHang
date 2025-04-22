using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Constants;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Helpers.Handlers;
using APIQuanLyCuaHang.Helpers.Utils;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.CaKip
{
    public class ShiftRepository(QuanLyCuaHangContext db) : Repository<Cakip>(db), IShiftRepository
    {
        private readonly QuanLyCuaHangContext _db = db;

        public async Task<ResponseAPI<string>> ChangeStatusAsync(int? id)
        {
            ResponseAPI<string> response = new();
            try
            {
                if (!id.HasValue)
                {
                    throw new BadHttpRequestException("Dữ liệu không nhận được để xử lí.");
                }
                var caKip = await _db.Cakips.FindAsync(id);

                if (caKip == null)
                {
                    throw new KeyNotFoundException("Dữ liệu không tìm thấy trong hệ thống.");
                }

                caKip.IsDelete = !caKip.IsDelete;

                _db.Update(caKip);
                await _db.SaveChangesAsync();

                response.SetSuccessResponse("Đã thay đổi trạng thái Ca thành công.");
                response.SetData("Ok");
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }
            return response;
        }

        public async Task<ResponseAPI<List<CaKipDTO>>> GetAllAsync()
        {
            ResponseAPI<List<CaKipDTO>> response = new();
            try
            {
                // Lấy dữ liệu
                var data = await GetData();

                // Đặt dữ liệu trả về
                response.SetSuccessResponse("Lấy danh sách ca kíp thành công!");
                response.SetData(data);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }

            return response;
        }
        public async Task<ResponseAPI<CaKipDTO>> GetAllEmployeesInShiftAsync(int? maCaKip)
        {
            var response = new ResponseAPI<CaKipDTO>();
            try
            {
                if (!maCaKip.HasValue)
                {
                    throw new ArgumentException("Không nhận được dữ liệu ca để phản hồi.");
                }

                var today = DateOnly.FromDateTime(DateTime.Today);

                // Truy vấn dữ liệu ca kíp
                var dataOrigin = await _db.Cakips
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ck => ck.MaCaKip == maCaKip);

                if (dataOrigin == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy ca kíp với mã được cung cấp.");
                }

                // Truy vấn lịch làm việc
                var schedules = await _db.Lichsulamviecs
                    .AsNoTracking()
                    .Where(ls => ls.MaCaKip == maCaKip)
                    .ToListAsync();

                // Truy vấn danh sách nhân viên để giảm số lần truy vấn
                var maNhanVienList = schedules.Select(ls => ls.MaNv).Distinct().ToList();
                var nhanVienDict = await _db.Nhanviens
                    .AsNoTracking()
                    .Where(nv => maNhanVienList.Contains(nv.MaNv))
                    .ToDictionaryAsync(nv => nv.MaNv, nv => nv.HoTen);

                // Định dạng dữ liệu lịch làm việc
                var scheduleDtos = schedules.Select(ls => new ScheduleDTO
                {
                    Id = ls.Id,
                    MaNv = ls.MaNv,
                    TenNhanVien = nhanVienDict.GetValueOrDefault(ls.MaNv), // Lấy tên từ dictionary
                    MaCaKip = ls.MaCaKip,
                    NgayThangNam = ls.NgayThangNam,
                    SoGioLam = ls.SoGioLam,
                    LyDoNghi = ls.LyDoNghi,
                    TongLuong = ls.TongLuong,
                    IsDelete = ls.IsDelete,
                    GioVao = ls.GioVao,
                    GioRa = ls.GioRa,
                    TrangThai = ls.TrangThai,
                    GhiChu = ls.GhiChu,
                }).ToList();

                // Định dạng dữ liệu cho phản hồi
                var dataFormatted = new CaKipDTO
                {
                    MaCaKip = maCaKip,
                    TenCa = dataOrigin.TenCa ?? "N/A",
                    SoNguoiToiDa = dataOrigin.SoNguoiToiDa,
                    SoNguoiHienTai = dataOrigin.SoNguoiHienTai,
                    GioBatDau = dataOrigin.GioBatDau,
                    GioKetThuc = dataOrigin.GioKetThuc,
                    IsDelete = dataOrigin.IsDelete,
                    QrCodeData = $"{dataOrigin.MaCaKip}-{today}",
                    Schedules = scheduleDtos
                };

                response.SetSuccessResponse("Lấy danh sách Ca thành công!");
                response.SetData(dataFormatted);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }

            return response;
        }

        public async Task<ResponseAPI<dynamic>> RemoveAsync(int? id)
        {
            ResponseAPI<dynamic> response = new();
            try
            {
                if (!id.HasValue)
                {
                    throw new Exception("Không nhận được dữ liệu ca.");
                }
                var targetRemove = await _db.Cakips.FindAsync(id);
                if (targetRemove == null)
                {
                    throw new Exception("Dữ liệu không thấy được trong hệ thống.");
                }
                if (await _db.Lichsulamviecs.AnyAsync(ls => ls.MaCaKip == id))
                {
                    throw new Exception("Ca này đã có dữ liệu làm việc nên không thể xóa.");
                }
                _db.Remove(targetRemove);
                await _db.SaveChangesAsync();
                response.SetSuccessResponse("Xóa ca thành công!");

            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }
            return response;
        }

        public async Task<ResponseAPI<List<CaKipDTO>>> UpsertCrewAsync(CaKipDTO? caKip)
        {
            ResponseAPI<List<CaKipDTO>> response = new();
            try
            {
                if (caKip == null)
                {
                    throw new Exception("Dữ liệu không nhận được để xử lí.");
                }

                if (caKip.SoNguoiToiDa < 1) throw new Exception("Không được đặt số người tối đa nhỏ hơn 1!");

                if (!caKip.MaCaKip.HasValue || caKip.MaCaKip == 0) // Thêm
                {
                    Cakip newCaKip = new Cakip
                    {
                        SoNguoiToiDa = caKip.SoNguoiToiDa,
                        TenCa = caKip.TenCa,
                        GioBatDau = caKip.GioBatDau,
                        GioKetThuc = caKip.GioKetThuc,
                        IsDelete = caKip.IsDelete,
                    };

                    await _db.AddAsync(newCaKip);
                }
                else // Sửa
                {
                    var updateCaKip = await _db.Cakips.FindAsync(caKip.MaCaKip);
                    if (updateCaKip == null)
                    {
                        throw new Exception("Dữ liệu không tìm thấy trong hệ thống.");
                    }
                    updateCaKip.SoNguoiToiDa = caKip.SoNguoiToiDa;
                    updateCaKip.TenCa = caKip.TenCa;
                    updateCaKip.GioBatDau = caKip.GioBatDau;
                    updateCaKip.GioKetThuc = caKip.GioKetThuc;
                    updateCaKip.IsDelete = caKip.IsDelete;

                    _db.Update(updateCaKip);
                }
                await _db.SaveChangesAsync();

                var data = await _db.Cakips.Select(ck => new CaKipDTO
                {
                    MaCaKip = ck.MaCaKip,
                    SoNguoiToiDa = ck.SoNguoiToiDa,
                    GioBatDau = ck.GioBatDau,
                    GioKetThuc = ck.GioKetThuc,
                    IsDelete = ck.IsDelete,
                }).ToListAsync();

                response.SetSuccessResponse("Lấy danh sách Ca thành công!");
                response.SetData(data);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }
            return response;
        }
        public async Task AutoUpdateAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Lấy lịch làm việc (trước và bằng ngày hôm nay)
            var schedules = await _db.Lichsulamviecs
                .Where(ls => ls.NgayThangNam <= today)
                .ToListAsync();

            // Lấy mã nhân viên từ lịch làm việc
            var maNhanVienList = schedules.Select(ls => ls.MaNv).Distinct().ToList();

            // Truy vấn thông tin nhân viên
            var nhanVienDict = await _db.Nhanviens
                .AsNoTracking()
                .Where(nv => maNhanVienList.Contains(nv.MaNv))
                .ToDictionaryAsync(nv => nv.MaNv, nv => (nv.HoTen, nv.MaChucVu));

            // Lấy thông tin lương theo mã chức vụ
            var maChucVuList = nhanVienDict.Values.Select(nv => nv.MaChucVu).Distinct().ToList();
            var roleStaffDict = await _db.Chucvus
                .AsNoTracking()
                .Where(role => maChucVuList.Contains(role.MaChucVu))
                .ToDictionaryAsync(role => role.MaChucVu, role => role.LuongTheoGio);

            // Cập nhật thông tin lịch làm việc và trạng thái nếu cần thiết
            bool needUpdate = false;
            foreach (var ls in schedules)
            {
                var caKip = await _db.Cakips.FirstOrDefaultAsync(ca => ca.MaCaKip == ls.MaCaKip);
                if (caKip == null) continue;

                // Lấy lương theo giờ từ chức vụ
                var luongTheoGio = roleStaffDict.TryGetValue(nhanVienDict.GetValueOrDefault(ls.MaNv).MaChucVu!.Value, out var luong) ? luong : 0;

                // Cập nhật giờ vào/ra nếu chưa có
                if (ls.TongLuong == 0 && ls.SoGioLam == 0 && ls.TrangThai == TrangThaiLichLamViec.ChoXacNhan)
                {
                    ls.GioVao ??= caKip.GioBatDau;
                    ls.GioRa ??= caKip.GioKetThuc;
                }

                // Kiểm tra tính hợp lệ của dữ liệu
                if (ls.SoGioLam == 0 || ls.TongLuong == 0)
                {
                    if (ls.TrangThai == TrangThaiLichLamViec.ChoXacNhan)
                    {
                        ls.TrangThai = TrangThaiLichLamViec.KhongDuocXacNhan;
                        ls.GhiChu = "Auto: Dữ liệu SoGioLam hoặc TongLuong không đầy đủ, không thể cập nhật.";
                        needUpdate = true;
                        continue;
                    }
                }

                // Giờ ra không hợp lệ
                if (ls.GioRa.HasValue && ls.GioVao.HasValue && ls.GioRa < ls.GioVao)
                {
                    ls.TrangThai = TrangThaiLichLamViec.KhongDuocXacNhan;
                    ls.GhiChu = "Auto: Giờ ra nhỏ hơn giờ vào.";
                    needUpdate = true;
                    continue;
                }

                // Tính toán số giờ làm
                if (ls.GioVao.HasValue && ls.GioRa.HasValue)
                {
                    ls.SoGioLam = (ls.GioRa.Value - ls.GioVao.Value).TotalHours;
                }

                // Gán tổng lương nếu chưa có nhưng số giờ làm hợp lệ
                if (ls.TongLuong == 0 && ls.SoGioLam > 0)
                {
                    ls.TongLuong = (decimal)ls.SoGioLam * luongTheoGio;
                }

                // Xử lý trạng thái tự động
                if (ls.NgayThangNam < today)
                {
                    if (ls.TrangThai == TrangThaiLichLamViec.ChoXacNhan)
                    {
                        if (ls.GioVao.HasValue && ls.GioRa.HasValue && ls.SoGioLam > 0 && ls.TongLuong > 0)
                        {
                            ls.TrangThai = TrangThaiLichLamViec.KetThucCa;
                            ls.GhiChu = "Auto: Ca làm đã kết thúc tự động.";
                        }
                        else
                        {
                            ls.TrangThai = TrangThaiLichLamViec.KhongDuocXacNhan;
                            ls.GhiChu = "Auto: Dữ liệu không đầy đủ.";
                        }
                        needUpdate = true;
                    }
                }
                else if (ls.NgayThangNam > today)
                {
                    if (ls.TrangThai == TrangThaiLichLamViec.ChoXacNhan)
                    {
                        ls.TrangThai = TrangThaiLichLamViec.KhongDuocXacNhan;
                        ls.GhiChu = "Auto: Lịch làm việc chưa bắt đầu.";
                        needUpdate = true;
                    }
                }
            }

            // Save changes nếu cần cập nhật thông tin lịch làm việc
            if (needUpdate)
            {
                _db.Lichsulamviecs.UpdateRange(schedules);
                await _db.SaveChangesAsync();
            }
        }


        #region [Just Method]
        private async Task<List<CaKipDTO>> GetData()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Lấy danh sách ca kíp từ Database
            var dataOrigin = await _db.Cakips.AsNoTracking().ToListAsync();
            if (dataOrigin == null || dataOrigin.Count == 0)
            {
                throw new Exception("Không tìm thấy ca kíp.");
            }

            // Lấy lịch làm việc
            var schedules = await _db.Lichsulamviecs
                .Where(ls => ls.NgayThangNam <= today)
                .ToListAsync();

            // Lấy mã nhân viên từ lịch làm việc
            var maNhanVienList = schedules.Select(ls => ls.MaNv).Distinct().ToList();

            // Truy vấn thông tin nhân viên
            var nhanVienDict = await _db.Nhanviens
                .AsNoTracking()
                .Where(nv => maNhanVienList.Contains(nv.MaNv))
                .ToDictionaryAsync(nv => nv.MaNv, nv => (nv.HoTen, nv.MaChucVu));

            // Tính số lượng nhân viên hiện tại đi làm cho từng ca kíp
            foreach (var caKip in dataOrigin)
            {
                caKip.SoNguoiHienTai = schedules
                    .Count(ls => ls.MaCaKip == caKip.MaCaKip && ls.TrangThai == TrangThaiLichLamViec.DiLam);
            }

            // Chuẩn bị dữ liệu đầu ra qua DTO
            var dataFormatted = dataOrigin.Select(caKip => new CaKipDTO
            {
                MaCaKip = caKip.MaCaKip,
                TenCa = caKip.TenCa,
                SoNguoiToiDa = caKip.SoNguoiToiDa,
                SoNguoiHienTai = caKip.SoNguoiHienTai,
                GioBatDau = caKip.GioBatDau,
                GioKetThuc = caKip.GioKetThuc,
                IsDelete = caKip.IsDelete,
                QrCodeData = QrCodeUtils.GenerateQrCodeData(caKip.MaCaKip, today),
                Schedules = schedules
                    .Where(ls => ls.MaCaKip == caKip.MaCaKip)
                    .Select(ls => new ScheduleDTO
                    {
                        Id = ls.Id,
                        MaNv = ls.MaNv,
                        TenNhanVien = nhanVienDict[ls.MaNv].HoTen,
                        MaCaKip = ls.MaCaKip,
                        TenCa = caKip.TenCa,
                        NgayThangNam = ls.NgayThangNam,
                        GioVao = ls.GioVao,
                        GioRa = ls.GioRa,
                        SoGioLam = ls.SoGioLam,
                        LyDoNghi = ls.LyDoNghi,
                        TongLuong = ls.TongLuong,
                        TrangThai = ls.TrangThai,
                        GhiChu = ls.GhiChu,
                        IsDelete = ls.IsDelete,
                    }).ToList()
            }).ToList();

            return dataFormatted;
        }
        #endregion

    }
}