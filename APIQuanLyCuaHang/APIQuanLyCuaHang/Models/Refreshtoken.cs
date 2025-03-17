using System;
using System.Collections.Generic;

namespace APIQuanLyCuaHang.Models;

public partial class Refreshtoken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime IssuedAt { get; set; }

    public DateTime ExpiredAt { get; set; }
}
