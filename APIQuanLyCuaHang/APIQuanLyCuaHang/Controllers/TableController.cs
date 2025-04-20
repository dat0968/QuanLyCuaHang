using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Table;
using APIQuanLyCuaHang.Repositories.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableRepository _tableRepository;
        private readonly IProduct _productRepository;
        private readonly QuanLyCuaHangContext _db;

        public TableController(ITableRepository tableRepository, IProduct productRepository, QuanLyCuaHangContext db)
        {
            _tableRepository = tableRepository;
            _productRepository = productRepository;
            _db = db;
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

        [HttpGet("{id}/menu")]
        public async Task<IActionResult> GetMenuForTable(int id)
        {
            var table = await _tableRepository.GetTableById(id);
            if (table == null) return NotFound(new { message = $"Không tìm thấy bàn với ID {id}." });

            var products = await _productRepository.GetAll(null, null, null, null);
            var combos = await _db.Combos
                .Where(c => c.IsDelete == false)
                .Include(c => c.Chitietcombos)
                .ThenInclude(ct => ct.MaSpNavigation)
                .ThenInclude(sp => sp.Chitietsanphams)
                .Select(c => new ComboResponseDTO
                {
                    MaCombo = c.MaCombo,
                    TenCombo = c.TenCombo,
                    Hinh = c.Hinh,
                    SoTienGiam = c.SoTienGiam,
                    PhanTramGiam = c.PhanTramGiam,
                    SoLuong = c.SoLuong,
                    MoTa = c.MoTa,
                    IsDelete = c.IsDelete,
                    Chitietcombos = c.Chitietcombos
                        .Where(ct => ct.MaSpNavigation != null && ct.MaSpNavigation.Chitietsanphams.Any())
                        .Select(ct => new DetaisComboResponseDTO
                        {
                            MaSp = ct.MaSp,
                            TenSp = ct.MaSpNavigation.TenSanPham,
                            SoLuongSp = ct.SoLuongSp,
                            Chitietsanphams = ct.MaSpNavigation.Chitietsanphams
                                .Select(ctsp => new DetailProductResponseDTO
                                {
                                    MaCtsp = ctsp.MaCtsp,
                                    MaSp = ctsp.MaSp,
                                    KichThuoc = ctsp.KichThuoc,
                                    HuongVi = ctsp.HuongVi,
                                    SoLuongTon = ctsp.SoLuongTon,
                                    DonGia = ctsp.DonGia,
                                    AnhDaiDien = ctsp.Hinhanhs.OrderBy(img => img.MaHinhAnh).Select(img => img.TenHinhAnh).FirstOrDefault()
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .Where(c => c.Chitietcombos.Any()) // Chỉ trả về combo có chitietcombos hợp lệ
                .ToListAsync();

            return Ok(new
            {
                Table = table,
                Products = products,
                Combos = combos
            });
        }
    }
}