using APIQuanLyCuaHang.Repositories.Role;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRole roleRepositoty;
        public RolesController(IRole roleRepositoty)
        {
            this.roleRepositoty = roleRepositoty;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await roleRepositoty.GetAll();
                return Ok(new
                {
                    Success = true,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                });
            }
        }
    }
}
