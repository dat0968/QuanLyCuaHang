using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Repositories.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dash;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dash = dashboardRepository;
        }

        /// <summary>
        /// Dữ liệu thống kê tổng tiền đơn hàng đã xác nhận theo thời gian
        /// </summary>
        /// <param name="timeRange">Nhập thời gian day/week/month/year</param>
        /// <returns></returns>
        [HttpGet("{timeRange}")]
        public async Task<IActionResult> GetEarningData(string timeRange)
        {
            var response = await _dash.GetEarningDataAsync(timeRange);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Dữ liệu thống kê theo tổng các đơn hàng chờ xác nhận và đã nhận
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllOrderData()
        {
            var response = await _dash.GetAllOrderDataAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Dữ liệu thống kê đơn hàng theo trạng thái
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IActionResult> GetOrderStatusData()
        {
            var response = await _dash.GetOrderStatusDataAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Dữ liệu thống kê đơn hàng theo thời gian, cung cấp cho dạng column + sparkline
        /// </summary>
        /// <param name="timeRange">Nhập thời gian day/week/month/year</param>
        /// <returns></returns>
        [HttpGet("{timeRange}")]
        public async Task<IActionResult> GetOrderOverViewData(string timeRange)
        {
            var response = await _dash.GetOrderOverviewDataAsync(timeRange);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> JustGetOneListSellingProducts()
        {
            var response = await _dash.GetTopSellingProductsAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Dữ liệu thống kê sản phẩm theo được chọn nhiều nhất,...
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTopSellingProducts()
        {
            var response = await _dash.GetTopSellingProductsAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Lấy thông tin chi tiết cho sản phẩm trong Datatable
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetailProduct(int id)
        {
            var response = await _dash.GetProductFullDetails(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        /// <summary>
        /// Dữ liệu thống kê danh sách nhân viên mang lại doanh thu nhiều nhất trong day/week/month/year
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetEmployeeOrderStats()
        {
            var response = await _dash.GetEmployeeOrderStatisticsAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Dữ liệu thống kê người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserStatisticsAsync()
        {
            var response = await _dash.GetUserStatisticsAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        /// <summary>
        /// Số liệu thống kê theo tình trạng người dùng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetListStatObject()
        {
            var response = await _dash.GetListStatObjectAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}