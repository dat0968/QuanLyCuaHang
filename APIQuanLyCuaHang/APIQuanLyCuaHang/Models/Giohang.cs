using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIQuanLyCuaHang.Models;

public partial class Giohang
{

    public int Id { get; set; }

    public int MaKh { get; set; }

    public int? MaCtsp { get; set; }
    [Column("MaComboNavigation")]
    public int? MaCombo { get; set; }


    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public virtual Chitietsanpham? MaCtspNavigation { get; set; }

    public virtual Khachhang MaKhNavigation { get; set; } = null!;
    public virtual Combo? MaComboNavigation { get; set; }
    public virtual ICollection<GioHangCTCombo> GioHangCTCombos { get; set; } = new List<GioHangCTCombo>();
}