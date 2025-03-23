using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Respositoies.HashPassword;
using APIQuanLyCuaHang.Respositoies.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using APIBanXeDap.ViewModels;
using Microsoft.EntityFrameworkCore;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> Register(RegisterDTO model)
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
        public async Task<IActionResult> LoginCustomer(LoginDTO model)
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
                var Khachhang = new PersonalInformationDTO
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
                    Data = new TokenResponseDTO
                    {
                        AccessToken = AccessToken,
                        RefreshToken = RefreshToken,
                    },
                });
            }
            
        }
        [HttpPost("LoginStaff")]
        public async Task<IActionResult> LoginStaff(LoginDTO model)
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
                var nhanvien = new PersonalInformationDTO
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
                    Data = new TokenResponseDTO
                    {
                        AccessToken = AccessToken,
                        RefreshToken = RefreshToken,
                    },
                });
            }
        }
        [HttpDelete("Logout")]
        public async Task<IActionResult> Logout(string RefreshToken)
        {
            var checkRefreshToken = await db.Refreshtokens.FirstOrDefaultAsync(p => p.Token == RefreshToken);
            if (checkRefreshToken == null)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Logout failed"
                });
            }
            try
            {
                db.Refreshtokens.Remove(checkRefreshToken);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(new
            {
                Success = true,
                Message = "Logout successfully"
            });
        }
        [HttpGet("ForgotPasswordCustomer")]
        public async Task<IActionResult> ForgotPasswordCustomer(string email)
        {
            var checkEmail = await db.Khachhangs.FirstOrDefaultAsync(p => p.Email.Trim() == email);
            if (checkEmail == null)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Email chưa được đăng ký với tài khoản nào",
                    Data = ""
                });
            }
            var newPassword = Guid.NewGuid().ToString("N").Substring(0, 10);
            checkEmail.MatKhau = _hasher.HashPassword(newPassword);
            db.Update(checkEmail);
            await db.SaveChangesAsync();
            try
            {
                await SendEmailAsync(email, "Đặt lại mật khẩu",
                    $"Mật khẩu mới của bạn là: {newPassword}. Vui lòng đăng nhập và đổi mật khẩu ngay sau khi nhận được email này.");
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Đã có lỗi khi gửi email. Vui lòng thử lại sau.",
                    Data = ""
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Yêu cầu đổi mật khẩu mới được chấp nhận. Vui lòng kiểm tra Email của bạn",
                Data = newPassword 
            });
        }
        [HttpGet("ForgotPasswordStaff")]
        public async Task<IActionResult> ForgotPasswordStaff(string email)
        {
            var checkEmail = await db.Nhanviens.FirstOrDefaultAsync(p => p.Email == email);
            if (checkEmail == null)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Email chưa được đăng ký với tài khoản nào",
                    Data = "",
                });
            }

            var newPassword = Guid.NewGuid().ToString("N").Substring(0, 10);
            checkEmail.MatKhau = newPassword;
            db.Update(checkEmail);
            await db.SaveChangesAsync();
            try
            {
                await SendEmailAsync(email, "Đặt lại mật khẩu",
                    $"Mật khẩu mới của bạn là: {newPassword}. Vui lòng đăng nhập và đổi mật khẩu ngay sau khi nhận được email này.");
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Đã có lỗi khi gửi email. Vui lòng thử lại sau.",
                    Data = ""
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Yêu cầu đổi mật khẩu mới được chấp nhận. Vui lòng kiểm tra Email của bạn",
                Data = newPassword 
            });
        }
        private async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("DARK BEE FOOD", "khongbiet12kk@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("khongbiet12kk@gmail.com", "vupb omuo wppx iccu");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
        [HttpPost("RenewAccessToken")]
        public async Task<IActionResult> RenewToken([FromBody] PersonalInformationDTO model)
        {
            var checkRefreshToken = await db.Refreshtokens.AsNoTracking().FirstOrDefaultAsync(p => p.Token == model.RefreshToken);
            var JwtSecurityToken = new JwtSecurityTokenHandler();
            if ((checkRefreshToken == null) || (checkRefreshToken != null && checkRefreshToken.ExpiredAt < DateTime.UtcNow))
            {
                return Ok(new
                {
                    Success = false,
                    Message = "RefreshToken has expired. Login again",
                });
            }
            var information = new PersonalInformationDTO
            {
                Id = model.Id,
                HoTen = model.HoTen,
                SDT = model.SDT,
            };
            var GenerateAccessToken = _tokenServices.GenerateAccessToken(information);
            return Ok(new
            {
                Success = true,
                Message = "Renew AccessToken successfully",
                Data = new TokenResponseDTO
                {
                    AccessToken = GenerateAccessToken,
                    RefreshToken = model.RefreshToken,
                }
            });
        }
        [HttpGet("LoginGoogle")]
        public async Task LoginGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }
        [HttpGet("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);
            var existingUser = db.Khachhangs.FirstOrDefault(u => u.Email == email);
            if (existingUser == null)
            {
                existingUser = new Khachhang
                {
                    HoTen = name,
                    Email = email,
                    TinhTrang = "Đang hoạt động",
                    NgayTao = DateTime.Now,
                    IsDelete = false,
                };
                db.Khachhangs.Add(existingUser);
                db.SaveChanges();
            }
            var model = new PersonalInformationDTO
            {
                Id = existingUser.MaKh,
                HoTen = existingUser.HoTen ?? "",
                SDT = existingUser.Sdt ?? "",
                VaiTro = "Customer"
            };
            var AccessToken = _tokenServices.GenerateAccessToken(model);
            var RefreshToken = _tokenServices.GenerateRefreshToken();
            var AddRefreshTokenDb = new Refreshtoken
            {
                UserId = existingUser.MaKh,
                Token = RefreshToken,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddDays(1),
            };
            db.Refreshtokens.Add(AddRefreshTokenDb);
            await db.SaveChangesAsync();
            return Redirect($"http://localhost:5173/GoogleLoginSuccess?access_token={AccessToken}&refresh_token={RefreshToken}");
            //return Ok(new
            //{   
            //    Success = true,
            //    Message = "Login successfully",
            //    Data = new TokenResponseDTO
            //    {
            //        AccessToken = AccessToken,
            //        RefreshToken = RefreshToken,
            //    },
            //});
        }
    }
}
