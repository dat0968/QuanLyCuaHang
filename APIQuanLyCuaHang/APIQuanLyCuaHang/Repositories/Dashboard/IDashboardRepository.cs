using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.DTO.DataChart;

namespace APIQuanLyCuaHang.Repositories.Dashboard
{
    public interface IDashboardRepository
    {
        Task<ResponseAPI<EarningData>> GetEarningDataAsync(string timeRange);
        Task<ResponseAPI<OrderData>> GetAllOrderDataAsync();
        Task<ResponseAPI<OrderStatusData>> GetOrderStatusDataAsync(string timeRange);
        Task<ResponseAPI<OrderOverviewData>> GetOrderOverviewDataAsync(string timeRange);
        Task<ResponseAPI<TopSellingProductsData>> GetTopSellingProductsAsync();
        Task<ResponseAPI<List<ComboDC>>> GetTopSellingCombosAsync();
        Task<ResponseAPI<List<StaffDC>>> GetEmployeeOrderStatisticsAsync();
        Task<ResponseAPI<UserStatisticsData>> GetUserStatisticsAsync();
        Task<ResponseAPI<ProductDC>> GetProductFullDetails(int id);
        Task<ResponseAPI<WorkHistoryDC>> GetTopEmployeeRegistShift();
        Task<ResponseAPI<List<StatObjectDC>>> GetListStatObjectAsync();
    }
}