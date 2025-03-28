using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.Bill
{
    public class BillRepository : IBillRepository
    {
        private readonly QuanLyCuaHangContext db;
        public BillRepository(QuanLyCuaHangContext db)
        {
            this.db = db;
        }
        public async Task<Hoadon> CreateOrder(Hoadon hoadon)
        {
            try
            {
                db.Hoadons.Add(hoadon);
                await db.SaveChangesAsync();
                return hoadon;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
