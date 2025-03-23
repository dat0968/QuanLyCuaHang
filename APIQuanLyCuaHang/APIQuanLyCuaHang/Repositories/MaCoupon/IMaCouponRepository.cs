using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.DTO;
namespace APIQuanLyCuaHang.Repository.MaCoupon
{
    public interface IMaCouponRepository
    {
        public List<CouponDTO> GetAll(string? keywords, string? status, string? sort);
        public CouponDTO Create(CouponDTO maCoupon);
        public void Update(CouponDTO maCoupon);
        public void Cancel(string id);
    }
}
