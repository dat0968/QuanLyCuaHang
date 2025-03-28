using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.DetailBill
{
    public class DetailBill : IDetailBill
    {
        private readonly QuanLyCuaHangContext db;

        public DetailBill(QuanLyCuaHangContext db)
        {
            this.db = db;
        }
        public async Task<Cthoadon> CreateDetailOrder(Cthoadon model)
        {
            try
            {
                db.Cthoadons.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
