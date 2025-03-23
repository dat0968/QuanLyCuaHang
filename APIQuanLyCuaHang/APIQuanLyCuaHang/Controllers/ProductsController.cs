using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Product;
using APIQuanLyCuaHang.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProduct product;
        private readonly ProductService productService;

        public ProductsController(IProduct product, ProductService productService)
        {
            this.product = product;
            this.productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, int? filterCatories, string? sort, string? filterPrices, int page = 1)
        {
            Console.WriteLine($"Search Parameter: {search}");
            var GetListProductVM = await product.GetAll(search, filterCatories, sort, filterPrices);
            page = page >= 1 ? page : 1;
            int pagesize = 10;
            // Phân trang
            var pagedProduct = GetListProductVM.Skip((page - 1) * pagesize).Take(pagesize).ToList();
            // Tổng số trang
            var totalItems = GetListProductVM.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            return Ok(new
            {
                Data = pagedProduct,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var GetProductVM = await product.GetById(id);
            return Ok(GetProductVM);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductCreateRequestDTO ProductRequestDTO)
        {
            try
            {
                await productService.AddProduct(ProductRequestDTO);
                return Ok(new
                {
                    Success = true,
                    Message = "Thêm sản phẩm mới thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = $"{ex.Message}" });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] ProductEditRequestDTO ProductRequestDTO)
        {
            try
            {
                await productService.EditProduct(id, ProductRequestDTO);
                return Ok(new
                {
                    Success = true,
                    Message = "Cập nhật thông tin sản phẩm thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = $"{ex.Message}" });
            }
        }
        [HttpPut("{id}/Cancel")]
        public async Task<IActionResult> Cancel([FromRoute] int id)
        {
            try
            {
                await product.CancelProduct(id);
                return Ok(new
                {
                    Success = true,
                    Message = "Xóa sản phẩm thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = $"{ex.Message}" });
            }
        }
    }
}
