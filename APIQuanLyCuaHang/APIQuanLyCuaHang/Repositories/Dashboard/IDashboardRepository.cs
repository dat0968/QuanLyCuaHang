using APIQuanLyCuaHang.ViewModels;

namespace APIQuanLyCuaHang.Repositories.Dashboard
{
    public interface IDashboardRepository
    {
        Task<ResponseAPI<dynamic?>> GetAllInvoiceStatisticsAsync();
        Task<ResponseAPI<dynamic?>> GetAllInvoiceDetailStatisticsAsync();
        Task<ResponseAPI<dynamic?>> GetEmployeeOrderStatisticsAsync();
        Task<ResponseAPI<dynamic?>> GetAllInvoiceDataStatisticsAsync();
        Task<ResponseAPI<dynamic?>> GetUserStatisticsAsync();
    }
}