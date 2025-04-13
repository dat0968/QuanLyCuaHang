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
    }
}