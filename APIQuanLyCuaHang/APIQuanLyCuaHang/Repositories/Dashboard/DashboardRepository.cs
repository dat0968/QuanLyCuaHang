using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.ViewModels;
using APIQuanLyCuaHang.ViewModels.DataChart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.Dashboard
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly QuanLyCuaHangContext _db;

        public DashboardRepository(QuanLyCuaHangContext db)
        {
            _db = db;
        }
        public async Task<ResponseAPI<dynamic?>> GetAllInvoiceDataStatisticsAsync()
        {
            ResponseAPI<dynamic?> response = new();
            try
            {

                // Lấy dữ liệu từ cơ sở dữ liệu
                var hoadonList = await _db.Hoadons.ToListAsync();

                // Chuyển đổi danh sách Hoadon sang HoadonVM
                var result = hoadonList.Select(hoadon => new InvoiceDC
                {
                    MaHd = hoadon.MaHd,
                    DiaChiNhanHang = hoadon.DiaChiNhanHang,
                    HinhThucTt = hoadon.HinhThucTt,
                    TinhTrang = hoadon.TinhTrang,
                    MaNv = hoadon.MaNv,
                    MaKh = hoadon.MaKh,
                    MoTa = hoadon.MoTa,
                    HoTen = hoadon.HoTen,
                    Sdt = hoadon.Sdt,
                    NgayTao = hoadon.NgayTao,
                    NgayNhan = hoadon.NgayNhan,
                    NgayThanhToan = hoadon.NgayThanhToan,
                    LyDoHuy = hoadon.LyDoHuy,
                    PhiVanChuyen = hoadon.PhiVanChuyen,
                    TienGoc = hoadon.TienGoc,
                    TongTien = hoadon.TienGoc - hoadon.PhiVanChuyen // Note
                }).ToList();

                var listInvoiceDetails = await GetAllInvoiceDetailData();
                if (listInvoiceDetails != null && listInvoiceDetails.Count != 0)
                {
                    foreach (var invoice in result)
                    {
                        invoice.SanPhamHoaDons = listInvoiceDetails.Where(dt => dt.MaHd == invoice.MaHd).ToList();
                    }
                }

                response.SetSuccessResponse();
                response.SetData(result);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(400, ex);
            }

            return response;
        }

        public async Task<ResponseAPI<dynamic?>> GetAllInvoiceDetailStatisticsAsync()
        {
            ResponseAPI<dynamic?> response = new();
            try
            {
                var result = await GetAllInvoiceDetailData();

                response.SetSuccessResponse();
                response.SetData(result);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(400, ex);
            }

            return response;
        }

        public async Task<ResponseAPI<dynamic?>> GetAllInvoiceStatisticsAsync()
        {
            ResponseAPI<dynamic?> response = new();
            try
            {

                // Lấy dữ liệu từ cơ sở dữ liệu
                var hoadonList = await _db.Hoadons.ToListAsync();

                // Chuyển đổi danh sách Hoadon sang HoadonVM
                var result = hoadonList.Select(hoadon => new InvoiceDC
                {
                    MaHd = hoadon.MaHd,
                    DiaChiNhanHang = hoadon.DiaChiNhanHang,
                    HinhThucTt = hoadon.HinhThucTt,
                    TinhTrang = hoadon.TinhTrang,
                    MaNv = hoadon.MaNv,
                    MaKh = hoadon.MaKh,
                    MoTa = hoadon.MoTa,
                    HoTen = hoadon.HoTen,
                    Sdt = hoadon.Sdt,
                    NgayTao = hoadon.NgayTao,
                    NgayNhan = hoadon.NgayNhan,
                    NgayThanhToan = hoadon.NgayThanhToan,
                    LyDoHuy = hoadon.LyDoHuy,
                    PhiVanChuyen = hoadon.PhiVanChuyen,
                    TienGoc = hoadon.TienGoc,
                    TongTien = hoadon.TienGoc - hoadon.PhiVanChuyen // Note
                }).ToList();

                response.SetSuccessResponse();
                response.SetData(result);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(400, ex);
            }

            return response;
        }

        public async Task<ResponseAPI<dynamic?>> GetEmployeeOrderStatisticsAsync()
        {
            ResponseAPI<dynamic?> response = new();
            try
            {
                // Lấy dữ liệu từ cơ sở dữ liệu
                var nhanviens = await _db.Nhanviens.ToListAsync();

                // Chuyển đổi danh sách nhân viên thành danh sách NhanVienVM
                var staffDCs = nhanviens.Select(nv => new StaffDC
                {
                    MaNv = nv.MaNv,
                    HoTen = nv.HoTen,
                    GioiTinh = nv.GioiTinh,
                    NgaySinh = nv.NgaySinh,
                    DiaChi = nv.DiaChi,
                    Sdt = nv.Sdt,
                    Email = nv.Email,
                    NgayVaoLam = nv.NgayVaoLam,
                    TenTaiKhoan = nv.TenTaiKhoan,
                    MaChucVu = nv.MaChucVu,
                    TinhTrang = nv.TinhTrang,
                }).ToList();

                // ! Important: This need loop data DoanhThuMangLai

                response.SetSuccessResponse();
                response.SetData(staffDCs);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(400, ex);
            }
            return response;
        }

        public async Task<ResponseAPI<dynamic?>> GetUserStatisticsAsync()
        {
            ResponseAPI<dynamic?> response = new();
            try
            {
                // Lấy dữ liệu từ cơ sở dữ liệu
                var nhanviens = await _db.Nhanviens.ToListAsync();

                // Chuyển đổi danh sách nhân viên thành danh sách NhanVienVM
                var staffDCs = nhanviens.Select(nv => new StaffDC
                {
                    MaNv = nv.MaNv,
                    HoTen = nv.HoTen,
                    GioiTinh = nv.GioiTinh,
                    NgaySinh = nv.NgaySinh,
                    DiaChi = nv.DiaChi,
                    Sdt = nv.Sdt,
                    Email = nv.Email,
                    NgayVaoLam = nv.NgayVaoLam,
                    TenTaiKhoan = nv.TenTaiKhoan,
                    MaChucVu = nv.MaChucVu,
                    TinhTrang = nv.TinhTrang,
                }).ToList();
                response.SetSuccessResponse();
                response.SetData(staffDCs);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(400, ex);
            }
            return response;
        }

        #region [Other func]
        private async Task<List<DetailInvoiceDC>?> GetAllInvoiceDetailData()
        {
            // Lấy dữ liệu từ cơ sở dữ liệu
            var chitiethoadonList = await _db.Cthoadons.ToListAsync();
            var sanPhamList = await _db.Sanphams.ToListAsync();

            var chiTietSanPham = await _db.Chitietsanphams.Include(x => x.MaSpNavigation).Include(x => x.Hinhanhs).ToListAsync();

            // Chuyển đổi sang ChiTietHoaDonVM
            var result = chitiethoadonList.Select(chitiet =>
            {
                Chitietsanpham? sp = chiTietSanPham.FirstOrDefault(x => x.MaCtsp == chitiet.MaCtsp);

                return new DetailInvoiceDC
                {
                    MaHd = chitiet.MaHd,
                    MaSp = sp!.MaSp,
                    TenSanPham = sp?.MaSpNavigation.TenSanPham ?? "N/A",
                    KichThuoc = sp?.KichThuoc ?? "N/A",
                    SoLuong = chitiet.SoLuong,
                    DonGia = sp?.DonGia ?? 0,
                    TongTien = chitiet.SoLuong * sp?.DonGia ?? 0,
                    MoTa = sp?.MaSpNavigation.MoTa ?? "N/A",
                    LinkAnhDau = sp?.Hinhanhs.FirstOrDefault()?.TenHinhAnh ?? "null.png"
                };
            }).ToList();
            return result;
        }
        #endregion
    }
}