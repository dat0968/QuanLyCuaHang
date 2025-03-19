using APIQuanLyCuaHang.DTO;

namespace APIQuanLyCuaHang.Respositoies.Token
{
    public interface ITokenServices
    {
        public string GenerateAccessToken(PersonalInformation model);
        public string GenerateRefreshToken();

    }
}
