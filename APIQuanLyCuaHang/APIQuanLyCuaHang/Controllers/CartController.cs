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
    public class CartController : ControllerBase
    {
        private readonly ICartRepository cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
        }

        [HttpPost]
        //[Authorize]
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
        //[Authorize] 
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
        //[Authorize]
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

        [HttpDelete("remove/{maKh}/{maCtsp}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int maKh, int maCtsp)
        {
            try
            {
                await cartRepository.RemoveFromCart(maKh, maCtsp);
                return Ok(new { Success = true, Message = "Xóa sản phẩm khỏi giỏ hàng thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("remove-combo/{maKh}/{maCombo}")]
        [Authorize]
        public async Task<IActionResult> RemoveComboFromCart(int maKh, int maCombo)
        {
            try
            {
                await cartRepository.RemoveComboFromCart(maKh, maCombo);
                return Ok(new { Success = true, Message = "Xóa combo khỏi giỏ hàng thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}