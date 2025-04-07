import DetailProductDC from './DetailProductDC' // Đảm bảo import lớp DetailProductDC nếu cần

export default class ProductDC {
  constructor(maSp, maDanhMuc, tenSanPham, moTa = null, chiTietSanPhams = []) {
    this.maSp = maSp
    this.maDanhMuc = maDanhMuc
    this.tenSanPham = tenSanPham
    this.moTa = moTa // Mô tả có thể là null
    this.chiTietSanPhams = chiTietSanPhams // Danh sách chi tiết sản phẩm, mặc định là mảng rỗng
  }
  static fromJson(json) {
    return new ProductDC(
      json.maSp,
      json.maDanhMuc,
      json.tenSanPham,
      json.moTa,
      json.chiTietSanPhams || [],
    )
  }

  toJson() {
    return {
      maSp: this.maSp,
      maDanhMuc: this.maDanhMuc,
      tenSanPham: this.tenSanPham,
      moTa: this.moTa,
      chiTietSanPhams: this.chiTietSanPhams,
    }
  }
}
