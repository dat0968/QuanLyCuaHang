using APIQuanLyCuaHang.DTO;

namespace APIQuanLyCuaHang.Repositories.Token
{
    public interface ITokenServices
    {
        public string GenerateAccessToken(PersonalInformationDTO model);
        public string GenerateRefreshToken();

    }
}
