export default class TrangThaiLichLamViec {
  // Định nghĩa các trạng thái lịch làm việc
  static ChoXacNhan = 'Chờ xác nhận'
  static DiLam = 'Đi làm'
  static KetThucCa = 'Kết thúc ca'
  static NghiPhep = 'Nghỉ phép'
  static Tre = 'Trễ'
  static NghiKhongPhep = 'Nghỉ không phép'
  static KhongDuocXacNhan = 'Không được xác nhận'
  static TatCaTrangThai = [
    TrangThaiLichLamViec.ChoXacNhan,
    TrangThaiLichLamViec.DiLam,
    TrangThaiLichLamViec.KetThucCa,
    TrangThaiLichLamViec.NghiPhep,
    TrangThaiLichLamViec.Tre,
    TrangThaiLichLamViec.NghiKhongPhep,
    TrangThaiLichLamViec.KhongDuocXacNhan,
  ]
  static TrangThaiKhongBinhThuong = [
    TrangThaiLichLamViec.NghiKhongPhep,
    TrangThaiLichLamViec.KhongDuocXacNhan,
  ]
  static TrangThaiCoTheDoi = [
    TrangThaiLichLamViec.ChoXacNhan,
    TrangThaiLichLamViec.DiLam,
    TrangThaiLichLamViec.KetThucCa,
  ]
}
