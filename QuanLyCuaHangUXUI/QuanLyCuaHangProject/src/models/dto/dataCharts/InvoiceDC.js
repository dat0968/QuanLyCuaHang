import DetailInvoiceDC from './DetailInvoiceDC' // Đảm bảo import lớp DetailInvoiceDC nếu cần

export default class InvoiceDC {
  constructor(
    maHd,
    maKh,
    maNv = null,
    ngayTao,
    batDauGiao = null,
    ngayNhan = null,
    diaChiNhanHang = null,
    ngayThanhToan = null,
    hinhThucTt = null,
    tinhTrang = null,
    moTa = null,
    hoTen,
    sdt,
    lyDoHuy = null,
    isDelete = null,
    phiVanChuyen,
    tienGoc,
    tongTien,
    sanPhamHoaDons = new DetailInvoiceDC(),
  ) {
    this.maHd = maHd
    this.maKh = maKh
    this.maNv = maNv // Mã nhân viên có thể là null
    this.ngayTao = ngayTao // Ngày tạo không thể là null
    this.batDauGiao = batDauGiao // Ngày bắt đầu giao có thể là null
    this.ngayNhan = ngayNhan // Ngày nhận có thể là null
    this.diaChiNhanHang = diaChiNhanHang // Địa chỉ nhận hàng có thể là null
    this.ngayThanhToan = ngayThanhToan // Ngày thanh toán có thể là null
    this.hinhThucTt = hinhThucTt // Hình thức thanh toán có thể là null
    this.tinhTrang = tinhTrang // Tình trạng có thể là null
    this.moTa = moTa // Mô tả có thể là null
    this.hoTen = hoTen // Họ tên không thể là null
    this.sdt = sdt // Số điện thoại không thể là null
    this.lyDoHuy = lyDoHuy // Lý do hủy có thể là null
    this.isDelete = isDelete // IsDelete có thể là null
    this.phiVanChuyen = phiVanChuyen // Phí vận chuyển không thể là null
    this.tienGoc = tienGoc // Tiền gốc không thể là null
    this.tongTien = tongTien // Tổng tiền không thể là null
    this.sanPhamHoaDons = sanPhamHoaDons // Danh sách sản phẩm hóa đơn có thể là null
  }
  static fromJson(json) {
    return new InvoiceDC(
      json.maHd,
      json.maKh,
      json.maNv,
      json.ngayTao,
      json.batDauGiao,
      json.ngayNhan,
      json.diaChiNhanHang,
      json.ngayThanhToan,
      json.hinhThucTt,
      json.tinhTrang,
      json.moTa,
      json.hoTen,
      json.sdt,
      json.lyDoHuy,
      json.isDelete,
      json.phiVanChuyen,
      json.tienGoc,
      json.tongTien,
      json.sanPhamHoaDons || [],
    )
  }

  toJson() {
    return {
      maHd: this.maHd,
      maKh: this.maKh,
      maNv: this.maNv,
      ngayTao: this.ngayTao,
      batDauGiao: this.batDauGiao,
      ngayNhan: this.ngayNhan,
      diaChiNhanHang: this.diaChiNhanHang,
      ngayThanhToan: this.ngayThanhToan,
      hinhThucTt: this.hinhThucTt,
      tinhTrang: this.tinhTrang,
      moTa: this.moTa,
      hoTen: this.hoTen,
      sdt: this.sdt,
      lyDoHuy: this.lyDoHuy,
      isDelete: this.isDelete,
      phiVanChuyen: this.phiVanChuyen,
      tienGoc: this.tienGoc,
      tongTien: this.tongTien,
      sanPhamHoaDons: this.sanPhamHoaDons,
    }
  }
}
