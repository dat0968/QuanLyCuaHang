using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HoaDonKhachController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        public HoaDonKhachController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int? userId)
        {
            var response = await _unit.HoaDonKhachs.GetAllInvoiceByUserId(userId);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatusOrder(int? userId, int? orderId, string? statusChange)
        {
            var response = await _unit.HoaDonKhachs.UpdateStatusOrderOfUser(userId, orderId, statusChange);
            return Ok(response);
        }
    }
}