using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Models;

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
        /* @statusChange Trạng thái thay đổi 
           @originHoadon Dữ liệu đơn hàng nguyên bản
           @reasonCancel Lý do hủy
        */
        public static void ValidateAndChangeStatus(Hoadon originHoadon, string statusChange, string? reasonCancel)
        {
            var validTransitions = new Dictionary<string, List<string>>
            {
                { TrangThaiDonHang.ChoThanhToan, new List<string> { TrangThaiDonHang.DaXacNhan, TrangThaiDonHang.DaHuy, TrangThaiDonHang.HoanTra_HoanTien } },
                { TrangThaiDonHang.DaXacNhan, new List<string> { TrangThaiDonHang.DaGiaoChoDonViVanChuyen, TrangThaiDonHang.DaHuy, TrangThaiDonHang.HoanTra_HoanTien } },
                { TrangThaiDonHang.DaGiaoChoDonViVanChuyen, new List<string> { TrangThaiDonHang.DangGiaoHang, TrangThaiDonHang.DaHuy, TrangThaiDonHang.HoanTra_HoanTien } },
                { TrangThaiDonHang.DangGiaoHang, new List<string> { TrangThaiDonHang.DaHuy, TrangThaiDonHang.HoanTra_HoanTien } }
            };

            if (validTransitions.TryGetValue(originHoadon.TinhTrang!, out var validStatuses) && validStatuses.Contains(statusChange))
            {
                if (statusChange == TrangThaiDonHang.DaHuy || statusChange == TrangThaiDonHang.HoanTra_HoanTien)
                {
                    ValidateCancellationTime(originHoadon);
                    originHoadon.LyDoHuy = reasonCancel ?? "Auto: Không có lý do hủy.";
                }
                originHoadon.TinhTrang = statusChange;
                if (statusChange == TrangThaiDonHang.DaGiaoChoDonViVanChuyen)
                {
                    originHoadon.BatDauGiao = DateTime.Now; // Record the shipping time
                }
            }
            else
            {
                throw new Exception($"Không thể chuyển từ '{originHoadon.TinhTrang}' sang trạng thái [{statusChange}].");
            }
        }

        public static void ValidateCancellationTime(Hoadon originHoadon)
        {
            DateTime timeNow = DateTime.Now;
            TimeSpan timeReceivedOrder = (originHoadon.NgayNhan!.Value - timeNow);
            if (timeReceivedOrder.Days > 0 && timeReceivedOrder.Hours > 1)
            {
                throw new Exception("Bạn không thể hủy đơn hàng đã quá 1h từ lần cuối bạn nhận đơn.");
            }
        }
    }
}