// TrangThaiDonHang.js
export default class TrangThaiDonHang {
  // Định nghĩa các trạng thái đơn hàng
  static DaXacNhan = 'Đã xác nhận'
  static DaGiaoChoDonViVanChuyen = 'Đã giao cho đơn vị vận chuyển'
  static DangGiaoHang = 'Đang giao hàng'
  static ChoThanhToan = 'Chờ thanh toán'
  static HoanTra_HoanTien = 'Hoàn trả/Hoàn tiền'
  static DaHuy = 'Đã hủy'
  static ChoXacNhan = 'Chờ xác nhận'

  // Tập hợp tất cả các trạng thái đơn hàng trong mảng
  static TatCaTrangThai = [
    TrangThaiDonHang.DaXacNhan,
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
}
