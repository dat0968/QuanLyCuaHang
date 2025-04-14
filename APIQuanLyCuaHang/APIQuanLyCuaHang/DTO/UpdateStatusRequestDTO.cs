namespace APIQuanLyCuaHang.DTO
{
    public class UpdateStatusRequestDTO
    {
        public int OrderId { get; set; }
        public string? StatusChange { get; set; }
        public string? ReasonCancel { get; set; }
    }
}
