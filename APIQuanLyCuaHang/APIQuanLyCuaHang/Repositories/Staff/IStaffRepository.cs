using APIQuanLyCuaHang.DTO;

namespace APIQuanLyCuaHang.Repositories.Staff
{
    public interface IStaffRepository
    {
        Task<List<NhanvienDTO>> GetAllStaff(string? search, string? gioiTinh, string? tinhTrang);
        Task<NhanvienDTO> GetStaffById(int id);
        Task<NhanvienDTO> AddStaff(NhanvienDTO staffDto);
        Task<NhanvienDTO> UpdateStaff(int id, NhanvienDTO staffDto);
        Task<bool> ToggleDeleteStaff(int id);
    }
}
