using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.DTO.Requests;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Repositories.LichLamViec
{
    public class LichLamViecRepository(QuanLyCuaHangContext db) : Repository<Lichsulamviec>(db), ILichLamViecRepository
    {
        private readonly QuanLyCuaHangContext _db = db;
        public async Task<ResponseAPI<dynamic>> DangKyCaLamViecAsync(int? maNv, int maCaKip, DateOnly? ngayLam)
        {
            ResponseAPI<dynamic> response = new();

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
                    MaNv = maNv.Value,
                    MaCaKip = maCaKip,
                    NgayThangNam = ngayLam ?? DateOnly.FromDateTime(DateTime.Today),
                    SoGioLam = (caKip.GioKetThuc - caKip.GioBatDau).TotalHours,
                    TongLuong = null,
                    IsDelete = false,
                    TrangThai = "Chờ xác nhận"
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
                string[] parts = qrCodeData.Split('-');
                if (parts.Length != 4) throw new Exception("QR Code không hợp lệ.");

                if (!int.TryParse(parts[0], out int maCaKip))
                    throw new Exception("Mã ca kíp không hợp lệ.");

                string ngayLamStr = $"20{parts[1]}-{parts[2]}-{parts[3]}";
                if (!DateOnly.TryParseExact(ngayLamStr, "yyyy-MM-dd", out DateOnly ngayLam))
                    throw new Exception("Ngày làm không hợp lệ.");

                var caKip = await _db.Cakips.FindAsync(maCaKip);
                if (caKip == null) throw new Exception("Ca kíp không hợp lệ.");

                var lichLam = await _db.Lichsulamviecs
                    .FirstOrDefaultAsync(l => l.MaNv == maNv && l.MaCaKip == maCaKip && l.NgayThangNam == ngayLam);

                if (lichLam == null)
                {
                    lichLam = new Lichsulamviec
                    {
                        MaNv = maNv,
                        MaCaKip = maCaKip,
                        NgayThangNam = ngayLam,
                        SoGioLam = 0,
                        IsDelete = false,
                        TrangThai = "Không được xác nhận"
                    };
                    await _db.Lichsulamviecs.AddAsync(lichLam);
                    response.SetSuccessResponse("Check-in không hợp lệ.");
                }
                else
                {
                    if (lichLam.TrangThai == "Chờ xác nhận")
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
                    lichLam.TrangThai = soGioLam >= 0.5 ? "Kết thúc ca" : "Trễ";

                    response.SetSuccessResponse(soGioLam >= 0.5 ? "Check-out thành công." : "Bạn đã trễ.");
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
        public async Task<ResponseAPI<dynamic>> SetStatusList(SetStatusListRequest request)
        {
            ResponseAPI<dynamic> response = new();
            try
            {
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

                var lichLamViecs = await _db.Lichsulamviecs
                    .Where(ls => request.MaNvs.Contains(ls.MaNv) && ls.MaCaKip == request.MaCaKip)
                    .ToListAsync();

                if (!lichLamViecs.Any())
                {
                    throw new KeyNotFoundException("Không tìm thấy lịch làm việc phù hợp.");
                }

                // Kiểm tra số lượng nhân viên trong CaKip
                int currentEmployees = await _db.Lichsulamviecs
                    .CountAsync(ls => ls.MaCaKip == request.MaCaKip);

                if (currentEmployees + request.MaNvs.Length > caKip.SoNguoiToiDa)
                {
                    throw new InvalidOperationException($"Số lượng nhân viên vượt quá giới hạn {caKip.SoNguoiToiDa} của CaKip.");
                }

                // Cập nhật trạng thái và giờ làm nếu trạng thái là "Đi làm"
                TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);
                lichLamViecs.ForEach(ls =>
                {
                    ls.TrangThai = request.TrangThaiCapNhap;
                    if (request.TrangThaiCapNhap == "Đi làm")
                    {
                        ls.GioVao = now;
                    }
                });

                await _db.SaveChangesAsync();

                response.SetSuccessResponse($"Đã cập nhật trạng thái {request.TrangThaiCapNhap} cho {lichLamViecs.Count} nhân viên.");
                response.SetData(lichLamViecs);
            }
            catch (ArgumentException ex)
            {
                response.SetMessageResponseWithException(400, ex);
            }
            catch (KeyNotFoundException ex)
            {
                response.SetMessageResponseWithException(404, ex);
            }
            catch (InvalidOperationException ex)
            {
                response.SetMessageResponseWithException(409, ex);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
            }

            return response;
        }
        public async Task<ResponseAPI<dynamic>> SetStatusOne(SetStatusOneRequest request)
        {
            ResponseAPI<dynamic> response = new();
            try
            {
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

                var lichLamViec = await _db.Lichsulamviecs
                    .FirstOrDefaultAsync(ls => ls.MaNv == request.MaNv && ls.MaCaKip == request.MaCaKip);

                if (lichLamViec == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy lịch làm việc phù hợp.");
                }

                // Cập nhật trạng thái và giờ làm nếu trạng thái là "Đi làm"
                if (request.TrangThaiCapNhap == "Đi làm")
                {
                    lichLamViec.GioVao = TimeOnly.FromDateTime(DateTime.Now);
                }

                lichLamViec.TrangThai = request.TrangThaiCapNhap;

                await _db.SaveChangesAsync();

                response.SetSuccessResponse($"Đã cập nhật trạng thái {request.TrangThaiCapNhap} cho nhân viên {request.MaNv}.");
                response.SetData(lichLamViec);
            }
            catch (ArgumentException ex)
            {
                response.SetMessageResponseWithException(400, ex);
            }
            catch (KeyNotFoundException ex)
            {
                response.SetMessageResponseWithException(404, ex);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
            }

            return response;
        }
    }
}
