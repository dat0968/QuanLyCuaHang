using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repository.MaCoupon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly IMaCouponRepository maCouponRepository;
        public CouponController(IMaCouponRepository MaCouponRepository)
        {
            this.maCouponRepository = MaCouponRepository;
        }
        [HttpGet("GetAllCouponCodeByPage")]
        public IActionResult GetAllByPage(string? keywords, string? status, string? sort, int page = 1)
        {
            try
            {
                int pagesize = 5;
                var listCounponCode = maCouponRepository.GetAll(keywords, status, sort);
                var totalItems = listCounponCode.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
                var pagedCouponCode = listCounponCode.Skip((page - 1) * pagesize).Take(pagesize);
                return Ok(new
                {
                    Success = true,
                    Data = pagedCouponCode,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Page = page,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Message = $"Error {ex.Message}"
                });
            }
        }
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                var listCounponCode = maCouponRepository.GetAll(null, null, null);
                return Ok(new
                {
                    Success = true,
                    Data = listCounponCode,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Message = $"Error {ex.Message}"
                });
            }
        }
        [HttpPost("Create")]
        public IActionResult Create(CouponDTO model)
        {
            try
            {
                var newCouponCode = maCouponRepository.Create(model);
                return Ok(new
                {
                    Success = true,
                    Message = "Thêm mã coupon mới thành công"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Message = $"error {ex.Message}"
                });
            }
        }
        [HttpPut("Update")]
        public IActionResult Update(CouponDTO model)
        {
            try
            {
                maCouponRepository.Update(model);
                return Ok(new
                {
                    Success = true,
                    Message = "Sửa thông tin mã coupon thành công"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Message = $"error {ex.Message}"
                });
            }
        }
        [HttpPut("Cancel")]
        public IActionResult Cancel(string id)
        {
            try
            {
                maCouponRepository.Cancel(id);
                return Ok(new
                {
                    Success = true,
                    Message = "Hủy thông tin mã coupon thành công"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Message = $"error {ex.Message}"
                });
            }
        }
    }
}
