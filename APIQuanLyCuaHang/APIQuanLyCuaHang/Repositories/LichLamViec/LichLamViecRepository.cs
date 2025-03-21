using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Repositories.LichLamViec
{
    public class LichLamViecRepository : ILichLamViecRepository
    {
        private readonly QuanLyCuaHangContext _db;

        public LichLamViecRepository(QuanLyCuaHangContext db)
        {
            _db = db;
        }

        public async Task<ResponseAPI<dynamic>> DangKyCaLamViecAsync(int maNv, int maCaKip, DateOnly ngayLam)
        {
            ResponseAPI<dynamic> response = new();

            try
            {
                var caKip = await _db.Cakips.FindAsync(maCaKip);
                if (caKip == null || caKip.IsDelete == true)
                {
                    throw new Exception("Ca kíp không hợp lệ hoặc đã bị xóa.");
                }

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

                var lichLamViec = new Lichsulamviec
                {
                    MaNv = maNv,
                    MaCaKip = maCaKip,
                    NgayThangNam = ngayLam,
                    SoGioLam = (caKip.GioKetThuc - caKip.GioBatDau).TotalHours,
                    TongLuong = null,
                    IsDelete = false
                };

                await _db.Lichsulamviecs.AddAsync(lichLamViec);
                caKip.SoNguoiHienTai++;

                await _db.SaveChangesAsync();
                response.SetSuccessResponse("Đăng ký thành công!");
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
            }

            return response;
        }
    }
}
