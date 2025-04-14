using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Models;

namespace APIQuanLyCuaHang.Constants
{
    public static class TrangThaiDonHang
    {
        public const string DangXuLy = "Đang xử lý VNPAY";
        public const string DaXacNhan = "Đã xác nhận";
        public const string DaGiaoChoDonViVanChuyen = "Đã giao cho đơn vị vận chuyển";
        public const string DangGiaoHang = "Đang giao hàng";
        public const string ChoThanhToan = "Chờ thanh toán";
        public const string DaThanhToan = "Đã thanh toán";
        public const string HoanTra_HoanTien = "Hoàn trả/Hoàn tiền";
        public const string DaHuy = "Đã hủy";
        public const string ChoXacNhan = "Chờ xác nhận";
        public const string DaNhan = "Đã Nhận";

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
        public static readonly List<string> TrangThaiXuLy = new List<string>
        {
            DangXuLy,
            ChoXacNhan,
            DaXacNhan,
            DaGiaoChoDonViVanChuyen,
            DangGiaoHang,
            ChoThanhToan,
            DaThanhToan
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
        public static bool ValidateAndCancelOrderForCustomer(Hoadon originHoadon, string statusChange, string? reasonCancel)
        {
            bool isReturn = false;

            // Kiểm tra trạng thái muốn đổi có nằm trong danh sách trạng thái không bình thường hay không
            bool isCanChange = TrangThaiKhongBinhThuong.Select(x=>x.ToLower()).Contains(statusChange.ToLower());
            if (isCanChange)
            {
                switch (originHoadon.TinhTrang)
                {
                    case DangXuLy:
                    case ChoThanhToan:
                        {
                            // Chắc sẽ viết gì đó ở đây
                            break;
                        }
                    case DaThanhToan:
                    case ChoXacNhan:
                    case DaXacNhan:
                    case DaGiaoChoDonViVanChuyen:
                    case DangGiaoHang:
                        {
                            isReturn = true;
                            break;
                        }
                    case DaNhan:
                        {
                            ValidateCancellationTime(originHoadon);
                            isReturn = true;
                            break;
                        }
                }
                originHoadon.TinhTrang = statusChange;
                originHoadon.LyDoHuy = reasonCancel ?? "Auto: Không có lý do hủy.";
            }
            else
            {
                throw new Exception($"Không thể chuyển từ '{originHoadon.TinhTrang}' sang trạng thái [{statusChange}].");
            }
            return isReturn;
        }

        public static void ValidateCancellationTime(Hoadon originHoadon)
        {
            DateTime timeNow = DateTime.Now;
            if (originHoadon.NgayNhan == null)
            {
                throw new Exception("Dữ liệu đơn hàng không hợp lệ, vui lòng liên hệ nhân viên để được hỗ trợ.");
            }
            TimeSpan timeReceivedOrder = originHoadon.NgayNhan.Value - timeNow;
            if (timeReceivedOrder.Days > 0 && timeReceivedOrder.Hours > 1)
            {
                throw new Exception("Bạn không thể hủy đơn hàng đã quá 1h từ lần cuối bạn nhận đơn.");
            }
        }
    }
}