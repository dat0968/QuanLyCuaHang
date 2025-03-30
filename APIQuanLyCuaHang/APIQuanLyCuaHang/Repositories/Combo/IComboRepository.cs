using APIQuanLyCuaHang.DTO;

namespace APIQuanLyCuaHang.Repositories.Combo
{
    public interface IComboRepository
    {
        Task<List<ComboResponseDTO>> GetAll(string? search);
        Task<ComboResponseDTO> GetById(int id);
        Task<APIQuanLyCuaHang.Models.Combo> AddCombo(APIQuanLyCuaHang.Models.Combo newcombo);
        Task EditCombo(APIQuanLyCuaHang.Models.Combo model);
        Task CancelCombo(int id);
    }
}
