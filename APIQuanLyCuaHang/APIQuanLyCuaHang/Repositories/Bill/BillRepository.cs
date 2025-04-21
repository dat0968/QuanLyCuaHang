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
                .Include(p => p.MaKhNavigation)
                .Include(p => p.MaCouponNavigation)
                .Include(p => p.Chitietcombohoadons)
                .ThenInclude(c => c.MaComboNavigation)
                .Select(hd => new HoaDonDTO
                {
                    MaHd = hd.MaHd,
                    MaKh = hd.MaKh.Value,
                    MaNv = hd.MaNv,
                    DiaChiNhanHang = hd.DiaChiNhanHang,
                    NgayTao = hd.NgayTao,
                    BatDauGiao = hd.BatDauGiao,
                    NgayNhan = hd.NgayNhan,
                    NgayThanhToan = hd.NgayThanhToan,
                    HinhThucTt = hd.HinhThucTt,
                    TinhTrang = hd.TinhTrang,
                    MoTa = hd.MoTa,
                    HoTenNguoiNhan = hd.HoTen,
                    HoTenNguoiDat = hd.MaKhNavigation.HoTen,
                    HoTenNv = hd.MaNvNavigation.HoTen,
                    Sdt = hd.Sdt,
                    LyDoHuy = hd.LyDoHuy,
                    PhiVanChuyen = hd.PhiVanChuyen,
                    TienGoc = hd.TienGoc,
                    GiamGiaCoupon = hd.MaCouponNavigation.SoTienGiam.Value == null ?
                    (hd.MaCouponNavigation.PhanTramGiam.Value == null ? 0 : hd.TienGoc - hd.TienGoc * hd.MaCouponNavigation.PhanTramGiam.Value / 100) : 0,
                    Tongtien = hd.TienGoc + hd.PhiVanChuyen - (hd.MaCouponNavigation.SoTienGiam.Value == null ?
                    (hd.MaCouponNavigation.PhanTramGiam.Value == null ? 0 : hd.TienGoc - hd.TienGoc * hd.MaCouponNavigation.PhanTramGiam.Value / 100) : 0),
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
                .Include(p => p.MaKhNavigation)
                .Include(p => p.MaCouponNavigation)
                .Include(p => p.Chitietcombohoadons)
                .ThenInclude(c => c.MaComboNavigation)
                .FirstOrDefaultAsync(p => p.MaHd == id);
            if (hd == null) return null;

            return new HoaDonDTO
            {
                MaHd = hd.MaHd,
                MaKh = hd.MaKh.Value,
                MaNv = hd.MaNv,
                DiaChiNhanHang = hd.DiaChiNhanHang,
                NgayTao = hd.NgayTao,
                BatDauGiao = hd.BatDauGiao,
                NgayNhan = hd.NgayNhan,
                NgayThanhToan = hd.NgayThanhToan,
                HinhThucTt = hd.HinhThucTt,
                TinhTrang = hd.TinhTrang,
                MoTa = hd.MoTa,
                HoTenNguoiNhan = hd.HoTen,
                HoTenNguoiDat = hd.MaKhNavigation.HoTen,
                HoTenNv = hd.MaNvNavigation.HoTen,
                Sdt = hd.Sdt,
                LyDoHuy = hd.LyDoHuy,
                PhiVanChuyen = hd.PhiVanChuyen,
                TienGoc = hd.TienGoc,
                GiamGiaCoupon = hd.MaCouponNavigation.SoTienGiam.Value == null ?
                    (hd.MaCouponNavigation.PhanTramGiam.Value == null ? 0 : hd.TienGoc - hd.TienGoc * hd.MaCouponNavigation.PhanTramGiam.Value / 100) : 0,
                Tongtien = hd.TienGoc + hd.PhiVanChuyen - (hd.MaCouponNavigation.SoTienGiam.Value == null ?
                    (hd.MaCouponNavigation.PhanTramGiam.Value == null ? 0 : hd.TienGoc - hd.TienGoc * hd.MaCouponNavigation.PhanTramGiam.Value / 100) : 0),
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

        public async Task<(IEnumerable<HoaDonDTO>, int)> GetFilteredBill(string? maHD, string? hinhThucTt, string? tinhTrang, int page, int pageSize)
        {
            var query = db.Hoadons
                .Include(p => p.MaNvNavigation)
                .Include(p => p.MaKhNavigation)
                .Include(p => p.MaCouponNavigation)
                .Include(p => p.Chitietcombohoadons)
                .ThenInclude(c => c.MaComboNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(maHD))
            {
                query = query.Where(hd => hd.MaHd.ToString().Contains(maHD));
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
                    MaKh = hd.MaKh.Value,
                    MaNv = hd.MaNv,
                    HoTenNguoiNhan = hd.HoTen,
                    HoTenNguoiDat = hd.MaKhNavigation.HoTen,
                    HoTenNv = hd.MaNvNavigation.HoTen,
                    Sdt = hd.Sdt,
                    DiaChiNhanHang = hd.DiaChiNhanHang,
                    NgayTao = hd.NgayTao,
                    NgayThanhToan = hd.NgayThanhToan,
                    HinhThucTt = hd.HinhThucTt,
                    TinhTrang = hd.TinhTrang,
                    TienGoc = hd.TienGoc,
                    PhiVanChuyen = hd.PhiVanChuyen,
                    GiamGiaCoupon = hd.MaCouponNavigation.SoTienGiam.Value == null ?
                    (hd.MaCouponNavigation.PhanTramGiam.Value == null ? 0 : hd.TienGoc - hd.TienGoc * hd.MaCouponNavigation.PhanTramGiam.Value / 100) : 0,
                    Tongtien = hd.TienGoc + hd.PhiVanChuyen - (hd.MaCouponNavigation.SoTienGiam.Value == null ?
                    (hd.MaCouponNavigation.PhanTramGiam.Value == null ? 0 : hd.TienGoc - hd.TienGoc * hd.MaCouponNavigation.PhanTramGiam.Value / 100) : 0),
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
        public async Task UpdateStatus(int maHd, string tinhTrang, int? maNv, string? lydohuy)
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
            //if (existingHoaDon.MaNv.HasValue && existingHoaDon.MaNv != maNv)
            //{
            //    throw new Exception("Đơn hàng đã có nhân viên tiếp nhận, không thể cập nhật");
            //}

            if (tinhTrang.ToLower() != "Chờ xác nhận".ToLower())
            {
                existingHoaDon.MaNv = maNv;
                existingHoaDon.TinhTrang = tinhTrang;
            }
            if ((tinhTrang.ToLower() == "Đã hủy".ToLower() || tinhTrang.ToLower() == "Hoàn trả/Hoàn tiền".ToLower()) && !string.IsNullOrEmpty(lydohuy))
            {
                await CancelOrder(existingHoaDon.MaHd, tinhTrang, lydohuy);
            }

            await db.SaveChangesAsync();
        }
        public async Task<HoaDonDTOWithDetails?> GetBillDetails(int maHd)
        {
            try
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
                .Include(p => p.MaKhNavigation)
                .Include(p => p.MaCouponNavigation)
                .Include(p => p.Chitietcombohoadons)
                .ThenInclude(c => c.MaComboNavigation)
                .Include(p => p.Chitietcombohoadons)
                .ThenInclude(c => c.MaCTSpNavigation)
                    .ThenInclude(ctsp => ctsp.MaSpNavigation)


                .Where(hd => hd.MaHd == maHd)
                .FirstOrDefaultAsync();


                if (bill == null) return null;
                var giamgia = 0m;

                if (bill.MaCouponNavigation != null)
                {
                    if (bill.MaCouponNavigation.SoTienGiam.HasValue)
                    {
                        giamgia = bill.MaCouponNavigation.SoTienGiam.Value;
                    }
                    else if (bill.MaCouponNavigation.PhanTramGiam.HasValue)
                    {
                        giamgia = bill.TienGoc * bill.MaCouponNavigation.PhanTramGiam.Value / 100;
                    }
                }
                var HoaDonDTOWithDetails = new HoaDonDTOWithDetails
                {
                    MaHd = bill.MaHd,
                    MaKh = bill.MaKh.Value,
                    MaNv = bill.MaNv,
                    HoTenNv = bill.MaNvNavigation != null ? bill.MaNvNavigation.HoTen : null,
                    DiaChiNhanHang = bill.DiaChiNhanHang,
                    NgayTao = bill.NgayTao,
                    BatDauGiao = bill.BatDauGiao,
                    NgayNhan = bill.NgayNhan,
                    NgayThanhToan = bill.NgayThanhToan,
                    HinhThucTt = bill.HinhThucTt,
                    TinhTrang = bill.TinhTrang,
                    MoTa = bill.MoTa,
                    HoTenNguoiNhan = bill.HoTen,
                    HoTenNguoiDat = bill.MaKhNavigation.HoTen,
                    Sdt = bill.Sdt,
                    LyDoHuy = bill.LyDoHuy,
                    PhiVanChuyen = bill.PhiVanChuyen,
                    TienGoc = bill.TienGoc,
                    GiamGiaCoupon = giamgia,
                    Tongtien = bill.TienGoc + bill.PhiVanChuyen - giamgia,
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
            catch (Exception ex)
            {
                throw new Exception("Lỗi", ex);
            }
        }

        public async Task CancelOrder(int oderId, string selectedCancelStatus, string reasonCancel)
        {
            await db.Database.BeginTransactionAsync();
            try
            {
                // Cập nhật trạng thái Đã hủy hoặc Hoàn trả/Hoàn tiền cho đơn hàng
                var existingHoaDon = db.Hoadons.Local.FirstOrDefault(p => p.MaHd == oderId) ?? await db.Hoadons.FindAsync(oderId);
                if (existingHoaDon == null)
                {
                    throw new Exception($"Không tìm thấy Hoadon với Id {oderId}");
                }
                existingHoaDon.TinhTrang = selectedCancelStatus;
                existingHoaDon.LyDoHuy = reasonCancel;
                db.Hoadons.Update(existingHoaDon);
                // Hoàn lại số lượng sản phẩm mua lẻ trong hóa đơn
                var checkDetailOrder = db.Cthoadons.Where(p => p.MaHd == existingHoaDon.MaHd).ToList();
                if (!checkDetailOrder.Any())
                {
                    throw new Exception($"Không tìm thấy CTHoadon với Id {existingHoaDon.MaHd}");
                }
                foreach (var detail in checkDetailOrder)
                {
                    if (detail.MaCombo == null)
                    {
                        var findDetailproduct = db.Chitietsanphams.Local.FirstOrDefault(p => p.MaCtsp == detail.MaCtsp) ?? await db.Chitietsanphams.FindAsync(detail.MaCtsp);
                        if (findDetailproduct == null)
                        {
                            throw new Exception($"Không tìm thấy CTSP với Id {detail.MaCtsp}");
                        }
                        findDetailproduct.SoLuongTon += detail.SoLuong;
                        db.Chitietsanphams.Update(findDetailproduct);
                    }
                    else
                    {
                        //Hoàn lại số lượng sản phẩm mua trong combo trong hóa đơn
                        var checkDetailOrderCombo = db.Chitietcombohoadons.Where(p => p.MaHd == existingHoaDon.MaHd && p.MaCombo == detail.MaCombo).ToList();
                        foreach (var detailComboOder in checkDetailOrderCombo)
                        {
                            var findDetailproduct = db.Chitietsanphams.Local.FirstOrDefault(p => p.MaCtsp == detailComboOder.MaCTSp) ?? await db.Chitietsanphams.FindAsync(detailComboOder.MaCTSp);
                            if (findDetailproduct == null)
                            {
                                throw new Exception($"Không tìm thấy CTSP với Id {detailComboOder.MaCTSp}");
                            }
                            findDetailproduct.SoLuongTon += detailComboOder.SoLuong;
                            db.Chitietsanphams.Update(findDetailproduct);
                        }
                        //Hoàn lại số lượng combo
                        var findCombo = db.Combos.Local.FirstOrDefault(p => p.MaCombo == detail.MaCombo) ?? await db.Combos.FindAsync(detail.MaCombo);
                        if (findCombo == null)
                        {
                            throw new Exception($"Không tìm thấy combo với Id {detail.MaCombo}");
                        }
                        findCombo.SoLuong += detail.SoLuong;
                        db.Combos.Update(findCombo);
                    }
                }
                //Hoàn lại mã coupon
                if (!string.IsNullOrEmpty(existingHoaDon.MaCoupon))
                {
                    var findCoupon = db.MaCoupons.Local.FirstOrDefault(p => p.MaCode == existingHoaDon.MaCoupon) ?? await db.MaCoupons.FirstOrDefaultAsync(p => p.MaCode == existingHoaDon.MaCoupon);
                    if (findCoupon == null)
                    {
                        throw new Exception($"Không tìm mã coupon {existingHoaDon.MaCoupon}");
                    }
                    findCoupon.SoLuongDaDung -= 1;
                    db.MaCoupons.Update(findCoupon);

                    var detailCoupon = db.ChitietmaCoupons.Local.FirstOrDefault(p => p.MaCode == existingHoaDon.MaCoupon && p.MaKh == existingHoaDon.MaKh) ??
                                       await db.ChitietmaCoupons.FirstOrDefaultAsync(p => p.MaCode == existingHoaDon.MaCoupon && p.MaKh == existingHoaDon.MaKh);
                    if (detailCoupon == null)
                    {
                        throw new Exception($"Không tìm chi tiết mã coupon {existingHoaDon.MaCoupon}");
                    }
                    db.ChitietmaCoupons.Remove(detailCoupon);
                }               
                await db.SaveChangesAsync();
                await db.Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await db.Database.RollbackTransactionAsync();
                throw new Exception("Lỗi", ex);
            }
        }
    }
}