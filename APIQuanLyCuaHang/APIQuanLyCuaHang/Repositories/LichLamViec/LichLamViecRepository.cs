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

        public async Task<ResponseAPI<dynamic>> DangKyCaLamViecAsync(int maNv, int maCaKip, DateOnly? ngayLam)
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

                if (caKip.IsDelete!.Value)
                {
                    throw new Exception("Ca kíp đã bị vô hiệu hóa để đăng kí làm việc.");
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
                    NgayThangNam = ngayLam.HasValue ? ngayLam.Value : DateOnly.FromDateTime(DateTime.Today),
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
        public async Task<ResponseAPI<dynamic>> ChamCongAsync(int maNv, string qrCodeData)
        {
            ResponseAPI<dynamic> response = new();

            try
            {
                Console.WriteLine(qrCodeData);
                string[] parts = qrCodeData.Split('-');
                if (parts.Length != 4) throw new Exception("QR Code không hợp lệ.");

                if (!int.TryParse(parts[0], out int maCaKip))
                    throw new Exception("Mã ca kíp không hợp lệ.");

                // Sửa định dạng ngày tháng
                string ngayLamStr = $"20{parts[1]}-{parts[2]}-{parts[3]}"; // Chuyển "21-03-25" -> "2021-03-25"
                if (!DateOnly.TryParseExact(ngayLamStr, "yyyy-MM-dd", out DateOnly ngayLam))
                    throw new Exception("Ngày làm không hợp lệ.");

                Console.WriteLine(ngayLamStr);
                var caKip = await _db.Cakips.FindAsync(maCaKip);
                if (caKip == null) throw new Exception("Ca kíp không hợp lệ.");

                var lichLam = await _db.Lichsulamviecs
                    .FirstOrDefaultAsync(l => l.MaNv == maNv && l.MaCaKip == maCaKip && l.NgayThangNam == ngayLam);

                if (lichLam == null)
                {
                    // Check-in lần đầu
                    lichLam = new Lichsulamviec
                    {
                        MaNv = maNv,
                        MaCaKip = maCaKip,
                        NgayThangNam = ngayLam,
                        SoGioLam = 0,
                        IsDelete = false
                    };
                    await _db.Lichsulamviecs.AddAsync(lichLam);

                    response.SetSuccessResponse("Check-in thành công.");
                }
                else
                {
                    // Check-out

                    DateTime today = DateTime.Today;
                    // Tạo thời gian bắt đầu ca làm việc
                    DateTime gioBatDau = new DateTime(today.Year, today.Month, today.Day,
                                                      caKip.GioBatDau.Hour, caKip.GioBatDau.Minute, caKip.GioBatDau.Second);

                    // Nếu ca làm việc có thể qua đêm, điều chỉnh cho chính xác
                    if (caKip.GioBatDau.Hour >= 22 && DateTime.Now.Hour <= 6)
                    {
                        gioBatDau = gioBatDau.AddDays(-1);
                    }

                    double soGioLam = (DateTime.Now - gioBatDau).TotalHours;
                    lichLam.SoGioLam = Math.Max(soGioLam, 0); // Đảm bảo không bị âm


                    response.SetSuccessResponse("Check-out thành công.");
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
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
                response.SetMessageResponseWithException(500, ex);
            }
            return response;
        }
    }
}
