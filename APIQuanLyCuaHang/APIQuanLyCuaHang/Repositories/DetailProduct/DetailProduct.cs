﻿using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.DetailProduct
{
    public class DetailProduct : IDetailProduct
    {
        private readonly QuanLyCuaHangContext db;

        public DetailProduct(QuanLyCuaHangContext db)
        {
            this.db = db;
        }
        public async Task<Chitietsanpham?> GetDetailByMaCTSp(int MaCTSp)
        {
            var findDetailProduct = await db.Chitietsanphams.AsNoTracking()
                .Include(p => p.MaSpNavigation)
                .FirstOrDefaultAsync(p => p.MaCtsp == MaCTSp);
            return findDetailProduct;
        }
        public async Task<Chitietsanpham> AddDetailProduct(Chitietsanpham model)
        {
            try
            {
                db.Chitietsanphams.Add(model);
                await db.SaveChangesAsync();
                return model;
            }
            catch(Exception ex)
            {
                throw;
            }           
        }

        public async Task UpdateDetailProduct(Chitietsanpham model)
        {
            try
            {
                // Ngắt theo dõi thực thể cũ nếu có
                var trackedEntity = db.ChangeTracker
                    .Entries<Chitietsanpham>()
                    .FirstOrDefault(e => e.Entity.MaCtsp == model.MaCtsp);

                if (trackedEntity != null)
                {
                    db.Entry(trackedEntity.Entity).State = EntityState.Detached;
                }
                // Ngắt theo dõi thực thể Sanpham trùng MaSp nếu có
                if (model.MaSp != null)
                {
                    var trackedSanpham = db.ChangeTracker
                        .Entries<Sanpham>()
                        .FirstOrDefault(e => e.Entity.MaSp == model.MaSp);

                    if (trackedSanpham != null)
                    {
                        db.Entry(trackedSanpham.Entity).State = EntityState.Detached;
                    }
                }
                db.Chitietsanphams.Update(model);
                await db.SaveChangesAsync();
            }catch(Exception ex)
            {
                throw new Exception("Lỗi", ex);
            }
        }

        public async Task<List<Chitietsanpham>> GetDetailProductByMaSP(int MaSp)
        {
            try
            {
                var FindDetailProduct = db.Chitietsanphams.AsNoTracking().Where(p => p.MaSp == MaSp).ToListAsync();
                return await FindDetailProduct;
            }catch(Exception ex)
            {
                throw new Exception("Lỗi", ex);
            }
        }
        public async Task DeleteDetailProduct(int MaCtsp)
        {
            try
            {
                var GetDetailProduct = await db.Chitietsanphams.FindAsync(MaCtsp);
                GetDetailProduct.IsDelete = true;
                db.Chitietsanphams.Update(GetDetailProduct);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Chitietsanpham?> CheckExistDetailProduct(int MaSp, string? KichThuoc, string? HuongVi)
        {
            var findDetailProduct = await db.Chitietsanphams.AsNoTracking()
            .FirstOrDefaultAsync(p => p.MaSp == MaSp &&
                (p.KichThuoc ?? "").ToLower() == (KichThuoc ?? "").ToLower() &&
                (p.HuongVi ?? "").ToLower() == (HuongVi ?? "").ToLower());
            if(findDetailProduct == null)
            {
                return null;
            }
            return findDetailProduct;
        }
    }
}
