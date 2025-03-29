using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Prng.Drbg;

namespace APIQuanLyCuaHang.Repositories.Bill
{
    public class BillRepository : IBillRepository
    {
        private readonly QuanLyCuaHangContext _db;

        public BillRepository(QuanLyCuaHangContext db)
        {
            _db = db;
        }

        // Lấy danh sách tất cả hóa đơn
        public async Task<IEnumerable<HoaDonDTO>> GetAllBill()
        {
            return await _db.Hoadons
                .Select(hd => new HoaDonDTO
                {
                    MaHd = hd.MaHd,
                    MaKh = hd.MaKh,
                    MaNv = hd.MaNv,
                    DiaChiNhanHang = hd.DiaChiNhanHang,
                    NgayTao = hd.NgayTao,
                    BatDauGiao = hd.BatDauGiao,
                    NgayNhan = hd.NgayNhan,
                    NgayThanhToan = hd.NgayThanhToan,
                    HinhThucTt = hd.HinhThucTt,
                    TinhTrang = hd.TinhTrang,
                    MoTa = hd.MoTa,
                    HoTen = hd.HoTen,
                    HoTenNv = hd.MaNvNavigation.HoTen,
                    Sdt = hd.Sdt,
                    LyDoHuy = hd.LyDoHuy,
                    PhiVanChuyen = hd.PhiVanChuyen,
                    TienGoc = hd.TienGoc,
                    Tongtien = hd.TienGoc + hd.PhiVanChuyen
                })
                .ToListAsync();
        }

        // Lấy hóa đơn theo ID
        public async Task<HoaDonDTO?> GetById(int id)
        {
            var hd = await _db.Hoadons.FindAsync(id);
            if (hd == null) return null;

            return new HoaDonDTO
            {
                MaHd = hd.MaHd,
                MaKh = hd.MaKh,
                MaNv = hd.MaNv,
                DiaChiNhanHang = hd.DiaChiNhanHang,
                NgayTao = hd.NgayTao,
                BatDauGiao = hd.BatDauGiao,
                NgayNhan = hd.NgayNhan,
                NgayThanhToan = hd.NgayThanhToan,
                HinhThucTt = hd.HinhThucTt,
                TinhTrang = hd.TinhTrang,
                MoTa = hd.MoTa,
                HoTen = hd.HoTen,
                HoTenNv = hd.MaNvNavigation.HoTen,
                Sdt = hd.Sdt,
                LyDoHuy = hd.LyDoHuy,
                PhiVanChuyen = hd.PhiVanChuyen,
                TienGoc = hd.TienGoc,
                Tongtien = hd.TienGoc + hd.PhiVanChuyen
            };
        }
        //
        public async Task<(IEnumerable<HoaDonDTO>, int)> GetFilteredBill(string? hoTen, string? hinhThucTt, string? tinhTrang, int page, int pageSize)
        {
            var query = _db.Hoadons.AsQueryable();

            if (!string.IsNullOrEmpty(hoTen))
            {
                query = query.Where(hd => hd.HoTen.Contains(hoTen));
            }

            if (!string.IsNullOrEmpty(hinhThucTt))
            {
                query = query.Where(hd => hd.HinhThucTt == hinhThucTt);
            }

            if (!string.IsNullOrEmpty(tinhTrang))
            {
                query = query.Where(hd => hd.TinhTrang == tinhTrang);
            }

            int totalItems = await query.CountAsync();

            var data = await query
                .OrderBy(hd => hd.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(hd => new HoaDonDTO
                {
                    MaHd = hd.MaHd,
                    HoTen = hd.HoTen,
                    HoTenNv = hd.MaNvNavigation.HoTen,
                    Sdt = hd.Sdt,
                    DiaChiNhanHang = hd.DiaChiNhanHang,
                    NgayTao = hd.NgayTao,
                    NgayThanhToan = hd.NgayThanhToan,
                    HinhThucTt = hd.HinhThucTt,
                    TinhTrang = hd.TinhTrang,
                    TienGoc = hd.TienGoc,
                    PhiVanChuyen = hd.PhiVanChuyen,
                    Tongtien = hd.TienGoc + hd.PhiVanChuyen,
                    MoTa = hd.MoTa,
                })
                .ToListAsync();

            return (data, totalItems);
        }

        // Cập nhật tình trạng hóa đơn
        public async Task UpdateStatus(int maHd, string tinhTrang, int? maNv)
        {
            var existingHoaDon = await _db.Hoadons.FindAsync(maHd);
            if (existingHoaDon == null)
            {
                throw new Exception("Không tìm thấy hóa đơn");
            }
            var immutableStatuses = new List<string> { "Đã hủy", "Hoàn trả/Hoàn tiền" };
            if (immutableStatuses.Contains(existingHoaDon.TinhTrang))
            {
                throw new Exception($"Hóa đơn với trạng thái '{existingHoaDon.TinhTrang}' không thể được cập nhật.");
            }
            if (existingHoaDon.MaNv.HasValue && existingHoaDon.MaNv != maNv)
            {
                throw new Exception("Đơn hàng đã có nhân viên tiếp nhận, không thể thay đổi người tiếp nhận.");
            }

            if(tinhTrang.ToLower() != "Chờ xác nhận".ToLower())
            {
                existingHoaDon.MaNv = maNv;
            }
            existingHoaDon.TinhTrang = tinhTrang;
            await _db.SaveChangesAsync();
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
                HoTen = bill.HoTen,
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
