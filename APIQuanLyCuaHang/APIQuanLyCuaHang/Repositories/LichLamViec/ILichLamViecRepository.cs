using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO;

namespace APIQuanLyCuaHang.Repositories.LichLamViec
{
    public interface ILichLamViecRepository
    {
        Task<ResponseAPI<dynamic>> DangKyCaLamViecAsync(int maNv, int maCaKip, DateOnly ngayLam);
    }
}