using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Repository;

namespace APIQuanLyCuaHang.Repositories.CaKip
{
    public interface ICaKipRepository : IRepository<Cakip>
    {
        Task<ResponseAPI<List<CaKipDTO>>> UpsertCrewAsync(CaKipDTO? caKip);
        Task<ResponseAPI<List<CaKipDTO>>> GetAllAsync();
        Task AutoUpdateAsync();
        Task<ResponseAPI<CaKipDTO>> GetAllEmployeesInShiftAsync(int? maCaKip);
        Task<ResponseAPI<dynamic>> RemoveAsync(int? id);
        Task<ResponseAPI<string>> ChangeStatusAsync(int? id);
    }
}