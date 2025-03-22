using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO;

namespace APIQuanLyCuaHang.Repositories.CaKip
{
    public interface ICaKipRepository
    {
        Task<ResponseAPI<List<CaKipDTO>>> UpsertCrewAsync(CaKipDTO? caKip);
        Task<ResponseAPI<List<CaKipDTO>>> GetAllAsync();
        Task<ResponseAPI<dynamic>> RemoveAsync(int? id);
        Task<ResponseAPI<string>> ChangeStatusAsync(int? id);
    }
}