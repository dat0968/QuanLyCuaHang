using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIQuanLyCuaHang.DTO
{
    public class HoaDonDTO
    {
        public int MaHd { get; set; }

        public int MaKh { get; set; }

        public int? MaNv { get; set; }

        public DateTime NgayTao { get; set; }

        public DateTime? BatDauGiao { get; set; }

        public DateTime? NgayNhan { get; set; }

        public string? DiaChiNhanHang { get; set; }

        public DateTime? NgayThanhToan { get; set; }

        public string? HinhThucTt { get; set; }

        public string? TinhTrang { get; set; }

        public string? MoTa { get; set; }

        public string HoTen { get; set; } = null!;
        public string HoTenNv { get; set; } = null!;

        public string Sdt { get; set; } = null!;

        public string? LyDoHuy { get; set; }

        public bool? IsDelete { get; set; }

        public decimal PhiVanChuyen { get; set; }

        public decimal TienGoc { get; set; }
        public decimal Tongtien { get; set; }
        public decimal GiamGiaCoupon { get; set; } = 0;
        public virtual List<ChitietcombohoadonDTO> ChitietcombohoadonDTOs { get; set; } = new List<ChitietcombohoadonDTO>();
    }
    public class ChitietcombohoadonDTO
    {
        public int MaHd { get; set; }

        public int MaCombo { get; set; }
        public string TenSpCombo { get; set; }
        public int MaCTSp { get; set; }
        public string? KichThuoc { get; set; }
        public string? HuongVi { get; set; }
        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

       
    }
}
