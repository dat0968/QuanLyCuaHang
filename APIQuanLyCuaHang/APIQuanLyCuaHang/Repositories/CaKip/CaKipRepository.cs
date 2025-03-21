using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.CaKip
{
    public class CaKipRepository : ICaKipRepository
    {
        private readonly QuanLyCuaHangContext _db;

        public CaKipRepository(QuanLyCuaHangContext db)
        {
            _db = db;
        }
        public async Task<ResponseAPI<List<CaKipDTO>?>> GetAllAsync()
        {
            ResponseAPI<List<CaKipDTO>?> response = new();
            try
            {
                var data = await _db.Cakips.Select(ck => new CaKipDTO
                {
                    MaCaKip = ck.MaCaKip,
                    SoNguoiToiDa = ck.SoNguoiToiDa,
                    SoNguoiHienTai = ck.SoNguoiHienTai,
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

        public async Task<ResponseAPI<List<CaKipDTO>?>> GetAllTodayAsync()
        {
            ResponseAPI<List<CaKipDTO>?> response = new();
            try
            {
                var data = await _db.Cakips.Where(ck => ck.GioBatDau.Date == DateTime.Today.Date).Select(ck => new CaKipDTO
                {
                    MaCaKip = ck.MaCaKip,
                    SoNguoiToiDa = ck.SoNguoiToiDa,
                    SoNguoiHienTai = ck.SoNguoiHienTai,
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

        public async Task<ResponseAPI<List<CaKipDTO>?>> UpsertCrewAsync(CaKipDTO? caKip, bool? isToday)
        {
            ResponseAPI<List<CaKipDTO>?> response = new();
            try
            {
                if (caKip == null)
                {
                    throw new Exception("Dữ liệu không nhận được để xử lí.");
                }
                if (!caKip.MaCaKip.HasValue || caKip.MaCaKip == 0) // Thêm
                {
                    Cakip newCaKip = new Cakip
                    {
                        SoNguoiToiDa = caKip.SoNguoiToiDa,
                        SoNguoiHienTai = caKip.SoNguoiHienTai,
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
                    // updateCaKip.SoNguoiHienTai = caKip.SoNguoiHienTai;
                    // updateCaKip.GioBatDau = caKip.GioBatDau;
                    updateCaKip.GioKetThuc = caKip.GioKetThuc;
                    updateCaKip.IsDelete = caKip.IsDelete;

                    _db.Update(updateCaKip);
                }
                await _db.SaveChangesAsync();

                var data = await _db.Cakips.Select(ck => new CaKipDTO
                {
                    MaCaKip = ck.MaCaKip,
                    SoNguoiToiDa = ck.SoNguoiToiDa,
                    SoNguoiHienTai = ck.SoNguoiHienTai,
                    GioBatDau = ck.GioBatDau,
                    GioKetThuc = ck.GioKetThuc,
                    IsDelete = ck.IsDelete,
                }).ToListAsync();

                if (isToday.HasValue && isToday.Value)
                {
                    data = data.Where(ck => ck.GioBatDau.Date == DateTime.Today.Date).ToList();
                }

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