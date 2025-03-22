using System;

namespace APIQuanLyCuaHang.DTO
{
    public class ResponseAPI<T> where T : class
    {
        public int? Status { get; set; } = 500;
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "Phản hồi không xác định";
        public T? Data { get; set; }

        public ResponseAPI()
        {
            Data = null;
        }

        public void SetSuccessResponse(string? message)
        {
            this.Status = 200;
            this.Success = true;
            if (!string.IsNullOrEmpty(message))
            {
                this.Message = message;
            }
        }

        public void SetData(T dataSet)
        {
            this.Data = dataSet;
        }

        public void SetMessageResponseWithException(int? status = 500, Exception ex = null!)
        {
            this.Status = status;
            this.Success = false;
            this.Message = ex?.Message ?? "Lỗi không xác định";
        }
    }
}
