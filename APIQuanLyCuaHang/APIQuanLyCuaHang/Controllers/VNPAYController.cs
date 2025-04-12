using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Repositories.DetailMaCoupon;
using APIQuanLyCuaHang.Repositories.VNPAY;
using APIQuanLyCuaHang.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VNPAYController : ControllerBase
    {       
        private readonly IVnPayService _vnPayService;
        public VNPAYController(IVnPayService _vnPayService)
        {
            this._vnPayService = _vnPayService;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl(OrderRequestDTO model)
        {
            var url = await _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Ok(url);
        }
        [HttpGet]
        public async Task<IActionResult> PaymentCallback()
        {
            var response = await _vnPayService.PaymentExecute(Request.Query);
            if(response.Success == true)
            {
                return Redirect($"http://localhost:5173/VNPAYresponse/{response.OrderId}/{response.Amount}");
                //return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
