using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.CaKip
{
    public class CaKipRepository(QuanLyCuaHangContext db) : Repository<Cakip>(db), ICaKipRepository
    {
        private readonly QuanLyCuaHangContext _db = db;

        public async Task<ResponseAPI<string>> ChangeStatusAsync(int? id)
        {
            ResponseAPI<string> response = new();
            try
            {
                if (!id.HasValue)
                {
                    throw new Exception("Dữ liệu không nhận được để xử lí.");
                }
                var caKip = await _db.Cakips.FindAsync(id);

                if (caKip == null)
                {
                    throw new Exception("Dữ liệu không tìm thấy trong hệ thống.");
                }

                caKip.IsDelete = !caKip.IsDelete;

                _db.Update(caKip);
                await _db.SaveChangesAsync();

                response.SetSuccessResponse("Đã thay đổi trạng thái Ca thành công.");
                response.SetData("Ok");
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
            }
            return response;
        }

        public async Task<ResponseAPI<List<CaKipDTO>>> GetAllAsync()
        {
            ResponseAPI<List<CaKipDTO>> response = new();
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);

                // Truy vấn dữ liệu ca kíp
                var dataOrigin = await _db.Cakips
                    .AsNoTracking()
                    .ToListAsync();

                if (dataOrigin == null || !dataOrigin.Any())
                {
                    throw new KeyNotFoundException("Không tìm thấy ca kíp.");
                }

                // Truy vấn lịch làm việc
                var lichLamViecs = await _db.Lichsulamviecs
                    .Where(ls => ls.NgayThangNam <= today) // Chỉ lấy lịch làm việc của ngày hôm trước hoặc hôm nay
                    .ToListAsync();

                // Truy vấn danh sách nhân viên
                var maNhanVienList = lichLamViecs
                    .Select(ls => ls.MaNv)
                    .Distinct()
                    .ToList();

                var nhanVienDict = await _db.Nhanviens
                    .AsNoTracking()
                    .Where(nv => maNhanVienList.Contains(nv.MaNv))
                    .ToDictionaryAsync(nv => nv.MaNv, nv => nv.HoTen);

                // Cập nhật thông tin ca làm việc nếu cần
                bool needUpdate = false;
                foreach (var ls in lichLamViecs)
                {
                    var caKip = dataOrigin.FirstOrDefault(ca => ca.MaCaKip == ls.MaCaKip);
                    if (caKip == null) continue;


                    // Cập nhật giờ vào/giờ ra nếu chưa có
                    if (ls.GioVao == null)
                    {
                        ls.GioVao = caKip.GioBatDau;
                        needUpdate = true;
                    }

                    if (ls.GioRa == null)
                    {
                        ls.GioRa = caKip.GioKetThuc;
                        needUpdate = true;
                    }

                    // Tính số giờ làm nếu chưa có dữ liệu
                    if (ls.SoGioLam == 0)
                    {
                        ls.SoGioLam = (caKip.GioKetThuc - caKip.GioBatDau).TotalHours;
                        needUpdate = true;
                    }

                    // Cập nhật tổng lương nếu chưa có dữ liệu
                    if (ls.TongLuong == null)
                    {
                        ls.TongLuong = 0;
                        needUpdate = true;
                    }

                    // Cập nhật trạng thái nếu ca đã kết thúc
                    if (ls.NgayThangNam < today && ls.TrangThai != "Kết thúc ca")
                    {
                        ls.TrangThai = "Kết thúc ca";
                        ls.GhiChu = "Ca làm việc đã kết thúc.";
                        needUpdate = true;
                    }

                }
                if (needUpdate)
                {
                    _db.Lichsulamviecs.UpdateRange(lichLamViecs);
                }

                await _db.SaveChangesAsync();

                // Cập nhật số lượng nhân viên hiện tại của mỗi ca kíp dựa trên trạng thái "Đi làm"
                foreach (var caKip in dataOrigin)
                {
                    caKip.SoNguoiHienTai = lichLamViecs.Count(ls => ls.MaCaKip == caKip.MaCaKip && ls.TrangThai == "Đi làm");
                }

                // Lưu thay đổi nếu có thay đổi dữ liệu
                await _db.SaveChangesAsync();

                // Định dạng dữ liệu ca kíp
                var dataFormatted = dataOrigin.Select(caKip => new CaKipDTO
                {
                    MaCaKip = caKip.MaCaKip,
                    SoNguoiToiDa = caKip.SoNguoiToiDa,
                    SoNguoiHienTai = caKip.SoNguoiHienTai,
                    GioBatDau = caKip.GioBatDau,
                    GioKetThuc = caKip.GioKetThuc,
                    IsDelete = caKip.IsDelete,
                    QrCodeData = $"{caKip.MaCaKip}-{today}",
                    LichLamViecs = lichLamViecs
                        .Where(ls => ls.MaCaKip == caKip.MaCaKip)
                        .Select(ls => new LichLamViecDTO
                        {
                            Id = ls.Id,
                            MaNv = ls.MaNv,
                            TenNhanVien = nhanVienDict.GetValueOrDefault(ls.MaNv),
                            MaCaKip = ls.MaCaKip,
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

                response.SetSuccessResponse("Lấy danh sách Ca thành công!");
                response.SetData(dataFormatted);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
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
                var lichLamViecs = await _db.Lichsulamviecs
                    .AsNoTracking()
                    .Where(ls => ls.MaCaKip == maCaKip && ls.SoGioLam == 0)
                    .ToListAsync();

                // Truy vấn danh sách nhân viên để giảm số lần truy vấn
                var maNhanVienList = lichLamViecs.Select(ls => ls.MaNv).Distinct().ToList();
                var nhanVienDict = await _db.Nhanviens
                    .AsNoTracking()
                    .Where(nv => maNhanVienList.Contains(nv.MaNv))
                    .ToDictionaryAsync(nv => nv.MaNv, nv => nv.HoTen);

                // Định dạng dữ liệu lịch làm việc
                var lichLamViecDtos = lichLamViecs.Select(ls => new LichLamViecDTO
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
                }).ToList();

                // Định dạng dữ liệu cho phản hồi
                var dataFormatted = new CaKipDTO
                {
                    MaCaKip = dataOrigin.MaCaKip,
                    SoNguoiToiDa = dataOrigin.SoNguoiToiDa,
                    SoNguoiHienTai = dataOrigin.SoNguoiHienTai,
                    GioBatDau = dataOrigin.GioBatDau,
                    GioKetThuc = dataOrigin.GioKetThuc,
                    IsDelete = dataOrigin.IsDelete,
                    QrCodeData = $"{dataOrigin.MaCaKip}-{today}",
                    LichLamViecs = lichLamViecDtos
                };

                response.SetSuccessResponse("Lấy danh sách Ca thành công!");
                response.SetData(dataFormatted);
            }
            catch (KeyNotFoundException ex)
            {
                response.SetMessageResponseWithException(404, ex);
            }
            catch (ArgumentException ex)
            {
                response.SetMessageResponseWithException(400, ex);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
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
                _db.Remove(targetRemove);
                await _db.SaveChangesAsync();
                response.SetSuccessResponse("Lấy danh sách Ca thành công!");

            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
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
                response.SetMessageResponseWithException(500, ex);
            }
            return response;
        }
    }
}