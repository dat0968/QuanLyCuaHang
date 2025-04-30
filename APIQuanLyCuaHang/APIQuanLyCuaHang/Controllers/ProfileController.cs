using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.HashPassword;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

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
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var userIdClaim = identity?.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Không tìm thấy userId trong token"
                    });
                }

                var customer = await _db.Khachhangs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.MaKh == userId);

                if (customer == null)
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = $"Không tìm thấy khách hàng với MaKh = {userId}"
                    });
                }

                var profile = new KhachhangDTO
                {
                    MaKh = customer.MaKh,
                    HoTen = customer.HoTen,
                    GioiTinh = customer.GioiTinh,
                    NgaySinh = customer.NgaySinh,
                    DiaChi = customer.DiaChi,
                    Cccd = customer.Cccd,
                    Sdt = customer.Sdt,
                    Email = customer.Email,
                    TenTaiKhoan = customer.TenTaiKhoan,
                    MatKhau = null,
                    HinhDaiDien = customer.HinhDaiDien,
                    NgayTao = customer.NgayTao,
                    TinhTrang = customer.TinhTrang ?? "Đang hoạt động",
                    IsDelete = customer.IsDelete
                };

                return Ok(new
                {
                    Success = true,
                    Message = "Lấy thông tin hồ sơ thành công",
                    Data = profile
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin hồ sơ",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("UpdateProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromForm] KhachhangDTO khachhang)
        {
            try
            {
                Console.WriteLine("Nhận được request UpdateProfile...");
                Console.WriteLine($"Dữ liệu nhận được: HoTen={khachhang.HoTen}, Sdt={khachhang.Sdt}, Email={khachhang.Email}");

                // Validation dữ liệu
                if (string.IsNullOrWhiteSpace(khachhang.HoTen))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Họ tên không được để trống"
                    });
                }
                if (string.IsNullOrWhiteSpace(khachhang.GioiTinh))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Giới tính không được để trống"
                    });
                }
                if (string.IsNullOrWhiteSpace(khachhang.DiaChi))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Địa chỉ không được để trống"
                    });
                }
                if (string.IsNullOrWhiteSpace(khachhang.Cccd) || !Regex.IsMatch(khachhang.Cccd, @"^\d{12}$"))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "CCCD phải là 12 chữ số"
                    });
                }
                if (string.IsNullOrWhiteSpace(khachhang.Sdt) || !Regex.IsMatch(khachhang.Sdt, @"^0\d{9,10}$"))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Số điện thoại phải bắt đầu bằng 0 và có 10-11 chữ số"
                    });
                }
                if (!string.IsNullOrWhiteSpace(khachhang.Email) && !Regex.IsMatch(khachhang.Email, @"^[\w-.]+@([\w-]+\.)+[\w-]{2,4}$"))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Email không hợp lệ"
                    });
                }

                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var userIdClaim = identity?.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Không tìm thấy userId trong token"
                    });
                }

                var existingKh = await _db.Khachhangs.FirstOrDefaultAsync(p => p.MaKh == userId);
                if (existingKh == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = $"Không tìm thấy khách hàng với MaKh = {userId}"
                    });
                }

                existingKh.HoTen = khachhang.HoTen;
                existingKh.GioiTinh = khachhang.GioiTinh;
                existingKh.NgaySinh = khachhang.NgaySinh;
                existingKh.DiaChi = khachhang.DiaChi;
                existingKh.Cccd = khachhang.Cccd;
                existingKh.Sdt = khachhang.Sdt;
                existingKh.Email = khachhang.Email;

                if (khachhang.Anh != null && khachhang.Anh.Length > 0)
                {
                    Console.WriteLine("Bắt đầu xử lý file ảnh...");
                    if (khachhang.Anh.Length > 5 * 1024 * 1024) // 5MB
                    {
                        return BadRequest(new
                        {
                            Success = false,
                            Message = "Kích thước file không được vượt quá 5MB"
                        });
                    }

                    var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(khachhang.Anh.FileName).ToLower();
                    if (!validExtensions.Contains(extension))
                    {
                        return BadRequest(new
                        {
                            Success = false,
                            Message = "Chỉ hỗ trợ file .jpg, .jpeg, .png"
                        });
                    }

                    var uploadsFolder = Path.Combine(_environment.WebRootPath ?? "wwwroot", "images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Console.WriteLine("Tạo thư mục uploads...");
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = $"{Guid.NewGuid()}_{khachhang.Anh.FileName}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await khachhang.Anh.CopyToAsync(stream);
                    }

                    existingKh.HinhDaiDien = $"/images/{fileName}";
                }

                await _db.SaveChangesAsync();
                Console.WriteLine("Cập nhật thành công!");
                return Ok(new { Success = true, Message = "Cập nhật thông tin thành công" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật hồ sơ: {ex.Message}\nStackTrace: {ex.StackTrace}");
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