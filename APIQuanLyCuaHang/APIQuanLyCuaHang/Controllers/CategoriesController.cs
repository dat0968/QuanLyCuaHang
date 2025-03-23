using APIQuanLyCuaHang.Repositories.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategory CategoryRepository;

        public CategoriesController(ICategory CategoryRepository)
        {
            this.CategoryRepository = CategoryRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var List = await CategoryRepository.GetAll();
                return Ok(List);
            }catch (Exception ex)
            {
                throw;
            }
        }
    }
}
