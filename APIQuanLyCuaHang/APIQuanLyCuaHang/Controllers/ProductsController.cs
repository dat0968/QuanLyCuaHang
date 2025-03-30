using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.DetailProduct;
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
        private readonly IDetailProduct detailproduct;
        private readonly ProductService productService;

        public ProductsController(IProduct product, ProductService productService, IDetailProduct detailproduct)
        {
            this.product = product;
            this.detailproduct = detailproduct;
            this.productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllByPage([FromQuery] string? search, int? filterCatories, string? sort, string? filterPrices, int page = 1)
        {
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
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var GetListProductVM = await product.GetAll(null, null, null, null);
            return Ok(GetListProductVM);
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
                    Message = "Thêm sản phẩm mới thành công",
                    
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = $"{ex.Message}", Detail = ex.StackTrace, });
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
                return BadRequest(new { Success = false, Message = $"{ex.Message}", Details = ex.InnerException?.Message, StackTrace = ex.StackTrace });
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
        [HttpGet("CheckExistDetailProduct")]
        public async Task<IActionResult> CheckExistDetailProduct(int id, string? KichThuoc, string? HuongVi)
        {
            try
            {
                var findDetailProduct = await detailproduct.CheckExistDetailProduct(id, KichThuoc, HuongVi);
                if(findDetailProduct != null)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Biến thể sản phẩm này đã tồn tại"
                    });
                }
                return Ok(new
                {
                    Success = false,
                    Message = "Biến thể sản phẩm này chưa tồn tại"
                });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
