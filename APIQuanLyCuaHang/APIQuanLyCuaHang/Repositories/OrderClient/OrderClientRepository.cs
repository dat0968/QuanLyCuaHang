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
                var listOrigin = await base.GetAllAsync(x => x.MaKh == userId, "Cthoadons,Cthoadons.MaCtspNavigation,Cthoadons.MaCtspNavigation.MaSpNavigation,Chitietcombohoadons,Chitietcombohoadons.MaComboNavigation");

                List<HoaDonKhachDTO> listDTO = new();

                if (listOrigin != null)
                {
                    foreach (var aOrder in listOrigin)
                    {
                        HoaDonKhachDTO configDataDTO = new HoaDonKhachDTO
                        {
                            MaHd = aOrder.MaHd,
                            MaKh = aOrder.MaKh,
                            MaNv = aOrder.MaNv,
                            NgayTao = aOrder.NgayTao,
                            BatDauGiao = aOrder.BatDauGiao,
                            NgayNhan = aOrder.NgayNhan,
                            DiaChiNhanHang = aOrder.DiaChiNhanHang,
                            NgayThanhToan = aOrder.NgayThanhToan,
                            HinhThucTt = aOrder.HinhThucTt,
                            TinhTrang = aOrder.TinhTrang,
                            MoTa = aOrder.MoTa,
                            HoTen = aOrder.HoTen,
                            Sdt = aOrder.Sdt,
                            LyDoHuy = aOrder.LyDoHuy,
                            IsDelete = aOrder.IsDelete,
                            PhiVanChuyen = aOrder.PhiVanChuyen,
                            TienGoc = aOrder.TienGoc,
                            GiamGiaCoupon = aOrder.GiamGiaCoupon,
                            ChiTietHoaDonKhachs = new List<ChiTietHoaDonKhachDTO>() // Khởi tạo danh sách chi tiết hóa đơn
                        };

                        // Thêm chi tiết sản phẩm vào hóa đơn
                        if (aOrder.Cthoadons.Count != 0)
                        {
                            foreach (var aDetailProductOrder in aOrder.Cthoadons)
                            {
                                ChiTietHoaDonKhachDTO configDetailDTO = new ChiTietHoaDonKhachDTO()
                                {
                                    MaHd = aDetailProductOrder.MaHd,
                                    MaDoiTuong = aDetailProductOrder.MaCtsp ?? 0,
                                    LoaiDoiTuong = "Sản phẩm",
                                    SoLuong = aDetailProductOrder.SoLuong,
                                    KichThuoc = aDetailProductOrder.MaCtspNavigation?.KichThuoc ?? "Không có",
                                    HuongVi = aDetailProductOrder.MaCtspNavigation?.HuongVi ?? "Không có",
                                    DonGia = aDetailProductOrder.DonGia,
                                    TenDoiTuong = aDetailProductOrder.MaCtspNavigation.MaSpNavigation.TenSanPham,
                                    HinhAnh = aDetailProductOrder.MaCtspNavigation.Hinhanhs.FirstOrDefault()?.TenHinhAnh ?? "",
                                    MoTa = aDetailProductOrder.MaCtspNavigation.MaSpNavigation.MoTa
                                };
                                configDataDTO.ChiTietHoaDonKhachs.Add(configDetailDTO); // Thêm chi tiết vào danh sách
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
                        configDataDTO.TongTien = configDataDTO.TienGoc - configDataDTO.GiamGiaCoupon - configDataDTO.PhiVanChuyen;
                        listDTO.Add(configDataDTO); // Thêm hóa đơn vào danh sách kết quả
                    }
                }
                response.SetSuccessResponse("Lấy danh sách thành công.");
                response.SetData(listDTO);
            }
            catch (ArgumentNullException argEx)
            {
                response.SetMessageResponseWithException(400, argEx);
            }
            catch (EntryPointNotFoundException userEx)
            {
                response.SetMessageResponseWithException(404, userEx);
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
                    await ReturnProductAndComboWhenCancel((int)orderId);
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
        private async Task ReturnProductAndComboWhenCancel(int orderId)
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
            _db.UpdateRange(listDetailsProducts);
            _db.UpdateRange(listDetailsCombos);
            await _db.SaveChangesAsync();
        }
        #endregion
    }
}