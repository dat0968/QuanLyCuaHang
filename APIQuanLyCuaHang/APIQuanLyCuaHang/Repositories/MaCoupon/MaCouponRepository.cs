using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
namespace APIQuanLyCuaHang.Repository.MaCoupon
{
    public class MaCouponRepository : IMaCouponRepository
    {
        private readonly QuanLyCuaHangContext db;

        public MaCouponRepository(QuanLyCuaHangContext db)
        {
            this.db = db;
        }
        public CouponDTO Create(CouponDTO maCoupon)
        {
            var newCouponCode = new APIQuanLyCuaHang.Models.MaCoupon
            {
                SoTienGiam = maCoupon.SoTienGiam > 0 ? maCoupon.SoTienGiam : null,
                PhanTramGiam = maCoupon.PhanTramGiam > 0 ? maCoupon.PhanTramGiam : null,
                NgayKetThuc = maCoupon.NgayKetThuc,
                NgayBatDau = maCoupon.NgayBatDau,
                SoLuong = maCoupon.SoLuong,
                TrangThai = true,
                DonHangToiThieu = maCoupon.DonHangToiThieu,
            };
            db.MaCoupons.Add(newCouponCode);
            db.SaveChanges();
            return maCoupon;
        }

        public void Cancel(string id)
        {
            var FindMaCoupon = db.MaCoupons.FirstOrDefault(p => p.MaCode == id);
            if (FindMaCoupon != null)
            {
                FindMaCoupon.TrangThai = false;
                db.SaveChanges();
            }
        }

        public List<CouponDTO> GetAll(string? keywords, string? status, string? sort)
        {
            var listCouponCode = db.MaCoupons.AsQueryable();
            var CovertToListMaCouponVM = new List<CouponDTO>();
            
            if (!string.IsNullOrEmpty(keywords))
            {
                listCouponCode = listCouponCode.Where(p => p.MaCode.Contains(keywords));
            }
            switch (status)
            {
                case "Còn hiệu lực":
                    listCouponCode = listCouponCode.Where(p => p.TrangThai == true && p.NgayKetThuc > DateTime.Now);
                    break;
                case "Đã hủy":
                    listCouponCode = listCouponCode.Where(p => p.TrangThai == false);
                    break;
                case "Đã hết hạn":
                    listCouponCode = listCouponCode.Where(p => p.NgayKetThuc < DateTime.Now);
                    break;   
                default:
                    listCouponCode = listCouponCode.OrderByDescending(p => p.NgayBatDau);
                    break;
            }
            switch (sort)
            {
                case "asc":
                    listCouponCode = listCouponCode.OrderBy(p => p.NgayBatDau);
                    break;
                default:
                    listCouponCode = listCouponCode.OrderByDescending(p => p.NgayBatDau);
                    break;
            }
            foreach (var item in listCouponCode)
            {
                CovertToListMaCouponVM.Add(new CouponDTO
                {
                    MaCode = item.MaCode,
                    PhanTramGiam = item.PhanTramGiam,
                    SoTienGiam = item.SoTienGiam,
                    NgayKetThuc = item.NgayKetThuc,
                    SoLuong = item.SoLuong,
                    SoLuongDaDung = item.SoLuongDaDung,
                    TrangThai = item.TrangThai,
                    NgayBatDau = item.NgayBatDau,
                    DonHangToiThieu = item.DonHangToiThieu,
                });
            }
            return CovertToListMaCouponVM;
        }

        public void Update(CouponDTO maCoupon)
        {
            var editCouponCode = new APIQuanLyCuaHang.Models.MaCoupon
            {
                MaCode = maCoupon.MaCode,
                SoTienGiam = maCoupon.SoTienGiam > 0 ? maCoupon.SoTienGiam : null,
                PhanTramGiam = maCoupon.PhanTramGiam > 0 ? maCoupon.PhanTramGiam : null,
                NgayKetThuc = maCoupon.NgayKetThuc,
                SoLuong = maCoupon.SoLuong,
                TrangThai = maCoupon.TrangThai,
                NgayBatDau = maCoupon.NgayBatDau,
                SoLuongDaDung = maCoupon.SoLuongDaDung,
                DonHangToiThieu = maCoupon.DonHangToiThieu,
            };
            db.MaCoupons.Update(editCouponCode);
            db.SaveChanges();
        }
        public async Task<CouponDTO?> GetById(string macoupon)
        {
            var findCoupon = await db.MaCoupons.AsNoTracking().FirstOrDefaultAsync(p => p.MaCode == macoupon.Trim());
            if(findCoupon != null)
            {
                var CouponDTO = new CouponDTO
                {
                    MaCode = findCoupon.MaCode,
                    SoLuong = findCoupon.SoLuong,
                    SoLuongDaDung = findCoupon.SoLuongDaDung,
                    MoTa = findCoupon.MoTa,
                    PhanTramGiam = findCoupon.PhanTramGiam,
                    SoTienGiam = findCoupon.SoTienGiam,
                    DonHangToiThieu = findCoupon.DonHangToiThieu,
                    NgayBatDau = findCoupon.NgayBatDau,
                    NgayKetThuc = findCoupon.NgayKetThuc,
                    TrangThai = findCoupon.TrangThai,
                };
                return CouponDTO;
            }
            return null;
        }
    }
}
