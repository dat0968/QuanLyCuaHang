using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Constants;
using APIQuanLyCuaHang.DTO;
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
                response.SetMessageResponseWithException(500, ex);
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

                var originData = await base.GetAsync(x => x.MaHd == orderId) ?? throw new Exception("Đơn hàng không tồn tại.");

                // Kiểm tra trạng thái hiện tại và trạng thái muốn chuyển đổi
                if (originData.TinhTrang == statusChange)
                {
                    throw new Exception("Trạng thái không thay đổi.");
                }

                // Logic chuyển đổi trạng thái
                switch (originData.TinhTrang)
                {
                    case TrangThaiDonHang.ChoThanhToan:
                        if (statusChange == TrangThaiDonHang.DaHuy)
                        {
                            TrangThaiDonHang.ValidateAndChangeStatus(originData, statusChange, reasonCancel);
                            await ReturnProductWhenCancel(orderId.Value!);
                        }
                        else
                        {
                            throw new Exception($"Không thể chuyển từ 'Chờ thanh toán' sang trạng thái [{statusChange}].");
                        }
                        break;

                    case TrangThaiDonHang.DaXacNhan:
                        if (statusChange == TrangThaiDonHang.HoanTra_HoanTien)
                        {
                            TrangThaiDonHang.ValidateAndChangeStatus(originData, statusChange, reasonCancel);
                            await ReturnProductWhenCancel(orderId.Value!);
                        }
                        else if (statusChange == TrangThaiDonHang.DaHuy)
                        {
                            TrangThaiDonHang.ValidateAndChangeStatus(originData, statusChange, reasonCancel);
                            await ReturnProductWhenCancel(orderId.Value!);
                        }
                        else
                        {
                            throw new Exception($"Không thể chuyển từ 'Đã xác nhận' sang trạng thái [{statusChange}].");
                        }
                        break;

                    case TrangThaiDonHang.DaGiaoChoDonViVanChuyen:
                        if (statusChange == TrangThaiDonHang.HoanTra_HoanTien)
                        {
                            originData.TinhTrang = statusChange;
                            originData.LyDoHuy = reasonCancel;
                        }
                        else if (statusChange == TrangThaiDonHang.DaHuy)
                        {
                            TrangThaiDonHang.ValidateAndChangeStatus(originData, statusChange, reasonCancel);
                        }
                        else
                        {
                            throw new Exception($"Không thể chuyển từ 'Đã giao cho đơn vị vận chuyển' sang trạng thái [{statusChange}].");
                        }
                        break;

                    case TrangThaiDonHang.DangGiaoHang:
                        if (statusChange == TrangThaiDonHang.HoanTra_HoanTien)
                        {
                            TrangThaiDonHang.ValidateAndChangeStatus(originData, statusChange, reasonCancel);
                        }
                        else if (statusChange == TrangThaiDonHang.DaHuy)
                        {
                            TrangThaiDonHang.ValidateAndChangeStatus(originData, statusChange, reasonCancel);
                        }
                        else
                        {
                            throw new Exception($"Không thể chuyển từ 'Đang giao hàng' sang trạng thái [{statusChange}].");
                        }
                        break;

                    case TrangThaiDonHang.HoanTra_HoanTien:
                        throw new Exception("Đơn hàng đã hoàn trả/hoàn tiền không thể thay đổi trạng thái.");

                    case TrangThaiDonHang.DaHuy:
                        throw new Exception("Đơn hàng đã hủy không thể thay đổi trạng thái.");

                    default:
                        throw new Exception("Trạng thái không hợp lệ.");
                }

                _db.Update(originData);
                await _db.SaveChangesAsync();

                response.SetSuccessResponse("Cập nhật trạng thái đơn hàng thành công.");
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
            }

            return response;
        }

        #region [Private Method]
        // ? Trả về lại sản phẩm về kho khi bị hủy, bruh
        private async Task ReturnProductWhenCancel(int orderId)
        {
            var listDetailOrder = await _db.Cthoadons.Where(x => x.MaHd == orderId).ToListAsync();
            var listDetailsProducts = await _db.Chitietsanphams.ToListAsync();
            foreach (var aDetail in listDetailOrder)
            {
                var increaseDtProduct = listDetailsProducts.FirstOrDefault(x => x.MaCtsp == aDetail.MaCtsp);
                if (increaseDtProduct == null) continue;
                increaseDtProduct.SoLuongTon += aDetail.SoLuong;
            }
            await _db.SaveChangesAsync();
        }
        #endregion
    }
}