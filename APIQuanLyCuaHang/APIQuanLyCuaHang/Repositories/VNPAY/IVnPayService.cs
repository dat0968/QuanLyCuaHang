using APIQuanLyCuaHang.DTO;

namespace APIQuanLyCuaHang.Repositories.VNPAY
{
    public interface IVnPayService
    {
        Task<string> CreatePaymentUrl(OrderRequestDTO model, HttpContext context);
        Task<PaymentResponseModel?> PaymentExecute(IQueryCollection collections);
    }
}
