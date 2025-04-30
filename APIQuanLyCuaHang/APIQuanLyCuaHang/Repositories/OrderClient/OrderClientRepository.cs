using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Constants;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Helpers.Handlers;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.OrderClient
{
    public class OrderClientRepository(QuanLyCuaHangContext db) : Repository<Hoadon>(db), IOrderClientRepository
    {
        private readonly QuanLyCuaHangContext _db = db;

        public async Task<ResponseAPI<List<HoaDonKhachDTO>>> GetAllInvoiceByUserId(int? userId)
        {
            ResponseAPI<List<HoaDonKhachDTO>> response = new();

            try
            {
                // Kiểm tra xem người dùng có tồn tại trong hệ thống không
                if (userId == null)
                {
                    throw new ArgumentNullException(nameof(userId), "ID người dùng không được để trống.");
                }

                bool isHaveUser = await _db.Khachhangs.AnyAsync(kh => kh.MaKh == userId);
                if (!isHaveUser)
                {
                    throw new EntryPointNotFoundException("Không nhận diện được tài khoản của bạn trong hệ thống.");
                }

                // Lấy danh sách hóa đơn của người dùng
                var listOrigin = await base.GetAllAsync(x => x.MaKh == userId, "Cthoadons,Cthoadons.MaCtspNavigation,Cthoadons.MaCtspNavigation.MaSpNavigation,Cthoadons.MaCtspNavigation.Hinhanhs,Chitietcombohoadons,Chitietcombohoadons.MaComboNavigation,MaCouponNavigation");

                List<HoaDonKhachDTO> listDTO = new();

                if (listOrigin != null)
                {
                    foreach (var aOrder in listOrigin)
                    {
                        if (aOrder == null) continue; // Kiểm tra xem aOrder có null không

                        decimal tienGoc = aOrder?.TienGoc ?? 0m;
                        decimal giamGiaCoupon = 0m;
                        if (aOrder?.MaCouponNavigation != null)
                        {
                            giamGiaCoupon = aOrder.MaCouponNavigation.SoTienGiam ?? (1 - aOrder.MaCouponNavigation.PhanTramGiam / 100) * tienGoc ?? 0m;
                        }
                        // Kiểm tra xem hóa đơn có tồn tại không
                        HoaDonKhachDTO configDataDTO = new HoaDonKhachDTO
                        {
                            MaHd = aOrder.MaHd,
                            MaKh = aOrder.MaKh.Value,
                            MaNv = aOrder.MaNv,
                            NgayTao = aOrder.NgayTao,
                            BatDauGiao = aOrder.BatDauGiao,
                            NgayNhan = aOrder.NgayNhan,
                            DiaChiNhanHang = aOrder.DiaChiNhanHang,
                            NgayThanhToan = aOrder.NgayThanhToan,
                            HinhThucTt = aOrder.HinhThucTt,
                            TinhTrang = aOrder.TinhTrang,
                            MoTa = aOrder.MoTa,
                            HoTen = aOrder?.HoTen ?? "Tên khách hàng",
                            Sdt = aOrder?.Sdt ?? "xxx-xxx-xxx",
                            LyDoHuy = aOrder?.LyDoHuy ?? "Không có lý do",
                            IsDelete = aOrder?.IsDelete,
                            PhiVanChuyen = aOrder?.PhiVanChuyen ?? 0m,
                            TienGoc = tienGoc,
                            GiamGiaCoupon = giamGiaCoupon,
                            ChiTietHoaDonKhachs = new List<ChiTietHoaDonKhachDTO>() // Khởi tạo danh sách chi tiết hóa đơn
                        };

                        // Thêm chi tiết sản phẩm vào hóa đơn
                        if (aOrder!.Cthoadons.Count != 0)
                        {
                            foreach (var aDetailProductOrder in aOrder.Cthoadons)
                            {
                                var testInfo = aDetailProductOrder.MaCtspNavigation;
                                ChiTietHoaDonKhachDTO configDetailDTO = new ChiTietHoaDonKhachDTO()
                                {
                                    MaHd = aDetailProductOrder.MaHd,
                                    MaDoiTuong = aDetailProductOrder.MaCtsp ?? 0,
                                    LoaiDoiTuong = "Sản phẩm",
                                    SoLuong = aDetailProductOrder.SoLuong,
                                    KichThuoc = testInfo == null ? "" : testInfo?.KichThuoc ?? "Không có",
                                    HuongVi = testInfo == null ? "" : testInfo?.HuongVi ?? "Không có",
                                    DonGia = testInfo == null ? 1m : aDetailProductOrder.DonGia,
                                    TenDoiTuong = testInfo == null ? "" : testInfo?.MaSpNavigation?.TenSanPham ?? "Noname",
                                    HinhAnh = testInfo == null ? "" : testInfo?.Hinhanhs.FirstOrDefault()?.TenHinhAnh ?? "",
                                    MoTa = testInfo == null ? "" : testInfo?.MaSpNavigation.MoTa
                                };
                                configDataDTO.ChiTietHoaDonKhachs.Add(configDetailDTO); // Thêm chi tiết vào danh sách Hmmm
                            }
                        }

                        // Thêm chi tiết combo vào hóa đơn
                        if (aOrder.Chitietcombohoadons.Count != 0)
                        {
                            foreach (var aDetailComboOrder in aOrder.Chitietcombohoadons)
                            {
                                ChiTietHoaDonKhachDTO configDetailDTO = new ChiTietHoaDonKhachDTO()
                                {
                                    MaHd = aDetailComboOrder.MaHd,
                                    MaDoiTuong = aDetailComboOrder.MaCombo,
                                    LoaiDoiTuong = "Combo",
                                    SoLuong = aDetailComboOrder.SoLuong,
                                    DonGia = aDetailComboOrder.DonGia,
                                    TenDoiTuong = aDetailComboOrder.MaComboNavigation.TenCombo,
                                    HinhAnh = aDetailComboOrder.MaComboNavigation.Hinh ?? "",
                                    MoTa = aDetailComboOrder.MaComboNavigation.MoTa
                                };
                                configDataDTO.ChiTietHoaDonKhachs.Add(configDetailDTO); // Thêm chi tiết vào danh sách
                            }
                        }
                        configDataDTO.TongTien = configDataDTO.TienGoc - configDataDTO.GiamGiaCoupon + configDataDTO.PhiVanChuyen;
                        listDTO.Add(configDataDTO); // Thêm hóa đơn vào danh sách kết quả
                    }
                }
                response.SetSuccessResponse("Lấy danh sách thành công.");
                response.SetData(listDTO);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }
            return response;
        }
        public async Task<ResponseAPI<dynamic>> UpdateStatusOrderOfUser(int? userId, int? orderId, string? statusChange, string? reasonCancel)
        {
            ResponseAPI<dynamic> response = new();
            try
            {
                userId ??= userId ?? throw new Exception($"Không nhận được mã người dùng.");
                orderId ??= orderId ?? throw new Exception("Không nhận được mã đơn hàng.");
                statusChange ??= statusChange ?? throw new Exception("Không xác định được trạng thái muốn đổi.");

                var originData = await base.GetAsync(x => x.MaKh == userId && x.MaHd == orderId) ?? throw new Exception("Đơn hàng không tồn tại.");

                // Kiểm tra trạng thái hiện tại và trạng thái muốn chuyển đổi
                if (originData.TinhTrang == statusChange)
                {
                    throw new Exception("Trạng thái không thay đổi.");
                }

                // Xử lí thay đổi trạng thái đơn hàng và có hoàn lại sản phẩm hay không
                if (TrangThaiDonHang.ValidateAndCancelOrderForCustomer(originData, statusChange, reasonCancel))
                {
                    // Nếu trạng thái là hủy đơn hàng thì trả lại sản phẩm về kho
                    await ReturnProductAndComboWhenCancel((int)orderId, originData.MaCoupon);
                }

                _db.Update(originData);
                await _db.SaveChangesAsync();

                response.SetSuccessResponse("Cập nhật trạng thái đơn hàng thành công.");
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, response);
            }

            return response;
        }


        #region [Private Method]
        // ? Trả về lại sản phẩm về kho khi bị hủy, bruh
        private async Task ReturnProductAndComboWhenCancel(int orderId, string? couponId)
        {
            var listDetailProductInOrder = await _db.Cthoadons.Where(x => x.MaHd == orderId).ToListAsync();
            var listDetailsProducts = await _db.Chitietsanphams.ToListAsync();
            var listDetailsCombosInOrder = await _db.Chitietcombohoadons.Where(x => x.MaHd == orderId).ToListAsync();
            var listDetailsCombos = await _db.Combos.ToListAsync();
            foreach (var aDetail in listDetailProductInOrder)
            {
                var increaseDtProduct = listDetailsProducts.FirstOrDefault(x => x.MaCtsp == aDetail.MaCtsp);
                if (increaseDtProduct == null) continue;
                increaseDtProduct.SoLuongTon += aDetail.SoLuong;
            }
            foreach (var aCombo in listDetailsCombosInOrder)
            {
                var increaseDtCombo = listDetailsCombos.FirstOrDefault(x => x.MaCombo == aCombo.MaCombo);
                if (increaseDtCombo == null) continue;
                increaseDtCombo.SoLuong += aCombo.SoLuong;
            }
            if (!string.IsNullOrEmpty(couponId))
            {
                var coupon = await _db.MaCoupons.FirstOrDefaultAsync(x => x.MaCode == couponId);
                if (coupon != null)
                {
                    coupon.SoLuongDaDung -= 1;
                    _db.MaCoupons.Update(coupon);
                }
            }
            _db.UpdateRange(listDetailsProducts);
            _db.UpdateRange(listDetailsCombos);
            await _db.SaveChangesAsync();
        }
        #endregion

    }
}