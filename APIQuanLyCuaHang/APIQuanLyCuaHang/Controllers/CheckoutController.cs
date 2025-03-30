using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly OrderService orderService;
        public CheckoutController(OrderService orderService)
        {
            this.orderService = orderService;
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(OrderRequestDTO NewOrder)
        {
            try
            {
                if(NewOrder.HinhThucTt.ToLower() == "cod"){
                    await orderService.AddOrder(NewOrder);
                }
                return Ok(new
                {
                    Success = true,
                    Message = "Thanh toán thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.InnerException,
                }); 
            }
        }
    }
}
