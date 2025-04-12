namespace APIQuanLyCuaHang.DTO
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
    
    }
    public class PaymentResponseModel
    {
        public bool Success { get; set; } 
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }   
        public int OrderId { get; set; }    
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
        public string Amount { get; set; }

    } 
}
