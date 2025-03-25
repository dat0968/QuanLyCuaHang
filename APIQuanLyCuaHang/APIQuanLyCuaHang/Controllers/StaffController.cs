using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffRepository _staffRepository;
        private const string DefaultImagePath = "/uploads/default-image.jpg";

        public StaffController(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetStaff(
            string? search = null,
            string? gioiTinh = null,
            string? tinhTrang = null)
        {
            try
            {
                var staffList = await _staffRepository.GetAllStaff(search, gioiTinh, tinhTrang);

                if (staffList == null || !staffList.Any())
                {
                    return NotFound("Không tìm thấy nhân viên nào.");
                }

                return Ok(staffList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetStaffById(int id)
        {
            try
            {
                var staff = await _staffRepository.GetStaffById(id);
                if (staff == null)
                {
                    return NotFound($"Không tìm thấy nhân viên với ID {id}.");
                }

                return Ok(staff);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> AddStaff([FromForm] NhanvienDTO staffDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(staffDto.MatKhau))
                {
                    return BadRequest("Mật Khẩu là bắt buộc khi thêm mới nhân viên.");
                }
                if (staffDto.HinhAnh != null)
                {
                    var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(staffDto.HinhAnh.FileName).ToLower();
                    if (!validExtensions.Contains(extension))
                    {
                        return BadRequest("Định dạng file ảnh không hợp lệ. Chỉ chấp nhận .jpg, .jpeg, .png, .gif");
                    }

                    if (staffDto.HinhAnh.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest("Kích thước file ảnh không được vượt quá 5MB");
                    }
                }
                if (staffDto.HinhAnh == null && string.IsNullOrEmpty(staffDto.HinhAnhDuongDan))
                {
                    staffDto.HinhAnhDuongDan = DefaultImagePath;
                }

                var addedStaff = await _staffRepository.AddStaff(staffDto);
                return CreatedAtAction(nameof(GetStaffById), new { id = addedStaff.MaNv }, addedStaff);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message} {(ex.InnerException != null ? $"Inner Exception: {ex.InnerException.Message}" : "")}");
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UpdateStaff(int id, [FromForm] NhanvienDTO staffDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (staffDto.HinhAnh != null)
                {
                    var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(staffDto.HinhAnh.FileName).ToLower();
                    if (!validExtensions.Contains(extension))
                    {
                        return BadRequest("Định dạng file ảnh không hợp lệ. Chỉ chấp nhận .jpg, .jpeg, .png, .gif");
                    }

                    if (staffDto.HinhAnh.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest("Kích thước file ảnh không được vượt quá 5MB");
                    }
                }
                if (staffDto.HinhAnh == null && string.IsNullOrEmpty(staffDto.HinhAnhDuongDan))
                {
                    staffDto.HinhAnhDuongDan = DefaultImagePath;
                }

                var updatedStaff = await _staffRepository.UpdateStaff(id, staffDto);
                if (updatedStaff == null)
                {
                    return NotFound($"Không tìm thấy nhân viên với ID {id}.");
                }

                return Ok(updatedStaff);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message} {(ex.InnerException != null ? $"Inner Exception: {ex.InnerException.Message}" : "")}");
            }
        }

        [HttpPatch("{id}/toggle-delete")]
        public async Task<ActionResult> ToggleDeleteStaff(int id)
        {
            try
            {
                var result = await _staffRepository.ToggleDeleteStaff(id);
                if (!result)
                {
                    return NotFound($"Không tìm thấy nhân viên với ID {id}.");
                }

                return Ok(new { message = "Ẩn nhân viên thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message} {(ex.InnerException != null ? $"Inner Exception: {ex.InnerException.Message}" : "")}");
            }
        }
    }
}