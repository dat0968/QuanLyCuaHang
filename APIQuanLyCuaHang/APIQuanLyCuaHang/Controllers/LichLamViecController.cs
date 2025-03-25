using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.DTO.Requests;
using APIQuanLyCuaHang.Repositories.LichLamViec;
using APIQuanLyCuaHang.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LichLamViecController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        public LichLamViecController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _unit.LichLamViecs.GetAllAsync());
        }
        [HttpPost]
        public async Task<IActionResult> DangKyCaLamViec([FromQuery] int maNv, [FromQuery] int maCaKip, [FromQuery] DateOnly ngayLam)
        {
            var result = await _unit.LichLamViecs.DangKyCaLamViecAsync(maNv, maCaKip, ngayLam);
            return Ok(result);
        }
        [HttpGet]
        public IActionResult GenerateQRCode([FromQuery] int maCaKip, [FromQuery] string ngayLam)
        {
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
            var result = await _unit.LichLamViecs.ChamCongAsync(maNv, qrCodeData);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> SetStatusList([FromBody] SetStatusListRequest request)
        {
            int? userManagerId = 120; // Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _unit.LichLamViecs.SetStatusList(request, userManagerId);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> SetStatusOne([FromBody] SetStatusOneRequest request)
        {
            int? userManagerId = 120; // Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _unit.LichLamViecs.SetStatusOne(request, userManagerId);
            return Ok(result);
        }


    }
}
