using APIQuanLyCuaHang.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace APIQuanLyCuaHang.Respositoies.Token
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration configuration;

        public TokenServices(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string GenerateAccessToken(PersonalInformation model)
        {
            var JwtTokenHandler = new JwtSecurityTokenHandler();
            var radomIDToken = Guid.NewGuid();
            var secretKetByte = Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]);
            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, model.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, radomIDToken.ToString()),
                    new Claim("PhoneNumber", model.SDT ?? ""),
                    new Claim("FullName", model.HoTen),
                    new Claim(ClaimTypes.Role, model.VaiTro),
                    new Claim("Avatar", model.Hinh ?? ""),
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKetByte), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = JwtTokenHandler.CreateToken(TokenDescriptor);
            var WritesToken = JwtTokenHandler.WriteToken(token);
            return WritesToken;
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)).TrimEnd('=').Replace('+', '_').Replace('/', '_');
        }


    }
}
