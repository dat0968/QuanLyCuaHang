using APIQuanLyCuaHang.DTO;

namespace APIQuanLyCuaHang.Repositories.Category
{
    public interface ICategory
    {
        Task<List<CategoryResponseDTO>> GetAll();
    }
}
