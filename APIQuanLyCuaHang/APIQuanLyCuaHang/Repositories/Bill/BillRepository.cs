using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Prng.Drbg;
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
        // Lấy danh sách tất cả hóa đơn
        public async Task<IEnumerable<HoaDonDTO>> GetAllBill()
        {
            return await db.Hoadons
                .Include(p => p.MaNvNavigation)
                .Include(p => p.Chitietcombohoadons)
                .ThenInclude(c => c.MaComboNavigation)
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
                    GiamGiaCoupon = hd.GiamGiaCoupon,
                    Tongtien = hd.TienGoc + hd.PhiVanChuyen - hd.GiamGiaCoupon,
                    ChitietcombohoadonDTOs = hd.Chitietcombohoadons.Select(p => new ChitietcombohoadonDTO
                    {
                        MaHd = hd.MaHd,
                        MaCombo = p.MaCombo,
                        MaCTSp = p.MaCTSp,
                        SoLuong = p.SoLuong,
                        DonGia = p.DonGia,
                    }).ToList()
                })
                .ToListAsync();
        }

        // Lấy hóa đơn theo ID
        public async Task<HoaDonDTO?> GetById(int id)
        {
            var hd = await db.Hoadons.AsNoTracking()
                .Include(p => p.MaNvNavigation)
                .Include(p => p.Chitietcombohoadons)
                .ThenInclude(c => c.MaComboNavigation)
                .FirstOrDefaultAsync(p => p.MaHd == id);
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
                GiamGiaCoupon = hd.GiamGiaCoupon,
                Tongtien = hd.TienGoc + hd.PhiVanChuyen - hd.GiamGiaCoupon,
                ChitietcombohoadonDTOs = hd.Chitietcombohoadons.Select(p => new ChitietcombohoadonDTO
                {
                    MaHd = hd.MaHd,
                    MaCombo = p.MaCombo,
                    MaCTSp = p.MaCTSp,
                    SoLuong = p.SoLuong,
                    DonGia = p.DonGia,
                }).ToList()
            };
        }
        
        public async Task<(IEnumerable<HoaDonDTO>, int)> GetFilteredBill(string? hoTen, string? hinhThucTt, string? tinhTrang, int page, int pageSize)
        {
            var query = db.Hoadons
                .Include(p => p.MaNvNavigation)
                .Include(p => p.Chitietcombohoadons)
                .ThenInclude(c => c.MaComboNavigation)
                .AsQueryable();

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
                    GiamGiaCoupon = hd.GiamGiaCoupon,
                    Tongtien = hd.TienGoc + hd.PhiVanChuyen - hd.GiamGiaCoupon,
                    MoTa = hd.MoTa,
                    ChitietcombohoadonDTOs = hd.Chitietcombohoadons.Select(p => new ChitietcombohoadonDTO
                    {
                        MaHd = hd.MaHd,
                        MaCombo = p.MaCombo,
                        MaCTSp = p.MaCTSp,
                        SoLuong = p.SoLuong,
                        DonGia = p.DonGia,
                    }).ToList()
                })
                .ToListAsync();

            return (data, totalItems);
        }

        // Cập nhật tình trạng hóa đơn
        public async Task UpdateStatus(int maHd, string tinhTrang, int? maNv)
        {
            var existingHoaDon = await db.Hoadons.FindAsync(maHd);
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

            if (tinhTrang.ToLower() != "Chờ xác nhận".ToLower())
            {
                existingHoaDon.MaNv = maNv;
            }
            existingHoaDon.TinhTrang = tinhTrang;
            await db.SaveChangesAsync();
        }
        public async Task<HoaDonDTOWithDetails?> GetBillDetails(int maHd)
        {
            var bill = await db.Hoadons
                .Include(hd => hd.Cthoadons)
                .ThenInclude(ct => ct.MaCtspNavigation)
                .ThenInclude(ctsp => ctsp.Hinhanhs)
                .Include(hd => hd.Cthoadons)
                .ThenInclude(ct => ct.MaCtspNavigation)
                .ThenInclude(ctsp => ctsp.MaSpNavigation)
                .Include(hd => hd.Cthoadons)
                .ThenInclude(c => c.Combo)
                .Include(p => p.MaNvNavigation)
                .Include(p => p.Chitietcombohoadons)
                .ThenInclude(c => c.MaComboNavigation)
            .Include(p => p.Chitietcombohoadons)
                .ThenInclude(c => c.MaCTSpNavigation)
                    .ThenInclude(ctsp => ctsp.MaSpNavigation)


                .Where(hd => hd.MaHd == maHd)
                .FirstOrDefaultAsync();


            if (bill == null) return null;
            var HoaDonDTOWithDetails = new HoaDonDTOWithDetails
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
                Tongtien = bill.TienGoc + bill.PhiVanChuyen - bill.GiamGiaCoupon,
                GiamGiaCoupon = bill.GiamGiaCoupon,
                ChiTietHoaDon = bill.Cthoadons.Select(ct => new ChiTietHoaDonDTO
                {
                    TenSanPham = ct.MaCtspNavigation != null && ct.MaCtspNavigation.MaSpNavigation != null
                        ? ct.MaCtspNavigation.MaSpNavigation.TenSanPham
                        : "Không có tên",
                    KichThuoc = ct.MaCtspNavigation != null ? ct.MaCtspNavigation.KichThuoc : "N/A",
                    HuongVi = ct.MaCtspNavigation != null ? ct.MaCtspNavigation.HuongVi : "N/A",
                    DonGia = ct.DonGia,
                    HinhAnh = ct.MaCtspNavigation != null && ct.MaCtspNavigation.Hinhanhs != null && ct.MaCtspNavigation.Hinhanhs.Any()
                        ? ct.MaCtspNavigation.Hinhanhs.First().TenHinhAnh
                        : "default.jpg",
                    SoLuong = ct.SoLuong,
                    GiamGia = ct.MaCombo != null ? 
                    (ct.Combo.PhanTramGiam != null && ct.Combo.PhanTramGiam > 0 ? (ct.DonGia * ct.SoLuong) * (decimal)ct.Combo.PhanTramGiam / 100 : (ct.DonGia * ct.SoLuong) - (decimal)ct.Combo.SoTienGiam) 
                    : ct.GiamGia,
                    TienGoc = ct.DonGia * ct.SoLuong,
                    MaCombo = ct.MaCombo,
                    TenCombo = ct.Combo?.TenCombo,
                }).ToList(),
                ChitietcombohoadonDTOs = bill.Chitietcombohoadons.Select(p => new ChitietcombohoadonDTO
                {
                    MaHd = bill.MaHd,
                    MaCombo = p.MaCombo,
                    MaCTSp = p.MaCTSp,
                    TenSpCombo = p.MaCTSpNavigation.MaSpNavigation.TenSanPham,
                    HuongVi = p.MaCTSpNavigation?.HuongVi,
                    KichThuoc = p.MaCTSpNavigation?.KichThuoc, 
                    SoLuong = p.SoLuong,
                    DonGia = p.DonGia,
                }).ToList()
            };
            return HoaDonDTOWithDetails;
        }
    }
}
