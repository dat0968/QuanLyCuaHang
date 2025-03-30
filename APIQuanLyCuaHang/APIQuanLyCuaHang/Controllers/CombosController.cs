using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Combo;
using APIQuanLyCuaHang.Repositories.Product;
using APIQuanLyCuaHang.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CombosController : ControllerBase
    {
        private readonly IComboRepository comboRepository;
        private readonly ComboService comboService;

        public CombosController(IComboRepository comboRepository, ComboService comboService)
        {
            this.comboRepository = comboRepository;
            this.comboService = comboService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCombos(string? search, int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pagesize = 10;
            var GetList = await comboRepository.GetAll(search);
            var GetListByPaged = GetList.Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var totalItems = GetList.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            return Ok(new
            {
                Data = GetListByPaged,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var GetProductVM = await comboRepository.GetById(id);
            return Ok(GetProductVM);
        }
        [HttpPost]
        public async Task<IActionResult> AddCombo([FromForm] ComboRequestDTO combo)
        {
            try
            {
                await comboService.AddCombo(combo);
                return Ok(new
                {
                    Success = true,
                    Message = "Thêm sản phẩm thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCombo([FromRoute]int id, [FromForm] ComboRequestDTO combo)
        {
            try
            {
                await comboService.EditCombo(id, combo);
                return Ok(new
                {
                    Success = true,
                    Message = "Cập nhật thông tin sản phẩm thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}/Cancel")]
        public async Task<IActionResult> CancelCombo([FromRoute] int id)
        {
            try
            {
                await comboRepository.CancelCombo(id);
                return Ok(new
                {
                    Message = "Xóa thông tin sản phẩm thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
