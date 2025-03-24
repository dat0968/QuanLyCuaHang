using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Repositories.CaKip;
using APIQuanLyCuaHang.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CaKipController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        public CaKipController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _unit.CaKips.GetAllAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Employees(int? id)
        {
            var result = await _unit.CaKips.GetAllEmployeesInShiftAsync(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await _unit.CaKips.RemoveAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpsertCrew([FromBody] CaKipDTO caKip)
        {
            var result = await _unit.CaKips.UpsertCrewAsync(caKip);
            return Ok(result);
        }
        [HttpPatch]
        public async Task<IActionResult> ChangeStatusShift(int? id)
        {
            var result = await _unit.CaKips.ChangeStatusAsync(id);
            return Ok(result);
        }
    }
}
