using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.DetailCombo
{
    public interface IDetailCombo
    {
        Task AddDetailCombo(Chitietcombo model);
        Task EditDetailCombo(Chitietcombo model);
        Task DeleteDetailComboByMaCombo(int MaCombo);
    }
}
