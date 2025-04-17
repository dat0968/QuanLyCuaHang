using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Repositories.Bill;
using APIQuanLyCuaHang.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OrderClientController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IBillRepository _billRepository;

        public OrderClientController(IUnitOfWork unit, IBillRepository billRepository )
        {
            _unit = unit;
            _billRepository = billRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _unit.HoaDonKhachs.GetAllInvoiceByUserId(userId);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatusOrder(int? orderId, string? statusChange, string? reasonCancel)
        {
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _unit.HoaDonKhachs.UpdateStatusOrderOfUser(userId, orderId, statusChange, reasonCancel);
            return Ok(response);
        }
        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequestDTO request)
        {
            if (request == null || request.OrderId <= 0)
            {
                return BadRequest(new { Success = false, Message = "ID đơn hàng không hợp lệ." });
            }
            if (string.IsNullOrEmpty(request.StatusChange))
            {
                return BadRequest(new { Success = false, Message = "Trạng thái mới là bắt buộc." });
            }

            try
            {
                // Debug: Log payload nhận được
                Console.WriteLine($"Received payload: OrderId={request.OrderId}, StatusChange={request.StatusChange}, ReasonCancel={request.ReasonCancel}");
                await _billRepository.CancelOrder(request.OrderId, request.StatusChange, request.ReasonCancel);
                return Ok(new { Success = true, Message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Lỗi khi cập nhật trạng thái: {ex.Message}" });
            }
        }
    }
}