using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Repositories.CaKip;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CaKipController : ControllerBase
    {
        private readonly ICaKipRepository _caKip;

        public CaKipController(ICaKipRepository caKip)
        {
            _caKip = caKip;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _caKip.GetAllAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllToday()
        {
            var result = await _caKip.GetAllTodayAsync();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await _caKip.RemoveAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpsertCrew([FromBody] CaKipDTO caKip, [FromQuery] bool isToday = false)
        {
            var result = await _caKip.UpsertCrewAsync(caKip, isToday);
            return Ok(result);
        }
    }
}
