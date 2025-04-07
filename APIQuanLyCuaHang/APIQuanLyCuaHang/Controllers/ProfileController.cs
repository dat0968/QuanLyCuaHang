using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Respositoies;
using APIQuanLyCuaHang.Respositoies.HashPassword;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly QuanLyCuaHangContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(QuanLyCuaHangContext db, IPasswordHasher passwordHasher, IWebHostEnvironment environment)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _environment = environment;
        }

        [HttpGet("GetProfile")]
        //[Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                Console.WriteLine($"User ID from token: {104}");

                var customer = await _db.Khachhangs.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.MaKh == 104);

                if (customer == null)
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                var profile = new KhachhangDTO
                {
                    MaKh = customer.MaKh,
                    HoTen = customer.HoTen,
                    GioiTinh = customer.GioiTinh ?? "Chưa cập nhật",
                    NgaySinh = customer.NgaySinh,
                    DiaChi = customer.DiaChi ?? "Chưa cập nhật",
                    Cccd = customer.Cccd ?? "Chưa cập nhật",
                    Sdt = customer.Sdt ?? "Chưa cập nhật",
                    Email = customer.Email,
                    TenTaiKhoan = customer.TenTaiKhoan ?? "Chưa cập nhật",
                    MatKhau = null,
                    HinhDaiDien = customer.HinhDaiDien,
                    NgayTao = customer.NgayTao,
                    TinhTrang = customer.TinhTrang ?? "Đang hoạt động",
                    IsDelete = customer.IsDelete
                };

                return Ok(new
                {
                    Success = true,
                    Data = profile
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong GetProfile: {ex.Message}");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin hồ sơ",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("UpdateProfile")]
        //[Authorize]
        public async Task<IActionResult> UpdateProfile([FromForm] KhachhangDTO model)
        {
            try
            {
                //string userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                //if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                //{
                //    return Unauthorized(new
                //    {
                //        Success = false,
                //        Message = "Token không hợp lệ hoặc thiếu thông tin userId"
                //    });
                //}

                var customer = await _db.Khachhangs.FirstOrDefaultAsync(p => p.MaKh == model.MaKh);
                if (customer == null)
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                // Cập nhật các trường thông tin giữ nguyên logic cũ
                customer.HoTen = string.IsNullOrEmpty(model.HoTen) ? customer.HoTen : model.HoTen;
                customer.GioiTinh = string.IsNullOrEmpty(model.GioiTinh) ? customer.GioiTinh : model.GioiTinh;
                customer.NgaySinh = model.NgaySinh.HasValue ? model.NgaySinh : customer.NgaySinh;
                customer.DiaChi = string.IsNullOrEmpty(model.DiaChi) ? customer.DiaChi : model.DiaChi;
                customer.Cccd = string.IsNullOrEmpty(model.Cccd) ? customer.Cccd : model.Cccd;
                customer.Sdt = string.IsNullOrEmpty(model.Sdt) ? customer.Sdt : model.Sdt;
                customer.Email = string.IsNullOrEmpty(model.Email) ? customer.Email : model.Email;
                //customer.MatKhau = passwordHasher.HashPassword(model.MatKhau); // nếu muốn cho cập nhật mật khẩu

                // Xử lý hình ảnh giống CustomerController
                if (model.Anh != null && model.Anh.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Anh.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "Hinh", "AnhKhachHang", fileName);
                    var directory = Path.GetDirectoryName(filePath);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Anh.CopyToAsync(stream);
                    }

                    // Xóa ảnh cũ nếu tồn tại
                    if (!string.IsNullOrEmpty(customer.HinhDaiDien))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, customer.HinhDaiDien.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    customer.HinhDaiDien = $"/Hinh/AnhKhachHang/{fileName}";
                }

                _db.Khachhangs.Update(customer);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Cập nhật thông tin thành công"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong UpdateProfile: {ex.Message}");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật thông tin",
                    Error = ex.Message
                });
            }
        }
    }
}