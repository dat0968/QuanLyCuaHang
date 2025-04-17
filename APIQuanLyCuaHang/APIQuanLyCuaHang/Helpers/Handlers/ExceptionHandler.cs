using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Helpers.Constants;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Helpers.Handlers
{
    public static class ExceptionHandler
    {
        public static void HandleException<T>(Exception ex, ResponseAPI<T> response) where T : class
        {
            switch (ex)
            {
                case ArgumentNullException _:
                case ArgumentOutOfRangeException _:
                    response.SetMessageResponseWithException(HttpStatusCodes.BadRequest, ex);
                    break;
                case UnauthorizedAccessException _:
                    response.SetMessageResponseWithException(HttpStatusCodes.Unauthorized, ex);
                    break;
                case KeyNotFoundException _: // Giả sử bạn có một ngoại lệ NotFoundException
                    response.SetMessageResponseWithException(HttpStatusCodes.NotFound, ex);
                    break;
                case InvalidOperationException _:
                    response.SetMessageResponseWithException(HttpStatusCodes.Conflict, ex);
                    break;
                case DbUpdateConcurrencyException _:
                    response.SetMessageResponseWithException(HttpStatusCodes.Conflict, ex);
                    break;
                case DbUpdateException _:
                    response.SetMessageResponseWithException(HttpStatusCodes.InternalServerError, ex);
                    break;
                default:
                    response.SetMessageResponseWithException(HttpStatusCodes.InternalServerError, ex);
                    break;

            }
        }
    }
}