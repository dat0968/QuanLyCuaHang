using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Helpers.Constants
{
    public class HttpStatusCodes
    {
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int NotFound = 404;
        public const int Conflict = 409;
        public const int InternalServerError = 500;
    }
}