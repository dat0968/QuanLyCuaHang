using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using BCrypt.Net;
using OfficeOpenXml;

namespace APIQuanLyCuaHang.Repositories.Customer
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly QuanLyCuaHangContext _context;

        // Định nghĩa các giá trị hợp lệ cho TinhTrang
        private readonly string[] ValidTinhTrangValues = { "Đang hoạt động", "Đã tạm khóa" };

        public CustomerRepository(QuanLyCuaHangContext context)
        {
            _context = context;
        }

        public async Task<(int TotalRecords, List<KhachhangDTO> Data)> GetCustomersAsync(int pageNumber, int pageSize, string? searchTerm, string? sortBy)
        {
            var query = _context.Khachhangs
                .Where(kh => kh.IsDelete != true)
                .Select(kh => new KhachhangDTO
                {
                    MaKh = kh.MaKh,
                    HoTen = kh.HoTen,
                    GioiTinh = kh.GioiTinh,
                    NgaySinh = kh.NgaySinh,
                    DiaChi = kh.DiaChi,
                    Cccd = kh.Cccd,
                    Sdt = kh.Sdt,
                    Email = kh.Email,
                    TenTaiKhoan = kh.TenTaiKhoan,
                    MatKhau = kh.MatKhau,
                    HinhDaiDien = kh.HinhDaiDien,
                    NgayTao = kh.NgayTao,
                    TinhTrang = kh.TinhTrang,
                    IsDelete = kh.IsDelete
                });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(kh =>
                    kh.HoTen.ToLower().Contains(searchTerm) ||
                    kh.Sdt.ToLower().Contains(searchTerm) ||
                    (kh.Email != null && kh.Email.ToLower().Contains(searchTerm)));
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                sortBy = sortBy.Trim();
                switch (sortBy.ToLower())
                {
                    case "gioitinh":
                        query = query.OrderBy(kh => kh.GioiTinh);
                        break;
                    case "tinhtrang":
                        query = query.OrderBy(kh => kh.TinhTrang);
                        break;
                    case "a-z":
                        query = query.OrderBy(kh => kh.HoTen);
                        break;
                    default:
                        query = query.OrderBy(kh => kh.MaKh);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(kh => kh.MaKh);
            }

            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalRecords, data);
        }

        public async Task<Khachhang> AddCustomerAsync(Khachhang customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            // Loại bỏ khoảng trắng thừa
            customer.HoTen = customer.HoTen?.Trim();
            customer.GioiTinh = customer.GioiTinh?.Trim();
            customer.DiaChi = customer.DiaChi?.Trim();
            customer.Cccd = customer.Cccd?.Trim();
            customer.Sdt = customer.Sdt?.Trim();
            customer.Email = customer.Email?.Trim();
            customer.TenTaiKhoan = customer.TenTaiKhoan?.Trim();
            customer.MatKhau = customer.MatKhau?.Trim();
            customer.TinhTrang = customer.TinhTrang?.Trim();

            // Validate: Không để trống các trường
            if (string.IsNullOrWhiteSpace(customer.HoTen))
                throw new ArgumentException("Họ và tên không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.GioiTinh))
                throw new ArgumentException("Giới tính không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.DiaChi))
                throw new ArgumentException("Địa chỉ không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.Cccd))
                throw new ArgumentException("CCCD không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.Sdt))
                throw new ArgumentException("Số điện thoại không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.Email))
                throw new ArgumentException("Email không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.TenTaiKhoan))
                throw new ArgumentException("Tên tài khoản không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.MatKhau))
                throw new ArgumentException("Mật khẩu không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.TinhTrang))
                throw new ArgumentException("Tình trạng không được để trống.");

            // Validate: TinhTrang chỉ được phép là "Đang hoạt động" hoặc "Tạm dừng"
            if (!ValidTinhTrangValues.Contains(customer.TinhTrang))
                throw new ArgumentException("Tình trạng chỉ được phép là 'Đang hoạt động' hoặc 'Tạm dừng'.");

            // Validate định dạng CCCD (12 số)
            if (!Regex.IsMatch(customer.Cccd, @"^\d{12}$"))
                throw new ArgumentException("CCCD phải là 12 chữ số.");

            // Validate định dạng SĐT (10-11 số, bắt đầu bằng 0)
            if (!Regex.IsMatch(customer.Sdt, @"^0\d{9,10}$"))
                throw new ArgumentException("Số điện thoại phải bắt đầu bằng 0 và có 10-11 chữ số.");

            // Validate định dạng Email
            if (!Regex.IsMatch(customer.Email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                throw new ArgumentException("Email không hợp lệ.");

            // Kiểm tra trùng lặp
            if (await _context.Khachhangs.AnyAsync(kh => kh.Cccd == customer.Cccd && kh.IsDelete != true))
                throw new InvalidOperationException("CCCD đã tồn tại.");
            if (await _context.Khachhangs.AnyAsync(kh => kh.Sdt == customer.Sdt && kh.IsDelete != true))
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");
            if (await _context.Khachhangs.AnyAsync(kh => kh.Email == customer.Email && kh.IsDelete != true))
                throw new InvalidOperationException("Email đã tồn tại.");
            if (await _context.Khachhangs.AnyAsync(kh => kh.TenTaiKhoan == customer.TenTaiKhoan && kh.IsDelete != true))
                throw new InvalidOperationException("Tên tài khoản đã tồn tại.");

            // Hash mật khẩu trước khi lưu
            customer.MatKhau = BCrypt.Net.BCrypt.HashPassword(customer.MatKhau);

            customer.NgayTao = customer.NgayTao != default ? customer.NgayTao : DateTime.Now;
            customer.IsDelete = false;

            _context.Khachhangs.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<Khachhang> UpdateCustomerAsync(int id, Khachhang customer)
        {
            if (customer == null || id != customer.MaKh)
                throw new ArgumentException("Dữ liệu không hợp lệ");

            var existingCustomer = await _context.Khachhangs
                .FirstOrDefaultAsync(kh => kh.MaKh == id && kh.IsDelete != true);
            if (existingCustomer == null)
                throw new KeyNotFoundException("Khách hàng không tồn tại");

            // Loại bỏ khoảng trắng thừa
            customer.HoTen = customer.HoTen?.Trim();
            customer.GioiTinh = customer.GioiTinh?.Trim();
            customer.DiaChi = customer.DiaChi?.Trim();
            customer.Cccd = customer.Cccd?.Trim();
            customer.Sdt = customer.Sdt?.Trim();
            customer.Email = customer.Email?.Trim();
            customer.TenTaiKhoan = customer.TenTaiKhoan?.Trim();
            customer.MatKhau = customer.MatKhau?.Trim();
            customer.TinhTrang = customer.TinhTrang?.Trim();

            // Validate: Không để trống các trường bắt buộc
            if (string.IsNullOrWhiteSpace(customer.HoTen))
                throw new ArgumentException("Họ và tên không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.GioiTinh))
                throw new ArgumentException("Giới tính không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.DiaChi))
                throw new ArgumentException("Địa chỉ không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.Cccd))
                throw new ArgumentException("CCCD không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.Sdt))
                throw new ArgumentException("Số điện thoại không được để trống.");
            if (string.IsNullOrWhiteSpace(customer.TinhTrang))
                throw new ArgumentException("Tình trạng không được để trống.");

            // Validate: TinhTrang chỉ được phép là "Đang hoạt động" hoặc "Tạm dừng"
            if (!ValidTinhTrangValues.Contains(customer.TinhTrang))
                throw new ArgumentException("Tình trạng chỉ được phép là 'Đang hoạt động' hoặc 'Tạm dừng'.");

            // Validate định dạng CCCD (12 số)
            if (!Regex.IsMatch(customer.Cccd, @"^\d{12}$"))
                throw new ArgumentException("CCCD phải là 12 chữ số.");

            // Validate định dạng SĐT (10-11 số, bắt đầu bằng 0)
            if (!Regex.IsMatch(customer.Sdt, @"^0\d{9,10}$"))
                throw new ArgumentException("Số điện thoại phải bắt đầu bằng 0 và có 10-11 chữ số.");

            // Validate định dạng Email nếu không null hoặc không rỗng
            if (!string.IsNullOrWhiteSpace(customer.Email) && !Regex.IsMatch(customer.Email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                throw new ArgumentException("Email không hợp lệ.");

            // Kiểm tra trùng lặp (ngoại trừ chính bản thân khách hàng đang cập nhật)
            if (await _context.Khachhangs.AnyAsync(kh => kh.Cccd == customer.Cccd && kh.MaKh != id && kh.IsDelete != true))
                throw new InvalidOperationException("CCCD đã tồn tại.");
            if (await _context.Khachhangs.AnyAsync(kh => kh.Sdt == customer.Sdt && kh.MaKh != id && kh.IsDelete != true))
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");
            if (!string.IsNullOrWhiteSpace(customer.Email) && await _context.Khachhangs.AnyAsync(kh => kh.Email == customer.Email && kh.MaKh != id && kh.IsDelete != true))
                throw new InvalidOperationException("Email đã tồn tại.");
            if (!string.IsNullOrWhiteSpace(customer.TenTaiKhoan) && await _context.Khachhangs.AnyAsync(kh => kh.TenTaiKhoan == customer.TenTaiKhoan && kh.MaKh != id && kh.IsDelete != true))
                throw new InvalidOperationException("Tên tài khoản đã tồn tại.");

            // Hash mật khẩu nếu có thay đổi và không null
            if (!string.IsNullOrWhiteSpace(customer.MatKhau) && customer.MatKhau != existingCustomer.MatKhau)
            {
                customer.MatKhau = BCrypt.Net.BCrypt.HashPassword(customer.MatKhau);
            }

            // Cập nhật các trường của khách hàng hiện tại
            existingCustomer.HoTen = customer.HoTen;
            existingCustomer.GioiTinh = customer.GioiTinh;
            existingCustomer.NgaySinh = customer.NgaySinh;
            existingCustomer.DiaChi = customer.DiaChi;
            existingCustomer.Cccd = customer.Cccd;
            existingCustomer.Sdt = customer.Sdt;
            existingCustomer.Email = customer.Email;
            existingCustomer.TenTaiKhoan = customer.TenTaiKhoan;
            existingCustomer.MatKhau = customer.MatKhau;
            existingCustomer.HinhDaiDien = customer.HinhDaiDien;
            existingCustomer.NgayTao = customer.NgayTao;
            existingCustomer.TinhTrang = customer.TinhTrang;
            existingCustomer.IsDelete = customer.IsDelete;

            await _context.SaveChangesAsync();

            return existingCustomer;
        }

        public async Task<bool> HideCustomerAsync(int id)
        {
            var customer = await _context.Khachhangs
                .FirstOrDefaultAsync(kh => kh.MaKh == id && kh.IsDelete != true);
            if (customer == null)
                throw new KeyNotFoundException("Khách hàng không tồn tại");

            customer.IsDelete = true;
            customer.Email = null;
            customer.TenTaiKhoan = null;
            customer.MatKhau = null;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<byte[]> ExportCustomersToExcelAsync(string? searchTerm, string? sortBy)
        {
            var query = _context.Khachhangs
                .Where(kh => kh.IsDelete != true)
                .Select(kh => new KhachhangDTO
                {
                    MaKh = kh.MaKh,
                    HoTen = kh.HoTen,
                    GioiTinh = kh.GioiTinh,
                    NgaySinh = kh.NgaySinh,
                    DiaChi = kh.DiaChi,
                    Cccd = kh.Cccd,
                    Sdt = kh.Sdt,
                    Email = kh.Email,
                    HinhDaiDien = kh.HinhDaiDien,
                    NgayTao = kh.NgayTao,
                    TinhTrang = kh.TinhTrang,
                    IsDelete = kh.IsDelete
                });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(kh =>
                    kh.HoTen.ToLower().Contains(searchTerm) ||
                    kh.Sdt.ToLower().Contains(searchTerm) ||
                    (kh.Email != null && kh.Email.ToLower().Contains(searchTerm)));
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                sortBy = sortBy.Trim();
                switch (sortBy.ToLower())
                {
                    case "gioitinh":
                        query = query.OrderBy(kh => kh.GioiTinh);
                        break;
                    case "tinhtrang":
                        query = query.OrderBy(kh => kh.TinhTrang);
                        break;
                    case "a-z":
                        query = query.OrderBy(kh => kh.HoTen);
                        break;
                    default:
                        query = query.OrderBy(kh => kh.MaKh);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(kh => kh.MaKh);
            }

            var customers = await query.ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("KhachHang");

                // Thiết lập font Times New Roman, kích thước 14 cho toàn bộ worksheet
                worksheet.Cells.Style.Font.Name = "Times New Roman";
                worksheet.Cells.Style.Font.Size = 14;

                // Thêm tiêu đề cột
                worksheet.Cells[1, 1].Value = "Mã KH";
                worksheet.Cells[1, 2].Value = "Họ Tên";
                worksheet.Cells[1, 3].Value = "Giới Tính";
                worksheet.Cells[1, 4].Value = "Ngày Sinh";
                worksheet.Cells[1, 5].Value = "Địa Chỉ";
                worksheet.Cells[1, 6].Value = "CCCD";
                worksheet.Cells[1, 7].Value = "Số Điện Thoại";
                worksheet.Cells[1, 8].Value = "Email";
                worksheet.Cells[1, 10].Value = "Ngày Tạo";
                worksheet.Cells[1, 11].Value = "Tình Trạng";

                // Định dạng tiêu đề: in đậm, background màu xám
                using (var range = worksheet.Cells[1, 1, 1, 12])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    // Thêm viền cho tiêu đề
                    range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                // Thêm dữ liệu
                for (int i = 0; i < customers.Count; i++)
                {
                    var customer = customers[i];
                    worksheet.Cells[i + 2, 1].Value = customer.MaKh;
                    worksheet.Cells[i + 2, 2].Value = customer.HoTen;
                    worksheet.Cells[i + 2, 3].Value = customer.GioiTinh;
                    worksheet.Cells[i + 2, 4].Value = customer.NgaySinh?.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 5].Value = customer.DiaChi;
                    worksheet.Cells[i + 2, 6].Value = customer.Cccd;
                    worksheet.Cells[i + 2, 7].Value = customer.Sdt;
                    worksheet.Cells[i + 2, 8].Value = customer.Email;
                    worksheet.Cells[i + 2, 10].Value = customer.NgayTao.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[i + 2, 11].Value = customer.TinhTrang;

                    // In đậm cột "Họ Tên" và "Giới Tính" cho từng hàng
                    worksheet.Cells[i + 2, 2].Style.Font.Bold = true; // Họ Tên
                    worksheet.Cells[i + 2, 3].Style.Font.Bold = true; // Giới Tính

                    // Thêm viền cho từng ô trong hàng dữ liệu
                    using (var range = worksheet.Cells[i + 2, 1, i + 2, 12])
                    {
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                }

                // Tự động điều chỉnh độ rộng cột
                worksheet.Cells.AutoFitColumns();

                // Trả về file Excel dưới dạng byte array
                return package.GetAsByteArray();
            }
        }

        public async Task<(int SuccessCount, int ErrorCount, List<string> Errors)> ImportCustomersFromExcelAsync(Stream excelStream)
        {
            var successCount = 0;
            var errorCount = 0;
            var errors = new List<string>();

            try
            {
                using (var package = new ExcelPackage(excelStream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        errors.Add("File Excel không có worksheet nào.");
                        return (successCount, errorCount, errors);
                    }

                    var rowCount = worksheet.Dimension?.Rows ?? 0;
                    if (rowCount < 2)
                    {
                        errors.Add("File Excel không có dữ liệu (ít nhất cần 2 hàng: tiêu đề và dữ liệu).");
                        return (successCount, errorCount, errors);
                    }

                    var expectedHeaders = new[] { "Mã KH", "Họ Tên", "Giới Tính", "Ngày Sinh", "Địa Chỉ", "CCCD", "Số Điện Thoại", "Email", "Tên Tài Khoản", "Mật Khẩu", "Hình Đại Diện", "Ngày Tạo", "Tình Trạng", "IsDelete" };
                    for (int col = 1; col <= expectedHeaders.Length; col++)
                    {
                        var header = worksheet.Cells[1, col].Text?.Trim();
                        if (header != expectedHeaders[col - 1])
                        {
                            errors.Add($"Cột {col} phải có tiêu đề là '{expectedHeaders[col - 1]}', nhưng tìm thấy '{header}'.");
                            return (successCount, errorCount, errors);
                        }
                    }

                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var customer = new Khachhang
                            {
                                MaKh = int.TryParse(worksheet.Cells[row, 1].Text, out var maKh) ? maKh : 0,
                                HoTen = worksheet.Cells[row, 2].Text?.Trim(),
                                GioiTinh = worksheet.Cells[row, 3].Text?.Trim(),
                                NgaySinh = DateOnly.TryParse(worksheet.Cells[row, 4].Text, out var ngaySinh) ? ngaySinh : (DateOnly?)null,
                                DiaChi = worksheet.Cells[row, 5].Text?.Trim(),
                                Cccd = worksheet.Cells[row, 6].Text?.Trim(),
                                Sdt = worksheet.Cells[row, 7].Text?.Trim(),
                                Email = string.IsNullOrWhiteSpace(worksheet.Cells[row, 8].Text) ? null : worksheet.Cells[row, 8].Text?.Trim(),
                                TenTaiKhoan = string.IsNullOrWhiteSpace(worksheet.Cells[row, 9].Text) ? null : worksheet.Cells[row, 9].Text?.Trim(),
                                MatKhau = string.IsNullOrWhiteSpace(worksheet.Cells[row, 10].Text) ? null : worksheet.Cells[row, 10].Text?.Trim(),
                                HinhDaiDien = string.IsNullOrWhiteSpace(worksheet.Cells[row, 11].Text) ? null : worksheet.Cells[row, 11].Text?.Trim(),
                                NgayTao = DateTime.TryParse(worksheet.Cells[row, 12].Text, out var ngayTao) ? ngayTao : DateTime.Now,
                                TinhTrang = worksheet.Cells[row, 13].Text?.Trim(),
                                IsDelete = bool.TryParse(worksheet.Cells[row, 14].Text, out var isDelete) ? isDelete : false
                            };

                            if (customer.MaKh == 0 && _context.Khachhangs.Any(kh => kh.MaKh == customer.MaKh))
                            {
                                throw new ArgumentException("Mã KH không hợp lệ hoặc đã tồn tại.");
                            }

                            if (string.IsNullOrWhiteSpace(customer.HoTen))
                                throw new ArgumentException("Họ và tên không được để trống.");
                            if (string.IsNullOrWhiteSpace(customer.GioiTinh))
                                throw new ArgumentException("Giới tính không được để trống.");
                            if (string.IsNullOrWhiteSpace(customer.DiaChi))
                                throw new ArgumentException("Địa chỉ không được để trống.");
                            if (string.IsNullOrWhiteSpace(customer.Cccd))
                                throw new ArgumentException("CCCD không được để trống.");
                            if (string.IsNullOrWhiteSpace(customer.Sdt))
                                throw new ArgumentException("Số điện thoại không được để trống.");
                            if (string.IsNullOrWhiteSpace(customer.TinhTrang))
                                throw new ArgumentException("Tình trạng không được để trống.");

                            // Validate: TinhTrang chỉ được phép là "Đang hoạt động" hoặc "Tạm dừng"
                            if (!ValidTinhTrangValues.Contains(customer.TinhTrang))
                                throw new ArgumentException("Tình trạng chỉ được phép là 'Đang hoạt động' hoặc 'Tạm dừng'.");

                            if (!Regex.IsMatch(customer.Cccd, @"^\d{12}$"))
                                throw new ArgumentException("CCCD phải là 12 chữ số.");

                            if (!Regex.IsMatch(customer.Sdt, @"^0\d{9,10}$"))
                                throw new ArgumentException("Số điện thoại phải bắt đầu bằng 0 và có 10-11 chữ số.");

                            if (!string.IsNullOrWhiteSpace(customer.Email) && !Regex.IsMatch(customer.Email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                                throw new ArgumentException("Email không hợp lệ.");

                            if (await _context.Khachhangs.AnyAsync(kh => kh.Cccd == customer.Cccd && kh.IsDelete != true))
                                throw new InvalidOperationException("CCCD đã tồn tại.");
                            if (await _context.Khachhangs.AnyAsync(kh => kh.Sdt == customer.Sdt && kh.IsDelete != true))
                                throw new InvalidOperationException("Số điện thoại đã tồn tại.");
                            if (!string.IsNullOrWhiteSpace(customer.Email) && await _context.Khachhangs.AnyAsync(kh => kh.Email == customer.Email && kh.IsDelete != true))
                                throw new InvalidOperationException("Email đã tồn tại.");
                            if (!string.IsNullOrWhiteSpace(customer.TenTaiKhoan) && await _context.Khachhangs.AnyAsync(kh => kh.TenTaiKhoan == customer.TenTaiKhoan && kh.IsDelete != true))
                                throw new InvalidOperationException("Tên tài khoản đã tồn tại.");

                            if (!string.IsNullOrWhiteSpace(customer.MatKhau))
                            {
                                customer.MatKhau = BCrypt.Net.BCrypt.HashPassword(customer.MatKhau);
                            }

                            _context.Khachhangs.Add(customer);
                            await _context.SaveChangesAsync();
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            errors.Add($"Hàng {row}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Lỗi khi xử lý file Excel: {ex.Message}");
            }

            return (successCount, errorCount, errors);
        }

        public async Task<Khachhang> GetCustomerByIdAsync(int id)
        {
            var customer = await _context.Khachhangs.AsNoTracking()
                .FirstOrDefaultAsync(kh => kh.MaKh == id && kh.IsDelete != true);

            if (customer == null)
            {
                throw new KeyNotFoundException("Khách hàng không tồn tại");
            }

            return customer;
        }
    }
}