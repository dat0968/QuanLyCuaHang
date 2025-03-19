using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Respositoies.HashPassword;
using APIQuanLyCuaHang.Respositoies.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using APIBanXeDap.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Controllers
{
    public class AccountController : Controller
    {
        private readonly IPasswordHasher _hasher;
        private readonly QuanLyCuaHangContext db;
        private readonly ITokenServices _tokenServices;
        public AccountController(QuanLyCuaHangContext db, ITokenServices TokenServices, IPasswordHasher passwordHasher)
        {
            this._tokenServices = TokenServices;
            this.db = db;
            this._hasher = passwordHasher;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(Register model)
        {
            string hashPass = _hasher.HashPassword(model.MatKhau);
            try
            {
                var AddCustomerDb = new Khachhang
                {
                    HoTen = model.HoTen,
                    TenTaiKhoan = model.TenTaiKhoan,
                    Email = model.Email,
                    MatKhau = hashPass,
                    NgayTao = DateTime.Now,
                    IsDelete = false,
                    TinhTrang = "Đang hoạt động",
                };
                db.Khachhangs.Add(AddCustomerDb);
                await db.SaveChangesAsync();
                return Ok(new
                {
                    Success = true,
                    Message = "Register successfully",
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Message = ex.Message.ToString(),
                });
            }

        }
        [HttpPost("LoginCustomer")]
        public async Task<IActionResult> LoginCustomer(Login model)
        {
            var findUser = db.Khachhangs.AsNoTracking().FirstOrDefault(p => (p.TenTaiKhoan.Trim().ToLower() == model.Email_TenTaiKhoan.Trim().ToLower()) || (p.Email.Trim().ToLower() == model.Email_TenTaiKhoan.Trim().ToLower()));
            if (findUser == null)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Tài khoản không tồn tại"
                });
            }
            else
            {
                if(findUser.TinhTrang?.Trim().ToLower() != "Đang hoạt động".Trim().ToLower())
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = "Tài khoản đang bị tạm khóa"
                    });
                }
                bool isPasswordValid = _hasher.VerifyPassword(model.MatKhau, findUser.MatKhau.Trim());
                if (!isPasswordValid)
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = "Sai mật khẩu"
                    });
                }
                var Khachhang = new PersonalInformation
                {
                    Id = findUser.MaKh,
                    HoTen = findUser.HoTen,
                    SDT = findUser.Sdt,
                    VaiTro = "Customer",
                    Hinh = findUser.HinhDaiDien ?? null,
                };
                var AccessToken = _tokenServices.GenerateAccessToken(Khachhang);
                var RefreshToken = _tokenServices.GenerateRefreshToken();
                var AddRefreshTokenDb = new Refreshtoken
                {
                    UserId = findUser.MaKh,
                    Token = RefreshToken,
                    IssuedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.AddDays(1),
                };
                db.Refreshtokens.Add(AddRefreshTokenDb);
                await db.SaveChangesAsync();
                Khachhang.RefreshToken = RefreshToken;
                return Ok(new
                {
                    Success = true,
                    Message = "Login successfully",
                    //IDCustomer = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub),
                    Data = new TokenResponse
                    {
                        AccessToken = AccessToken,
                        RefreshToken = RefreshToken,
                    },
                });
            }
            
        }
        [HttpPost("LoginStaff")]
        public async Task<IActionResult> LoginStaff(Login model)
        {
            var findUser = db.Nhanviens.AsNoTracking().FirstOrDefault(p => (p.TenTaiKhoan.Trim().ToLower() == model.Email_TenTaiKhoan.Trim().ToLower()) || (p.Email.Trim().ToLower() == model.Email_TenTaiKhoan.Trim().ToLower()));
            if (findUser == null)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Tài khoản không tồn tại"
                });
            }
            else
            {
                if (findUser.TinhTrang?.Trim().ToLower() != "Đang hoạt động".Trim().ToLower())
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = "Tài khoản đang bị tạm khóa"
                    });
                }
                if (findUser.MatKhau?.Trim().ToLower() != model.MatKhau.Trim().ToLower())
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = "Sai mật khẩu"
                    });
                }
                var cv = db.Chucvus.FirstOrDefault(p => p.MaChucVu == findUser.MaChucVu);
                string tenCv = "";
                if (cv != null) { 
                    tenCv = cv.TenChucVu.Trim();
                }
                var nhanvien = new PersonalInformation
                {
                    Id = findUser.MaNv,
                    HoTen = findUser.HoTen,
                    SDT = findUser.Sdt,
                    VaiTro = tenCv,
                    Hinh = null 
                };
                var AccessToken = _tokenServices.GenerateAccessToken(nhanvien);
                var RefreshToken = _tokenServices.GenerateRefreshToken();
                var AddRefreshTokenDb = new Refreshtoken
                {
                    
                    UserId = findUser.MaNv,
                    Token = RefreshToken,
                    IssuedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.AddDays(1),
                };
                db.Refreshtokens.Add(AddRefreshTokenDb);
                await db.SaveChangesAsync();
                nhanvien.RefreshToken = RefreshToken;
                return Ok(new
                {
                    Success = true,
                    Message = "Login successfully",
                    Data = new TokenResponse
                    {
                        AccessToken = AccessToken,
                        RefreshToken = RefreshToken,
                    },
                });
            }
        }
    }
}
