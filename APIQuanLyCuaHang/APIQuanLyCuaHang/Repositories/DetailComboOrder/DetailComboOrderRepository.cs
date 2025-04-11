using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace APIQuanLyCuaHang.Repositories.DetailComboOrder
{
    public class DetailComboOrderRepository : IDetailComboOrderRepository
    {
        private readonly QuanLyCuaHangContext db;
        public DetailComboOrderRepository(QuanLyCuaHangContext db)
        {
            this.db = db;
        }
        public async Task<Chitietcombohoadon> AddDetailComboOrder(Chitietcombohoadon model)
        {
            try
            {
                var checkDetailComboOrder = await db.Chitietcombohoadons
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.MaHd == model.MaHd && p.MaCombo == model.MaCombo && p.MaCTSp == model.MaCTSp);
                if(checkDetailComboOrder != null)
                {
                    checkDetailComboOrder.SoLuong += model.SoLuong;
                    db.Chitietcombohoadons.Update(checkDetailComboOrder);
                }
                else
                {
                    db.Chitietcombohoadons.Add(model);
                }              
                await db.SaveChangesAsync();
                return model;
            }catch(Exception ex)
            {
                throw new Exception("Lỗi", ex);
            }
        }
    }
}
