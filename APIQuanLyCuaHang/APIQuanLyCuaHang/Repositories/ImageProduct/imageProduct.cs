using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.ImageProduct
{
    public class imageProduct : IimageProduct
    {
        private readonly QuanLyCuaHangContext db;

        public imageProduct(QuanLyCuaHangContext db)
        {
            this.db = db;
        }
        public async Task<Hinhanh> AddImageProduct(Hinhanh imageProduct)
        {
            try
            {
                db.Hinhanhs.Add(imageProduct);
                await db.SaveChangesAsync();
                return imageProduct;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task DeleteImageProductByMaCtSp(int id)
        {
            try
            {
                var FindImage = db.Hinhanhs.Where(p => p.MaCtsp == id);
                db.Hinhanhs.RemoveRange(FindImage);
                await db.SaveChangesAsync();
            }catch (Exception ex)
            {
                throw;
            }                    
        }
    }
}
