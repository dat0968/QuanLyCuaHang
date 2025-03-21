export default class StaffDC {
  constructor(
    maNv,
    hoTen,
    gioiTinh,
    ngaySinh = null,
    diaChi = null,
    cccd = null,
    sdt,
    email,
    ngayVaoLam,
    tenTaiKhoan = null,
    matKhau,
    tinhTrang = null,
    isDelete = null,
    maChucVu = null,
    soDonHangDamNhan = null,
    doanhThuMangLai = null,
  ) {
    this.maNv = maNv
    this.hoTen = hoTen
    this.gioiTinh = gioiTinh
    this.ngaySinh = ngaySinh // Ngày sinh có thể là null
    this.diaChi = diaChi // Địa chỉ có thể là null
    this.cccd = cccd // CCCD có thể là null
    this.sdt = sdt
    this.email = email
    this.ngayVaoLam = ngayVaoLam // Ngày vào làm không thể là null
    this.tenTaiKhoan = tenTaiKhoan // Tên tài khoản có thể là null
    this.matKhau = matKhau
    this.tinhTrang = tinhTrang // Tình trạng có thể là null
    this.isDelete = isDelete // IsDelete có thể là null
    this.maChucVu = maChucVu // Mã chức vụ có thể là null
    this.soDonHangDamNhan = soDonHangDamNhan // Số đơn hàng đã nhận có thể là null
    this.doanhThuMangLai = doanhThuMangLai // Doanh thu mang lại có thể là null
  }
}
