using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using static OfficeOpenXml.ExcelErrorValue;

namespace APIQuanLyCuaHang.Repositories.Combo
{
    public class ComboRepository : IComboRepository
    {
        private readonly QuanLyCuaHangContext db;

        public ComboRepository(QuanLyCuaHangContext db)
        {
            this.db = db;
        }
        public async Task<APIQuanLyCuaHang.Models.Combo> AddCombo(APIQuanLyCuaHang.Models.Combo newcombo)
        {
            try
            {
                db.Combos.Add(newcombo);
                await db.SaveChangesAsync();
                return newcombo;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public async Task CancelCombo(int id)
        {
            try
            {
                var findCombo = await db.Combos.FindAsync(id);
                if (findCombo != null)
                {
                    findCombo.IsDelete = true;
                    db.Combos.Update(findCombo);
                    await db.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("This combo not found");
                }
            }catch(Exception ex)
            {
                throw;
            }
                    
        }

        public async Task EditCombo(APIQuanLyCuaHang.Models.Combo model)
        {
            try
            {
                db.Combos.Update(model);
                await db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<List<ComboResponseDTO>> GetAll(string? search)
        {
            try
            {
                var listCombo = await db.Combos.Where(p => p.IsDelete == false).Select(p => new ComboResponseDTO
                {
                    MaCombo = p.MaCombo,
                    TenCombo = p.TenCombo,
                    Hinh = p.Hinh,
                    PhanTramGiam = p.PhanTramGiam,
                    SoTienGiam = p.SoTienGiam,
                    SoLuong = p.SoLuong,
                    MoTa = p.MoTa,
                    IsDelete = p.IsDelete,
                    Chitietcombos = p.Chitietcombos.Select(p => new DetaisComboResponseDTO
                    {
                        MaSp = p.MaSp,
                        TenSp = p.MaSpNavigation.TenSanPham,
                        SoLuongSp = p.SoLuongSp
                    }).ToList()
                }).ToListAsync();
                if (!string.IsNullOrEmpty(search))
                {
                    listCombo = listCombo.Where(p => p.MaCombo.ToString().Contains(search) || p.TenCombo.Contains(search)).ToList();
                };
                return listCombo;
            }catch(Exception ex)
            {
                throw;
            }
           
        }

        public async Task<ComboResponseDTO> GetById(int id)
        {
            try
            {
                var getCombobyID = await db.Combos.Select(p => new ComboResponseDTO
                {
                    MaCombo = p.MaCombo,
                    TenCombo = p.TenCombo,
                    Hinh = p.Hinh,
                    PhanTramGiam = p.PhanTramGiam,
                    SoTienGiam = p.SoTienGiam,
                    SoLuong = p.SoLuong,
                    MoTa = p.MoTa,
                    IsDelete = p.IsDelete,
                    Chitietcombos = p.Chitietcombos.Select(p => new DetaisComboResponseDTO
                    {
                        MaSp = p.MaSp,
                        TenSp = p.MaSpNavigation.TenSanPham,
                        SoLuongSp = p.SoLuongSp
                    }).ToList()
                }).FirstOrDefaultAsync(p => p.MaCombo == id);
                return getCombobyID;
            }catch (Exception ex)
            {
                throw;
            }
            
        }
    }
}
