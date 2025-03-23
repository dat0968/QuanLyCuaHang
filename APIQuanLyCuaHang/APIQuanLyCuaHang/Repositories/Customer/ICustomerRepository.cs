using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Repositories.Customer
{
    public interface ICustomerRepository
    {
        Task<(int TotalRecords, List<KhachhangDTO> Data)> GetCustomersAsync(int pageNumber, int pageSize, string? searchTerm, string? sortBy);
        Task<Khachhang> AddCustomerAsync(Khachhang customer);
        Task<Khachhang> UpdateCustomerAsync(int id, Khachhang customer);
        Task<bool> HideCustomerAsync(int id);
        Task<byte[]> ExportCustomersToExcelAsync(string? searchTerm, string? sortBy);
        Task<(int SuccessCount, int ErrorCount, List<string> Errors)> ImportCustomersFromExcelAsync(Stream excelStream); // Thêm phương thức import
        Task<Khachhang> GetCustomerByIdAsync(int id);
    }
}