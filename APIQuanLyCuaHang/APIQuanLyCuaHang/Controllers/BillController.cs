using APIQuanLyCuaHang.Repositories.Bill;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillRepository _billRepository;
        public BillController(IBillRepository billRepository)
        {
            _billRepository = billRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _billRepository.GetAllBill();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var hoaDon = await _billRepository.GetById(id);
            if (hoaDon == null)
            {
                return NotFound(new { message = "Không tìm thấy hóa đơn!" });
            }
            return Ok(hoaDon);
        }

        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                await _billRepository.UpdateStatus(id, request.TinhTrang, request.MaNv);
                return Ok(new { message = "Cập nhật trạng thái hóa đơn thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public class UpdateStatusRequest
        {
            public string TinhTrang { get; set; }
            public int? MaNv { get; set; }
        }
        [HttpGet("search")]
        public async Task<IActionResult> GetFiltered([FromQuery] string? hoTen, [FromQuery] string? hinhThucTt, [FromQuery] string? tinhTrang, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (orders, totalItems) = await _billRepository.GetFilteredBill(hoTen, hinhThucTt, tinhTrang, page, pageSize);
            return Ok(new { orders, totalItems });
        }
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetBillDetails(int id)
        {
            var billDetails = await _billRepository.GetBillDetails(id);
            if (billDetails == null)
            {
                return NotFound(new { message = "Không tìm thấy hóa đơn!" });
            }
            return Ok(billDetails);
        }
    }
}
