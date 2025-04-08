using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.DTO.DataChart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIQuanLyCuaHang.Helpers.Utils;
using APIQuanLyCuaHang.Helpers.Constants;

namespace APIQuanLyCuaHang.Repositories.Dashboard
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly QuanLyCuaHangContext _db;

        public DashboardRepository(QuanLyCuaHangContext db)
        {
            _db = db;
        }

        public async Task<ResponseAPI<EarningData>> GetEarningDataAsync(string timeRange)
        {
            ResponseAPI<EarningData> response = new();
            try
            {
                var invoices = await GetAllInvoiceDataStatisticsAsync();
                var earningData = CalculateEarningData(invoices, timeRange);
                response.SetSuccessResponse("Doanh thu theo thời gian đã được lấy.");
                response.SetData(earningData);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(400, ex);
            }
            return response;
        }

        public async Task<ResponseAPI<OrderData>> GetAllOrderDataAsync()
        {
            try
            {
                var invoices = await GetAllInvoiceStatisticsAsync();
                var orderData = CalculateOrderData(invoices);
                return new ResponseAPI<OrderData> { Data = orderData, Success = true };
            }
            catch (Exception ex)
            {
                return new ResponseAPI<OrderData> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ResponseAPI<OrderStatusData>> GetOrderStatusDataAsync(string timeRange)
        {
            try
            {
                var invoices = await GetAllInvoiceStatisticsAsync(timeRange);
                var orderStatusData = CalculateOrderStatusData(invoices);
                return new ResponseAPI<OrderStatusData> { Data = orderStatusData, Success = true };
            }
            catch (Exception ex)
            {
                return new ResponseAPI<OrderStatusData> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ResponseAPI<OrderOverviewData>> GetOrderOverviewDataAsync(string timeRange)
        {
            try
            {
                var invoices = await GetAllInvoiceDataStatisticsAsync();
                var orderOverviewData = CalculateOrderOverviewData(invoices, timeRange);
                return new ResponseAPI<OrderOverviewData> { Data = orderOverviewData, Success = true };
            }
            catch (Exception ex)
            {
                return new ResponseAPI<OrderOverviewData> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ResponseAPI<TopSellingProductsData>> GetTopSellingProductsAsync()
        {
            try
            {
                var products = await GetAllInvoiceDetailData();
                var topSellingProductsData = CalculateTopSellingProductsData(products);
                return new ResponseAPI<TopSellingProductsData> { Data = topSellingProductsData, Success = true };
            }
            catch (Exception ex)
            {
                return new ResponseAPI<TopSellingProductsData> { Success = false, Message = ex.Message };
            }
        }

        // * Old: EmployeeStatisticsData
        public async Task<ResponseAPI<List<StaffDC>>> GetEmployeeOrderStatisticsAsync()
        {
            try
            {
                var employeeStats = await CalculateEmployeeOrderStatisticsAsync();
                return new ResponseAPI<List<StaffDC>> { Data = employeeStats, Success = true };
            }
            catch (Exception ex)
            {
                return new ResponseAPI<List<StaffDC>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ResponseAPI<UserStatisticsData>> GetUserStatisticsAsync()
        {
            try
            {
                var userStats = await CalculateUserStatisticsAsync();
                UserStatisticsData userStatisticsData = new() { Users = userStats };
                return new ResponseAPI<UserStatisticsData> { Data = userStatisticsData, Success = true };
            }
            catch (Exception ex)
            {
                return new ResponseAPI<UserStatisticsData> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ResponseAPI<ProductDC>> GetProductFullDetails(int id)
        {
            ResponseAPI<ProductDC> response = new();
            try
            {
                var productDb = await _db.Sanphams.FirstOrDefaultAsync(sp => sp.MaSp == id);
                var fullDetailOfProductDb = await _db.Chitietsanphams.Include(ctsp => ctsp.Hinhanhs).Where(ctsp => ctsp.MaSp == id).Select(ctsp => new DetailProductDC
                {
                    MaCtsp = ctsp.MaCtsp,
                    MaSp = ctsp.MaSp,
                    KichThuoc = ctsp.KichThuoc,
                    HuongVi = ctsp.HuongVi,
                    SoLuongTon = ctsp.SoLuongTon,
                    DonGia = ctsp.DonGia,
                    TenHinhAnh = ctsp.Hinhanhs.FirstOrDefault() != null ? ctsp.Hinhanhs.FirstOrDefault()!.TenHinhAnh : "null.png"
                }).ToListAsync();

                ProductDC productDC = new ProductDC
                {
                    MaSp = productDb!.MaSp,
                    MaDanhMuc = productDb.MaDanhMuc,
                    TenSanPham = productDb.TenSanPham,
                    MoTa = productDb.MoTa,
                    ChiTietSanPhams = fullDetailOfProductDb
                };
                response.SetSuccessResponse("Thông tin các chi tiết liên quan đến sản phẩm đã được lấy!");
                response.SetData(productDC);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(400, ex);
            }
            return response;
        }


        public async Task<ResponseAPI<WorkHistoryDC>> GetTopEmployeeRegistShift()
        {
            ResponseAPI<WorkHistoryDC> response = new();
            try
            {
                var dictionaryNameEmployee = await _db.Nhanviens.ToDictionaryAsync(nv => nv.MaNv, nv => nv.HoTen);

                var allWorkHistory = await _db.Lichsulamviecs.GroupBy(ls => ls.MaNv).Select(ls => new WorkHistoryDC
                {
                    MaNv = ls.Key,
                    TenNhanVien = dictionaryNameEmployee.GetValueOrDefault(ls.Key),
                    MaCaKip = ls.First().MaCaKip,
                    TongSoGioLam = ls.Sum(tg => tg.SoGioLam),
                    TongLuong = ls.Sum(tl => tl.TongLuong),
                }).OrderByDescending(wh => wh.TongLuong).ToListAsync();
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
            }
            return response;
        }

        public async Task<ResponseAPI<List<StatObjectDC>>> GetListStatObjectAsync()
        {
            ResponseAPI<List<StatObjectDC>> response = new();
            List<StatObjectDC> data = new();
            try
            {
                // ? Dữ liệu tình trạng khách hàng
                StatObjectDC statClient = new StatObjectDC
                {
                    NameObject = "Khách hàng",
                    AmountActive = await _db.Khachhangs.CountAsync(x => !(x.IsDelete ?? true)),
                    AmountInactive = await _db.Khachhangs.CountAsync(x => x.IsDelete ?? false)
                };
                data.Add(statClient);

                // ? Dữ liệu tình trạng nhân viên
                StatObjectDC statStaff = new StatObjectDC
                {
                    NameObject = "Nhân viên",
                    AmountActive = await _db.Nhanviens.CountAsync(x => !(x.IsDelete ?? true)),
                    AmountInactive = await _db.Nhanviens.CountAsync(x => x.IsDelete ?? false)
                };
                data.Add(statStaff);

                response.SetSuccessResponse("Lấy dữ liệu thống kê thành công.");
                response.SetData(data);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
            }
            return response;
        }
        #region [Private Methods]

        private async Task<List<InvoiceDC>> GetAllInvoiceDataStatisticsAsync()
        {
            var hoadonList = await _db.Hoadons.ToListAsync();
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
                TongTien = hoadon.TienGoc - hoadon.PhiVanChuyen
            }).ToList();

            var listInvoiceDetails = await GetAllInvoiceDetailData();
            if (listInvoiceDetails != null && listInvoiceDetails.Count != 0)
            {
                foreach (var invoice in result)
                {
                    invoice.SanPhamHoaDons = listInvoiceDetails.Where(dt => dt.MaHd == invoice.MaHd).ToList();
                }
            }

            return result;
        }

        private async Task<List<DetailInvoiceDC>> GetAllInvoiceDetailData()
        {
            var chitiethoadonList = await _db.Cthoadons.ToListAsync();
            var sanPhamList = await _db.Sanphams.ToListAsync();
            var chiTietSanPham = await _db.Chitietsanphams.Include(x => x.MaSpNavigation).Include(x => x.Hinhanhs).ToListAsync();

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

        private async Task<List<InvoiceDC>> GetAllInvoiceStatisticsAsync(string? timeRange = "all")
        {
            var hoadonList = await _db.Hoadons.ToListAsync();
            DateTime now = DateTime.Now;

            Func<DateTime, bool> filterTimeRange;
            switch (timeRange)
            {
                case TimeRangeConstants.Day:
                    filterTimeRange = x => x.Date == now.Date;
                    break;
                case TimeRangeConstants.Week:
                    filterTimeRange = FormatDate.GetDateFilter(TimeRangeConstants.Week, now);
                    break;
                case TimeRangeConstants.Month:
                    filterTimeRange = FormatDate.GetDateFilter(TimeRangeConstants.Month, now);
                    break;
                case TimeRangeConstants.Year:
                    filterTimeRange = FormatDate.GetDateFilter(TimeRangeConstants.Year, now);
                    break;
                default:
                    filterTimeRange = x => true;
                    break;
            }

            hoadonList = hoadonList.Where(hd => hd.NgayNhan.HasValue && filterTimeRange(hd.NgayNhan.Value)).ToList();

            return hoadonList.Select(hoadon => new InvoiceDC
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
                TongTien = hoadon.TienGoc - hoadon.PhiVanChuyen
            }).ToList();
        }

        private async Task<List<StaffDC>> CalculateEmployeeOrderStatisticsAsync()
        {
            try
            {
                var nhanviens = await _db.Nhanviens.ToListAsync();
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

                var allOrders = await _db.Hoadons.ToListAsync();

                foreach (var staff in staffDCs)
                {
                    var allOrderOfStaff = allOrders.Where(od => od.MaNv == staff.MaNv).ToList();
                    if (allOrderOfStaff != null && allOrderOfStaff.Count != 0)
                    {
                        staff.SoDonHangDamNhan = allOrderOfStaff.Count;
                        staff.DoanhThuMangLai = allOrderOfStaff.Sum(ods => ods.TienGoc - ods.PhiVanChuyen);
                    }
                }

                return staffDCs;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return null;
            }
        }

        private async Task<List<UserStatistics>> CalculateUserStatisticsAsync()
        {
            var nhanviens = await _db.Nhanviens.ToListAsync();
            return nhanviens.Select(nv => new UserStatistics
            {
                UserType = "Employee",
                Active = nhanviens.Count(n => n.TinhTrang == "Active"),
                Inactive = nhanviens.Count(n => n.TinhTrang == "Inactive")
            }).ToList();
        }

        // Các phương thức tính toán dữ liệu thống kê khác sẽ được định nghĩa ở đây
        private EarningData CalculateEarningData(List<InvoiceDC> invoices, string timeRange)
        {
            var earningData = new EarningData
            {
                Name = "Doanh thu",
                Data = new List<decimal>(),
                Categories = new List<string>()
            };

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            switch (timeRange)
            {
                case "day":
                    earningData.Data = TimeRangeConstants.DaysOfWeek.Select(day =>
                        invoices
                            .Where(x => x.NgayTao.DayOfWeek.ToString().StartsWith(day.Substring(0, 3)))
                            .Sum(x => x.TongTien)
                    ).ToList();
                    earningData.Categories = TimeRangeConstants.DaysOfWeek.ToList();
                    break;

                case "week":
                    var weeksInMonth = Enumerable.Range(1, 4);
                    earningData.Data = weeksInMonth.Select(week =>
                        invoices
                            .Where(x => x.NgayTao.Month == today.Month)
                            .Where(x => (x.NgayTao.Day - 1) / 7 + 1 == week)
                            .Sum(x => x.TongTien)
                    ).ToList();
                    earningData.Categories = weeksInMonth.Select(week => "Tuần " + week).ToList();
                    break;

                case "month":
                    int currentYear = today.Year;
                    earningData.Data = Enumerable.Range(1, 12).Select(month =>
                        invoices
                            .Where(x => x.NgayTao.Month == month && x.NgayTao.Year == currentYear)
                            .Sum(x => x.TongTien)
                    ).ToList();
                    earningData.Categories = Enumerable.Range(1, 12).Select(month => "Tháng " + month).ToList();
                    break;

                case "year":
                    currentYear = today.Year;
                    var recentYears = Enumerable.Range(currentYear - 5, 5);
                    earningData.Data = recentYears.Select(year =>
                        invoices
                            .Where(x => x.NgayTao.Year == year)
                            .Sum(x => x.TongTien)
                    ).ToList();
                    earningData.Categories = recentYears.Select(year => year.ToString()).ToList();
                    break;

                default:
                    throw new ArgumentException("Invalid time range specified.");
            }

            return earningData;
        }

        private OrderStatusData CalculateOrderStatusData(List<InvoiceDC> invoices)
        {
            var orderStatusData = new OrderStatusData
            {
                Labels = new List<string> { "Đã xác nhận", "Đã giao cho đơn vị vận chuyển", "Đang giao hàng", "Chờ thanh toán", "Hoàn trả/Hoàn tiền", "Đã hủy", "Chờ xác nhận" },
                Data = new List<int>()
            };

            foreach (var status in orderStatusData.Labels)
            {
                orderStatusData.Data.Add(invoices.Count(x => x.TinhTrang == status));
            }

            return orderStatusData;
        }
        private OrderOverviewData CalculateOrderOverviewData(List<InvoiceDC> invoices, string timeRange)
        {
            var orderOverviewData = new OrderOverviewData
            {
                Overview = new List<OrderOverview>(),
                Categories = new List<string>(),
                TotalOrders = invoices.Count()
            };

            var orderStatuses = new[] { "Đã xác nhận", "Đã giao cho đơn vị vận chuyển", "Đang giao hàng", "Chờ thanh toán", "Hoàn trả/Hoàn tiền", "Đã hủy", "Chờ xác nhận" };

            foreach (var status in orderStatuses)
            {
                var orderData = new OrderOverview
                {
                    Name = status,
                    Data = new List<ParameterDataOrder>()
                };

                switch (timeRange)
                {
                    case "day":
                        var daysOfWeek = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
                        orderData.Data = daysOfWeek.Select(day =>
                        {
                            var filteredOrders = invoices
                                .Where(x => x.TinhTrang == status && x.NgayTao.DayOfWeek.ToString().StartsWith(day.Substring(0, 3)));
                            return new ParameterDataOrder
                            {
                                Count = filteredOrders.Count(),
                                Revenue = filteredOrders.Sum(x => x.TongTien)
                            };
                        }).ToList();
                        orderOverviewData.Categories = daysOfWeek.ToList();
                        break;

                    case "week":
                        var weeksInMonth = Enumerable.Range(1, 4);
                        orderData.Data = weeksInMonth.Select(week =>
                        {
                            var filteredOrders = invoices
                                .Where(x => x.TinhTrang == status && (x.NgayTao.Day - 1) / 7 + 1 == week);
                            return new ParameterDataOrder
                            {
                                Count = filteredOrders.Count(),
                                Revenue = filteredOrders.Sum(x => x.TongTien)
                            };
                        }).ToList();
                        orderOverviewData.Categories = weeksInMonth.Select(week => "Tuần " + week).ToList();
                        break;

                    case "month":
                        int currentYear = DateTime.Today.Year;
                        orderData.Data = Enumerable.Range(1, 12).Select(month =>
                        {
                            var filteredOrders = invoices
                                .Where(x => x.TinhTrang == status && x.NgayTao.Month == month && x.NgayTao.Year == currentYear);
                            return new ParameterDataOrder
                            {
                                Count = filteredOrders.Count(),
                                Revenue = filteredOrders.Sum(x => x.TongTien)
                            };
                        }).ToList();
                        orderOverviewData.Categories = Enumerable.Range(1, 12).Select(month => "Tháng " + month).ToList();
                        break;

                    case "year":
                        int currentYearForOverview = DateTime.Today.Year;
                        var recentYears = Enumerable.Range(currentYearForOverview - 5, 5);
                        orderData.Data = recentYears.Select(year =>
                        {
                            var filteredOrders = invoices
                                .Where(x => x.TinhTrang == status && x.NgayTao.Year == year);
                            return new ParameterDataOrder
                            {
                                Count = filteredOrders.Count(),
                                Revenue = filteredOrders.Sum(x => x.TongTien)
                            };
                        }).ToList();
                        orderOverviewData.Categories = recentYears.Select(year => year.ToString()).ToList();
                        break;

                    default:
                        throw new ArgumentException("Invalid time range specified.");
                }

                orderOverviewData.Overview.Add(orderData);
            }

            return orderOverviewData;
        }
        private TopSellingProductsData CalculateTopSellingProductsData(List<DetailInvoiceDC> products)
        {
            var topSellingProductsData = new TopSellingProductsData
            {
                Products = products.GroupBy(p => p.MaSp)
                    .Select(g => new ProductStatistics
                    {
                        ProductId = g.Key.ToString(),
                        ProductName = g.First().TenSanPham,
                        Quantity = g.Sum(x => x.SoLuong),
                        TotalRevenue = g.Sum(x => x.TongTien ?? 0)
                    })
                    .OrderByDescending(x => x.Quantity)
                    .Take(5) // Lấy 5 sản phẩm bán chạy nhất
                    .ToList()
            };

            return topSellingProductsData;
        }

        private OrderData CalculateOrderData(List<InvoiceDC> invoices)
        {
            var orderData = new OrderData
            {
                ApprovedOrders = invoices.Count(x => x.TinhTrang == "Đã xác nhận"),
                PendingOrders = invoices.Count(x => x.TinhTrang == "Chờ thanh toán"),
                InProgressOrders = invoices.Count() - (invoices.Count(x => x.TinhTrang == "Đã xác nhận") + invoices.Count(x => x.TinhTrang == "Chờ thanh toán"))
            };

            return orderData;
        }
        #endregion
    }
}