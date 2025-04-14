using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Bill;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.AtCounterBill
{
    public class AtCounterBillRepository : IAtCounterBillRepository
    {
        private readonly QuanLyCuaHangContext _db;

        public AtCounterBillRepository(QuanLyCuaHangContext db)
        {
            _db = db;
        }

        public async Task<Hoadon> CreateOrder(Hoadon hoadon)
        {
            try
            {
                _db.Hoadons.Add(hoadon);
                await _db.SaveChangesAsync();
                return hoadon;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo hóa đơn: " + ex.Message);
            }
        }

        public async Task<Cthoadon> CreateDetailOrder(Cthoadon cthoadon)
        {
            try
            {
                _db.Cthoadons.Add(cthoadon);
                await _db.SaveChangesAsync();
                return cthoadon;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo chi tiết hóa đơn: " + ex.Message);
            }
        }

        public async Task<HoaDonDTOWithDetails?> GetBillDetails(int maHd)
        {
            var bill = await _db.Hoadons
                .Include(hd => hd.Cthoadons)
                .ThenInclude(ct => ct.MaCtspNavigation)
                .ThenInclude(ctsp => ctsp.Hinhanhs)
                .Include(hd => hd.Cthoadons)
                .ThenInclude(ct => ct.MaCtspNavigation)
                .ThenInclude(ctsp => ctsp.MaSpNavigation)
                .Where(hd => hd.MaHd == maHd)
                .FirstOrDefaultAsync();

            if (bill == null) return null;

            return new HoaDonDTOWithDetails
            {
                MaHd = bill.MaHd,
                MaKh = bill.MaKh,
                MaNv = bill.MaNv,
                DiaChiNhanHang = bill.DiaChiNhanHang,
                NgayTao = bill.NgayTao,
                BatDauGiao = bill.BatDauGiao,
                NgayNhan = bill.NgayNhan,
                NgayThanhToan = bill.NgayThanhToan,
                HinhThucTt = bill.HinhThucTt,
                TinhTrang = bill.TinhTrang,
                MoTa = bill.MoTa,
                HoTenNguoiNhan = bill.HoTen,
                Sdt = bill.Sdt,
                LyDoHuy = bill.LyDoHuy,
                PhiVanChuyen = bill.PhiVanChuyen,
                TienGoc = bill.TienGoc,
                Tongtien = bill.TienGoc + bill.PhiVanChuyen,
                ChiTietHoaDon = bill.Cthoadons.Select(ct => new ChiTietHoaDonDTO
                {
                    TenSanPham = ct.MaCtspNavigation != null && ct.MaCtspNavigation.MaSpNavigation != null
                        ? ct.MaCtspNavigation.MaSpNavigation.TenSanPham
                        : "Không có tên",
                    KichThuoc = ct.MaCtspNavigation != null ? ct.MaCtspNavigation.KichThuoc : "N/A",
                    HuongVi = ct.MaCtspNavigation != null ? ct.MaCtspNavigation.HuongVi : "N/A",
                    DonGia = ct.MaCtspNavigation?.DonGia ?? 0,
                    HinhAnh = ct.MaCtspNavigation != null && ct.MaCtspNavigation.Hinhanhs != null && ct.MaCtspNavigation.Hinhanhs.Any()
                        ? ct.MaCtspNavigation.Hinhanhs.First().TenHinhAnh
                        : "default.jpg",
                    SoLuong = ct.SoLuong
                }).ToList()
            };
        }
    }
}