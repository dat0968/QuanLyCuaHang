using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.DTO.Requests;
using APIQuanLyCuaHang.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Iana;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        public ScheduleController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _unit.Schedules.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> TimeKeeping([FromQuery] string qrCodeData)
        {
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _unit.Schedules.TimeKeepingAsync(userId.Value, qrCodeData);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> SetStatusList([FromBody] SetStatusListRequest request)
        {
            int? userManagerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _unit.Schedules.SetStatusList(request, userManagerId, null);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> SetStatusOne([FromBody] SetStatusOneRequest request)
        {
            int? userManagerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _unit.Schedules.SetStatusOne(request, userManagerId);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetScheduleActiveOfUser()
        {
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _unit.Schedules.GetScheduleOfUser(userId);
            return Ok(response);
        }
        [HttpGet("{shiftId}")]
        public async Task<IActionResult> GetScheduleActiveOfShift(int? shiftId)
        {
            var response = await _unit.Schedules.GetScheduleActiveOfShift(shiftId);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUserId()
        {
            var response = await _unit.Schedules.GetAllUserIdAsync();
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateSchedules([FromBody] SetStatusListRequest request)
        {
            int? userManagerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _unit.Schedules.SetStatusList(request, userManagerId, true);
            return Ok(result);
        }
    }
}
