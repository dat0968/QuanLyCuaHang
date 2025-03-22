using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Repositories.LichLamViec;
using APIQuanLyCuaHang.Services.QRCode;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LichLamViecController : ControllerBase
    {
        private readonly ILichLamViecRepository _lichLamViecRepo;

        public LichLamViecController(ILichLamViecRepository lichLamViecRepo)
        {
            _lichLamViecRepo = lichLamViecRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _lichLamViecRepo.GetAllAsync());
        }
        [HttpPost]
        public async Task<IActionResult> DangKyCaLamViec([FromQuery] int maNv, [FromQuery] int maCaKip, [FromQuery] DateOnly ngayLam)
        {
            var result = await _lichLamViecRepo.DangKyCaLamViecAsync(maNv, maCaKip, ngayLam);
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
            var result = await _lichLamViecRepo.ChamCongAsync(maNv, qrCodeData);
            return Ok(result);
        }

    }
}
