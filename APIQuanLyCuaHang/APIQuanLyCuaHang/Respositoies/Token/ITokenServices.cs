using APIQuanLyCuaHang.DTO;

namespace APIQuanLyCuaHang.Respositoies.Token
{
    public interface ITokenServices
    {
        public string GenerateAccessToken(PersonalInformationDTO model);
        public string GenerateRefreshToken();

    }
}
