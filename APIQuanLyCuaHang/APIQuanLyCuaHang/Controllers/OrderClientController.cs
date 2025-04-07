using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OrderClientController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        public OrderClientController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int? userId)
        {
            // TODO: Remove the userId request to this API
            string? userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdString))
            {
                userId = Convert.ToInt32(userIdString);
            }
            var response = await _unit.HoaDonKhachs.GetAllInvoiceByUserId(userId);
            // response.Message += $"[{userId}]";
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatusOrder(int? userId, int? orderId, string? statusChange, string? reasonCancel)
        {
            var response = await _unit.HoaDonKhachs.UpdateStatusOrderOfUser(userId, orderId, statusChange, reasonCancel);
            return Ok(response);
        }
    }
}