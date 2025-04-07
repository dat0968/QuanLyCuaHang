class DetailInvoiceDC {
  constructor(
    maHd,
    maCtsp,
    soLuong,
    maSp,
    maDanhMuc,
    tenSanPham,
    moTa = null,
    kichThuoc = null,
    huongVi = null,
    soLuongTon = null,
    donGia = null,
    tongTien = null,
    linkAnhDau = null,
  ) {
    this.maHd = maHd
    this.maCtsp = maCtsp
    this.soLuong = soLuong
    this.maSp = maSp
    this.maDanhMuc = maDanhMuc
    this.tenSanPham = tenSanPham
    this.moTa = moTa
    this.kichThuoc = kichThuoc
    this.huongVi = huongVi
    this.soLuongTon = soLuongTon
    this.donGia = donGia
    this.tongTien = tongTien
    this.linkAnhDau = linkAnhDau
  }
}

export default DetailInvoiceDC
