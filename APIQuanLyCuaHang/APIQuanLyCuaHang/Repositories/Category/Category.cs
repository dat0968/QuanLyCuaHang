using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.Category
{
    public class Category : ICategory
    {
        private readonly QuanLyCuaHangContext db;

        public Category(QuanLyCuaHangContext db)
        {
            this.db = db;
        }
        public async Task<List<CategoryResponseDTO>> GetAll()
        {
            try
            {
                var ListCategories = await db.Danhmucs
                                .Where(p => p.IsDelete == false)
                                 .Select(p => new CategoryResponseDTO
                                 {
                                     MaDanhMuc = p.MaDanhMuc,
                                     TenDanhMuc = p.TenDanhMuc,
                                     IsDelete = p.IsDelete,
                                 })
                                 .ToListAsync();
                return ListCategories;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
