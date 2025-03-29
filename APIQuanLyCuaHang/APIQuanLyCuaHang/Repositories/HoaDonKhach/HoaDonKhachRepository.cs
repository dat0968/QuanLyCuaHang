using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Constants;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.HoaDonKhach
{
    public class HoaDonKhachRepository(QuanLyCuaHangContext db) : Repository<Hoadon>(db), IHoaDonKhachRepository
    {
        private readonly QuanLyCuaHangContext _db = db;

        public async Task<ResponseAPI<List<HoaDonKhachDTO>>> GetAllInvoiceByUserId(int? userId)
        {
            ResponseAPI<List<HoaDonKhachDTO>> response = new();

            try
            {
                bool isHaveUser = await _db.Khachhangs.AnyAsync(kh => kh.MaKh == userId);
                if (!isHaveUser)
                {
                    throw new Exception("Không nhận diện được tài khoản của bạn trong hệ thống.");
                }
                var listOrigin = await base.GetAllAsync(x => x.MaKh == userId, "Cthoadons,Cthoadons.MaCtspNavigation,Cthoadons.MaCtspNavigation.MaSpNavigation");
                var listDTO = listOrigin.Select(lo => new HoaDonKhachDTO
                {
                    MaHd = lo.MaHd,
                    MaKh = lo.MaKh,
                    MaNv = lo.MaNv,
                    NgayTao = lo.NgayTao,
                    BatDauGiao = lo.BatDauGiao,
                    NgayNhan = lo.NgayNhan,
                    DiaChiNhanHang = lo.DiaChiNhanHang,
                    NgayThanhToan = lo.NgayThanhToan,
                    HinhThucTt = lo.HinhThucTt,
                    TinhTrang = lo.TinhTrang,
                    MoTa = lo.MoTa,
                    HoTen = lo.HoTen,
                    Sdt = lo.Sdt,
                    LyDoHuy = lo.LyDoHuy,
                    IsDelete = lo.IsDelete,
                    PhiVanChuyen = lo.PhiVanChuyen,
                    TienGoc = lo.TienGoc,
                    ChiTietHoaDonKhachs = lo.Cthoadons.Select(ct => new ChiTietHoaDonKhachDTO
                    {
                        MaHd = ct.MaHd,
                        MaCtsp = ct.MaCtsp,
                        SoLuong = ct.SoLuong,
                        KichThuoc = ct.MaCtspNavigation.KichThuoc,
                        HuongVi = ct.MaCtspNavigation.HuongVi,
                        DonGia = ct.MaCtspNavigation.DonGia,
                        TenSanPham = ct.MaCtspNavigation.MaSpNavigation.TenSanPham,
                        MoTa = ct.MaCtspNavigation.MaSpNavigation.MoTa
                    }).ToList()
                }).ToList();

                response.SetSuccessResponse("Lấy danh sách thành công.");
                response.SetData(listDTO);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
            }
            return response;
        }
        public async Task<ResponseAPI<dynamic>> UpdateStatusOrderOfUser(int? userId, int? orderId, string? statusChange)
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
                        if (statusChange == TrangThaiDonHang.DaXacNhan)
                        {
                            originData.TinhTrang = statusChange;
                        }
                        else
                        {
                            throw new Exception("Không thể chuyển từ 'Chờ thanh toán' sang trạng thái khác.");
                        }
                        break;

                    case TrangThaiDonHang.DaXacNhan:
                        if (statusChange == TrangThaiDonHang.DaGiaoChoDonViVanChuyen)
                        {
                            originData.TinhTrang = statusChange;
                        }
                        else if (statusChange == TrangThaiDonHang.DaHuy)
                        {
                            originData.TinhTrang = statusChange; // Hủy đơn hàng
                        }
                        else
                        {
                            throw new Exception("Không thể chuyển từ 'Đã xác nhận' sang trạng thái khác.");
                        }
                        break;

                    case TrangThaiDonHang.DaGiaoChoDonViVanChuyen:
                        if (statusChange == TrangThaiDonHang.DangGiaoHang)
                        {
                            originData.TinhTrang = statusChange;
                        }
                        else if (statusChange == TrangThaiDonHang.DaHuy)
                        {
                            originData.TinhTrang = statusChange; // Hủy đơn hàng
                        }
                        else
                        {
                            throw new Exception("Không thể chuyển từ 'Đã giao cho đơn vị vận chuyển' sang trạng thái khác.");
                        }
                        break;

                    case TrangThaiDonHang.DangGiaoHang:
                        if (statusChange == TrangThaiDonHang.HoanTra_HoanTien)
                        {
                            originData.TinhTrang = statusChange; // Hoàn trả/Hoàn tiền
                        }
                        else if (statusChange == TrangThaiDonHang.DaHuy)
                        {
                            originData.TinhTrang = statusChange; // Hủy đơn hàng
                        }
                        else
                        {
                            throw new Exception("Không thể chuyển từ 'Đang giao hàng' sang trạng thái khác.");
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
    }
}