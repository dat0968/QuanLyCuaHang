using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Constants
{
    public static class TrangThaiDonHang
    {
        public const string DaXacNhan = "Đã xác nhận";
        public const string DaGiaoChoDonViVanChuyen = "Đã giao cho đơn vị vận chuyển";
        public const string DangGiaoHang = "Đang giao hàng";
        public const string ChoThanhToan = "Chờ thanh toán";
        public const string HoanTra_HoanTien = "Hoàn trả/Hoàn tiền";
        public const string DaHuy = "Đã hủy";
        public const string ChoXacNhan = "Chờ xác nhận";

        public static readonly string ValidateRegex = String.Join("|", (new[]
        {
            DaXacNhan,
            DaGiaoChoDonViVanChuyen,
            DangGiaoHang,
            ChoThanhToan,
            HoanTra_HoanTien,
            DaHuy,
            ChoXacNhan
        }));

        public static readonly List<string> TatCaTrangThai = new List<string>
        {
            DaXacNhan,
            DaGiaoChoDonViVanChuyen,
            DangGiaoHang,
            ChoThanhToan,
            HoanTra_HoanTien,
            DaHuy,
            ChoXacNhan
        };

        public static readonly List<string> TrangThaiKhongBinhThuong = new List<string>
        {
            HoanTra_HoanTien,
            DaHuy
        };
    }
}