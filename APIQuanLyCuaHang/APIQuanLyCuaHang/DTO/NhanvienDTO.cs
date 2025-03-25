using System;
namespace APIQuanLyCuaHang.DTO
{
    public class NhanvienDTO
    {
        public int MaNv { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string Cccd { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public DateOnly NgayVaoLam { get; set; }
        public string TenTaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public string TinhTrang { get; set; }
        public bool? IsDelete { get; set; }
        public int? MaChucVu { get; set; }
        public IFormFile HinhAnh { get; set; } // Dùng để upload file, không bắt buộc

        public string HinhAnhDuongDan { get; set; }
    }
}
