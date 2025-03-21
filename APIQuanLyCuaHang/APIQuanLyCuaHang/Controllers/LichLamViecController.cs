using APIQuanLyCuaHang.Repositories.LichLamViec;
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

        [HttpPost]
        public async Task<IActionResult> DangKyCaLamViec([FromQuery] int maNv, [FromQuery] int maCaKip, [FromQuery] DateOnly ngayLam)
        {
            var result = await _lichLamViecRepo.DangKyCaLamViecAsync(maNv, maCaKip, ngayLam);
            return Ok(result);
        }
    }
}
