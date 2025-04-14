namespace APIQuanLyCuaHang.Repositories.DetailMaCoupon
{
    public interface IDetailMaCoupon
    {
        Task<bool> CheckUser_CouponCode(int MaUser, string CouponCode);
        Task AddDetailMacoupon(string MaCoupon, int MaKh);
        
    }
}
