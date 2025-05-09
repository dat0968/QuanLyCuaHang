﻿
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.DetailMaCoupon
{
    public class DetailMaCoupon : IDetailMaCoupon
    {
        private readonly QuanLyCuaHangContext db;

        public DetailMaCoupon(QuanLyCuaHangContext db)
        {
            this.db = db;
        }

        public async Task AddDetailMacoupon(string MaCoupon, int MaKh)
        {
            try
            {
                var newDetailMaCoupon = new ChitietmaCoupon
                {
                    MaCode = MaCoupon,
                    MaKh = MaKh,
                    NgaySuDung = DateTime.Now,
                };
                db.ChitietmaCoupons.Add(newDetailMaCoupon);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi", ex);
            }
        }

        public async Task<bool> CheckUser_CouponCode(int MaUser, string CouponCode)
        {
            var check = await db.ChitietmaCoupons.AsNoTracking().FirstOrDefaultAsync(p => p.MaKh == MaUser && p.MaCode == CouponCode.Trim());
            if(check != null)
            {
                return false;
            }
            return true;
        }

       
    }
}
