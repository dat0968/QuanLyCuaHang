using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
        }

        [HttpPost]
       
        public async Task<IActionResult> AddToCart([FromBody] CartItemResquestDTO cartItem)
        {
            try
            {
                await cartRepository.AddToCart(cartItem);
                return Ok(new { Success = true, Message = "Thêm vào giỏ hàng thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message, Detail = ex.InnerException });
            }
        }

        [HttpGet("{maKh}")]
        
        public async Task<IActionResult> GetCart(int maKh)
        {
            try
            {
                var cartItems = await cartRepository.GetCart(maKh);
                var Total = cartItems.Sum(p => p.DonGia * p.SoLuong);
                var TotalQuantity = cartItems.Sum(p => p.SoLuong);
                return Ok(new { cartItems, TotalQuantity, Total});
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        
        public async Task<IActionResult> UpdateCartItem([FromRoute] int id, [FromBody] CartItemResquestDTO cartItem)
        {
            try
            {
                if (cartItem == null)
                {
                    return BadRequest(new { Success = false, Message = "Cart item cannot be null." });
                }
                
                await cartRepository.UpdateCartItem(id, cartItem);
                return Ok(new { Success = true, Message = "Cập nhật giỏ hàng thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("{id}/{userId}")]
        
        public async Task<IActionResult> RemoveCart(int id, int userId)
        {
            try
            {
                await cartRepository.RemoveCart(userId, id);
                return Ok(new { Success = true, Message = "Đã xóa sản phẩm ra khỏi giỏ hàng" });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.Message });
            }
        }

    }
}