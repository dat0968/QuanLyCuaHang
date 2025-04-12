using APIQuanLyCuaHang.Repositories.Customer;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IWebHostEnvironment _environment;

        public CustomerController(ICustomerRepository customerRepository, IWebHostEnvironment environment)
        {
            _customerRepository = customerRepository;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? sortBy = null)
        {
            try
            {
                var (totalRecords, data) = await _customerRepository.GetCustomersAsync(pageNumber, pageSize, searchTerm, sortBy);
                var response = new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = data
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đã có lỗi xảy ra", error = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            try
            {
                var findCustomer = await _customerRepository.GetCustomerByIdAsync(id);
                return Ok(new
                {
                    Success = true,
                    Data = findCustomer,
                    Message = "Lấy thông tin khách hàng thành công"
                });
            }
            catch(Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Message = ex.InnerException
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromForm] KhachhangDTO customerDto)
        {
            try
            {
                if (customerDto == null)
                    throw new ArgumentNullException(nameof(customerDto));

                var customer = new Khachhang
                {
                    HoTen = customerDto.HoTen,
                    GioiTinh = customerDto.GioiTinh,
                    NgaySinh = customerDto.NgaySinh,
                    DiaChi = customerDto.DiaChi,
                    Cccd = customerDto.Cccd,
                    Sdt = customerDto.Sdt,
                    Email = customerDto.Email,
                    TenTaiKhoan = customerDto.TenTaiKhoan,
                    MatKhau = customerDto.MatKhau,
                    HinhDaiDien = customerDto.HinhDaiDien,
                    NgayTao = customerDto.NgayTao != default ? customerDto.NgayTao : DateTime.Now,
                    TinhTrang = customerDto.TinhTrang,
                    IsDelete = customerDto.IsDelete
                };

                if (customerDto.Anh != null && customerDto.Anh.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(customerDto.Anh.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "Hinh", "AnhKhachHang", fileName);
                    var directory = Path.GetDirectoryName(filePath);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await customerDto.Anh.CopyToAsync(stream);
                    }

                    customer.HinhDaiDien = $"/Hinh/AnhKhachHang/{fileName}";
                }

                var addedCustomer = await _customerRepository.AddCustomerAsync(customer);
                return CreatedAtAction(nameof(GetCustomers), new { id = addedCustomer.MaKh }, new { message = "Thêm khách hàng thành công", data = addedCustomer });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đã có lỗi xảy ra", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromForm] KhachhangDTO customerDto)
        {
            try
            {
                if (customerDto == null || id != customerDto.MaKh)
                    throw new ArgumentException("Dữ liệu không hợp lệ");

                // Lấy thông tin khách hàng hiện tại từ cơ sở dữ liệu
                var existingCustomer = await _customerRepository.GetCustomerByIdAsync(id);
                if (existingCustomer == null)
                    throw new KeyNotFoundException("Khách hàng không tồn tại");

                // Tạo đối tượng khách hàng mới để cập nhật
                var customer = new Khachhang
                {
                    MaKh = customerDto.MaKh,
                    HoTen = customerDto.HoTen,
                    GioiTinh = customerDto.GioiTinh,
                    NgaySinh = customerDto.NgaySinh,
                    DiaChi = customerDto.DiaChi,
                    Cccd = customerDto.Cccd,
                    Sdt = customerDto.Sdt,
                    Email = string.IsNullOrWhiteSpace(customerDto.Email) ? existingCustomer.Email : customerDto.Email, // Giữ email cũ nếu không có email mới
                    TenTaiKhoan = customerDto.TenTaiKhoan,
                    MatKhau = customerDto.MatKhau,
                    HinhDaiDien = existingCustomer.HinhDaiDien, // Giữ nguyên ảnh cũ mặc định
                    NgayTao = existingCustomer.NgayTao, // Giữ nguyên ngày tạo cũ
                    TinhTrang = customerDto.TinhTrang,
                    IsDelete = customerDto.IsDelete
                };

                // Nếu có ảnh mới được tải lên, cập nhật ảnh
                if (customerDto.Anh != null && customerDto.Anh.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(customerDto.Anh.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "Hinh", "AnhKhachHang", fileName);
                    var directory = Path.GetDirectoryName(filePath);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await customerDto.Anh.CopyToAsync(stream);
                    }

                    // Xóa ảnh cũ nếu tồn tại
                    if (!string.IsNullOrEmpty(existingCustomer.HinhDaiDien))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, existingCustomer.HinhDaiDien.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    customer.HinhDaiDien = $"/Hinh/AnhKhachHang/{fileName}";
                }

                var updatedCustomer = await _customerRepository.UpdateCustomerAsync(id, customer);
                return Ok(new { message = "Cập nhật khách hàng thành công", data = updatedCustomer });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đã có lỗi xảy ra", error = ex.Message });
            }
        }

        [HttpPut("Hide/{id}")]
        public async Task<IActionResult> HideCustomer(int id)
        {
            try
            {
                var success = await _customerRepository.HideCustomerAsync(id);
                return Ok(new { message = "Ẩn khách hàng thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đã có lỗi xảy ra", error = ex.Message });
            }
        }

        // Thêm endpoint Export Excel
        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportCustomersToExcel(string? searchTerm = null, string? sortBy = null)
        {
            try
            {
                var fileBytes = await _customerRepository.ExportCustomersToExcelAsync(searchTerm, sortBy);
                var fileName = $"KhachHang_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đã có lỗi xảy ra khi xuất file Excel", error = ex.Message });
            }
        }

        // Thêm endpoint Import Excel
        [HttpPost("ImportExcel")]
        public async Task<IActionResult> ImportCustomersFromExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Vui lòng tải lên file Excel." });

                if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { message = "File phải có định dạng .xlsx." });

                using (var stream = file.OpenReadStream())
                {
                    var (successCount, errorCount, errors) = await _customerRepository.ImportCustomersFromExcelAsync(stream);
                    return Ok(new
                    {
                        message = "Nhập dữ liệu từ Excel hoàn tất.",
                        successCount,
                        errorCount,
                        errors
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đã có lỗi xảy ra khi nhập file Excel", error = ex.Message });
            }
        }
    }
}