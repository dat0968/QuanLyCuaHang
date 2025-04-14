using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Services;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.VNPAY
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly OrderService orderService;
        private readonly QuanLyCuaHangContext db;
        public VnPayService(IConfiguration _configuration, OrderService orderService, QuanLyCuaHangContext db)
        {
            this._configuration = _configuration;
            this.orderService = orderService;
            this.db = db;
        }
        public async Task<string> CreatePaymentUrl(OrderRequestDTO model, HttpContext context)
        {
            var NewOrder = await orderService.AddOrder(model);
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["Vnpay:TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["Vnpay:ReturnUrl"];
            var GiamGiaCoupon = 0;
            if (!string.IsNullOrEmpty(model.MaCoupon))
            {
                var findCoupon = await db.MaCoupons.AsNoTracking().FirstOrDefaultAsync(p => p.MaCode == model.MaCoupon);
                if (findCoupon != null)
                {
                    GiamGiaCoupon = (int)(findCoupon.SoTienGiam.Value == null ? (findCoupon.PhanTramGiam.Value == null ? 0 : model.TienGoc - findCoupon.PhanTramGiam.Value) : 0);
                }
            }
            var total = model.TienGoc + model.PhiVanChuyen - GiamGiaCoupon;
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", (total * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.HoTen} {model.MoTa}");
            pay.AddRequestData("vnp_OrderType", "VNPAY");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", NewOrder.MaHd.ToString());
     
            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public async Task<PaymentResponseModel?> PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            try
            {
                if (response.Success)
                {
                    var FindOrder = await db.Hoadons.FindAsync(response.OrderId);
                    if (FindOrder == null)
                    {
                        throw new Exception("Hóa đơn không tồn tại");
                    }
                    FindOrder.TinhTrang = "Chờ xác nhận";
                    db.Hoadons.Update(FindOrder);
                    await db.SaveChangesAsync();
                    return response;
                }
                else
                {
                    throw new Exception("Lỗi");
                }
            }catch (Exception ex)
            {
                throw new Exception("Lỗi", ex);
            }
        }
    }
}
