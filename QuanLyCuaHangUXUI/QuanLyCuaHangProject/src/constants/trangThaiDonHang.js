// TrangThaiDonHang.js
export default class TrangThaiDonHang {
  // Định nghĩa các trạng thái đơn hàng
  static DangXuLy = 'Đang xử lý VNPAY'
  static DaXacNhan = 'Đã xác nhận'
  static DaGiaoChoDonViVanChuyen = 'Đã giao cho đơn vị vận chuyển'
  static DangGiaoHang = 'Đang giao hàng'
  static ChoThanhToan = 'Chờ thanh toán'
  static DaThanhToan = 'Đã thanh toán'
  static HoanTra_HoanTien = 'Hoàn trả/Hoàn tiền'
  static DaHuy = 'Đã hủy'
  static ChoXacNhan = 'Chờ xác nhận'
  static DaNhan = 'Đã Nhận'

  // Tập hợp tất cả các trạng thái đơn hàng trong mảng
  static TatCaTrangThai = [
    TrangThaiDonHang.DaXacNhan,
    TrangThaiDonHang.DaThanhToan,
    TrangThaiDonHang.DangXuLy,
    TrangThaiDonHang.DaNhan,
    TrangThaiDonHang.DaGiaoChoDonViVanChuyen,
    TrangThaiDonHang.DangGiaoHang,
    TrangThaiDonHang.ChoThanhToan,
    TrangThaiDonHang.HoanTra_HoanTien,
    TrangThaiDonHang.DaHuy,
    TrangThaiDonHang.ChoXacNhan,
  ]

  // Tập hợp các trạng thái không bình thường
  static TrangThaiKhongBinhThuong = [TrangThaiDonHang.HoanTra_HoanTien, TrangThaiDonHang.DaHuy]

  // Tập hợp các trạng thái có thể thay đổi
  static TrangThaiCoTheDoi = [
    TrangThaiDonHang.ChoThanhToan,
    TrangThaiDonHang.DaXacNhan,
    TrangThaiDonHang.DaGiaoChoDonViVanChuyen,
    TrangThaiDonHang.DangGiaoHang,
  ]

  // Các trạng thái có thể hủy dựa trên trạng thái hiện tại
  static TrangThaiCoTheHuy = {
    [TrangThaiDonHang.ChoThanhToan]: [TrangThaiDonHang.DaHuy],
    [TrangThaiDonHang.DaXacNhan]: [TrangThaiDonHang.HoanTra_HoanTien, TrangThaiDonHang.DaHuy],
    [TrangThaiDonHang.DaGiaoChoDonViVanChuyen]: [
      TrangThaiDonHang.DangGiaoHang,
      TrangThaiDonHang.HoanTra_HoanTien,
      TrangThaiDonHang.DaHuy,
    ],
    [TrangThaiDonHang.DangGiaoHang]: [TrangThaiDonHang.HoanTra_HoanTien, TrangThaiDonHang.DaHuy],
  }

  // Hàm kiểm tra xem trạng thái có thể hủy không
  static isCancelable(currentStatus, newStatus) {
    if (!TrangThaiDonHang.TrangThaiCoTheHuy[currentStatus]) {
      return []
    }
    return TrangThaiDonHang.TrangThaiCoTheHuy[currentStatus]
  }
}
