using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.ViewModels
{
    public class ResponseAPI<T> where T : new()
    {
        public int? Status { get; set; } = 500;
        public bool? Success { get; set; } = false;
        public string Message { get; set; } = "Phản hồi không xác định";
        public T? Data { get; set; } = new T();
        public void SetSuccessResponse()
        {
            this.Status = 200;
            this.Success = true;
        }
        public void SetData(T dataSet)
        {
            this.Data = dataSet;
        }
        public void SetMessageResponseWithException(int? Status = 500, Exception ex = null)
        {
            this.Status = Status;
            this.Success = false;
            this.Message = ex.Message;
        }
    }
}