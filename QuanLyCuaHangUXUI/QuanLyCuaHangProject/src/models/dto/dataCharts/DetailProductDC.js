class DetailProductDC {
  constructor(
    maCtsp,
    maSp,
    kichThuoc = null,
    huongVi = null,
    soLuongTon = null,
    donGia = null,
    tenHinhAnh = null,
  ) {
    this.maCtsp = maCtsp
    this.maSp = maSp
    this.kichThuoc = kichThuoc
    this.huongVi = huongVi
    this.soLuongTon = soLuongTon
    this.donGia = donGia
    this.tenHinhAnh = tenHinhAnh
  }
}

export default DetailProductDC
