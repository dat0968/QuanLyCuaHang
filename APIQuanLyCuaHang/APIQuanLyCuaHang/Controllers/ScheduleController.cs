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
        public async Task<IActionResult> SignUpScheduleWork([FromQuery] int maNv, [FromQuery] int maCaKip, [FromQuery] DateOnly ngayLam)
        {
            var result = await _unit.Schedules.SignUpScheduleWorkAsync(maNv, maCaKip, ngayLam);
            return Ok(result);
        }
        [HttpGet]
        public IActionResult GenerateQRCode([FromQuery] int maCaKip, [FromQuery] string ngayLam)
        {
            // ! Change This
            ResponseAPI<string> response = new();
            try
            {
                if (!DateOnly.TryParse(ngayLam, out DateOnly date))
                {
                    return BadRequest("Ngày làm không hợp lệ.");
                }

                string qrContent = $"{maCaKip}-{date}";

                response.SetSuccessResponse("Lấy dữ liệu QR thành công.");
                response.SetData(qrContent);
            }
            catch (Exception ex)
            {
                response.SetMessageResponseWithException(500, ex);
            }
            return Ok(response);
            // Mã cũ trả về ảnh QR (đã comment lại)
            /*
            byte[] qrCodeImage = QRCodeService.GenerateQRCode(qrContent);
            return File(qrCodeImage, "image/png");
            */
        }

        [HttpPost]
        public async Task<IActionResult> ChamCong([FromQuery] int maNv, [FromQuery] string qrCodeData)
        {
            // ! Change This
            var result = await _unit.Schedules.TimeKeepingAsync(maNv, qrCodeData);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> SetStatusList([FromBody] SetStatusListRequest request)
        {
            int? userManagerId = 112; // ! Change This: Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _unit.Schedules.SetStatusList(request, userManagerId);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> SetStatusOne([FromBody] SetStatusOneRequest request)
        {
            int? userManagerId = 112; // ! Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _unit.Schedules.SetStatusOne(request, userManagerId);
            return Ok(result);
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetScheduleActiveOfUser(int? userId)
        {
            var response = await _unit.Schedules.GetScheduleOfUser(userId);
            return Ok(response);
        }
        [HttpGet("{shiftId}")]
        public async Task<IActionResult> GetScheduleActiveOfShift(int? shiftId)
        {
            var response = await _unit.Schedules.GetScheduleActiveOfShift(shiftId);
            return Ok(response);
        }
    }
}
