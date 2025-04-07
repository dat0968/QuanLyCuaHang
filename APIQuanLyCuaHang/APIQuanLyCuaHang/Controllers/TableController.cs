using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Table;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableRepository _tableRepository;

        public TableController(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTables()
        {
            var tables = await _tableRepository.GetAllTables();
            return Ok(tables);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTableById(int id)
        {
            var table = await _tableRepository.GetTableById(id);
            if (table == null) return NotFound();
            return Ok(table);
        }

        [HttpPost]
        public async Task<IActionResult> AddTable([FromBody] BanDTO banDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Chuyển BanDTO thành Ban để thêm vào DB
            var ban = new Ban
            {
                TinhTrang = string.IsNullOrEmpty(banDTO.TinhTrang) ? "Trống" : banDTO.TinhTrang
            };

            var result = await _tableRepository.AddTable(ban);
            return CreatedAtAction(nameof(GetTableById), new { id = result.Id }, new BanDTO
            {
                Id = result.Id,
                TinhTrang = result.TinhTrang
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTableStatus(int id, [FromBody] BanDTO banDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingTable = await _tableRepository.GetTableById(id);
            if (existingTable == null) return NotFound();

            var updatedBanDTO = new BanDTO
            {
                Id = id,
                TinhTrang = banDTO.TinhTrang
            };

            var result = await _tableRepository.UpdateTable(id, updatedBanDTO);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var result = await _tableRepository.DeleteTable(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterTablesByStatus([FromQuery] string tinhTrang, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var validStatuses = new[] { "Trống", "Đang sử dụng", "Đã đặt trước", "Đang sửa chữa" };
            if (!string.IsNullOrEmpty(tinhTrang) && !validStatuses.Contains(tinhTrang))
                return BadRequest("Tình trạng không hợp lệ");

            var (tables, totalItems) = await _tableRepository.FilterTablesByStatus(tinhTrang, page, pageSize);
            return Ok(new { Items = tables, TotalItems = totalItems });
        }
    }
}