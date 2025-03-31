using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace APIQuanLyCuaHang.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly QuanLyCuaHangContext _db;

        public DbInitializer(QuanLyCuaHangContext db)
        {
            _db = db;
        }
        public void Initializer()
        {
            //Mã dùng kiểm tra Migrations chưa có trong CSDL hoặc chưa thấy CSDL

            if (_db.Database.GetPendingMigrations().Any())
            {
                _db.Database.Migrate();
            }
            if (!_db.Khachhangs.Any(kh => kh.Email == "admin@default.com"))
            {
                CreateTestAccount();
            }
        }
        private void CreateTestAccount()
        {

            // Tạo tài khoản mặc định
            var defaultUser = new Khachhang
            {
                HoTen = "Admin Default",
                TenTaiKhoan = "admin",
                Email = "admin@default.com",
                MatKhau = BCrypt.Net.BCrypt.HashPassword("Admin@123"), // Mã hóa mật khẩu
                NgayTao = DateTime.Now,
                IsDelete = false,
                TinhTrang = "Đang hoạt động",
            };

            // Lưu vào cơ sở dữ liệu
            _db.Khachhangs.Add(defaultUser);
            _db.SaveChanges();
        }
        private void CreateOrderForCharts()
        {
            try
            {
                // Lấy danh sách khách hàng và nhân viên
                var maKhachHangs = _db.Khachhangs.Select(x => new { x.MaKh, x.HoTen, x.Sdt }).ToList();
                var maNhanViens = _db.Nhanviens.Select(x => x.MaNv).ToList();

                // Lấy danh sách sản phẩm
                var sanPhams = _db.Chitietsanphams.ToList();

                // Random generator
                Random rand = new Random();

                // Danh sách tình trạng đơn hàng
                var tinhTrangs = new List<string>
                {
                    "Chờ xác nhận",
                    "Đã xác nhận",
                    "Đã giao cho đơn vị vận chuyển",
                    "Đang giao hàng",
                    "Chờ thanh toán",
                    "Đã thanh toán",
                    "Hoàn trả/Hoàn tiền",
                    "Đã hủy"
                };

                // Danh sách lưu hóa đơn và chi tiết hóa đơn để thêm vào DbContext
                var hoaDons = new List<Hoadon>();
                var Cthoadons = new List<Cthoadon>();

                for (int i = 0; i < 500; i++) // Tạo 500 hóa đơn
                {
                    // Lấy ngẫu nhiên khách hàng và nhân viên
                    var khachHang = maKhachHangs[rand.Next(maKhachHangs.Count)];
                    int? maNhanVien = maNhanViens.Count > 0 ? maNhanViens[rand.Next(maNhanViens.Count)] : (int?)null;

                    // Lấy ngẫu nhiên tình trạng đơn hàng
                    string tinhTrang = tinhTrangs[rand.Next(tinhTrangs.Count)];
                    // Phân bổ khoảng thời gian theo tỷ lệ
                    var randomRange = rand.Next(1, 101); // Tạo số ngẫu nhiên từ 1 đến 100 để xác định phân bổ thời gian
                    DateTime ngayTao;

                    // Phân phối tỷ lệ:
                    if (randomRange <= 10) // 10% số đơn (trong ngày)
                    {
                        ngayTao = DateTime.Now; // Lấy ngày hiện tại
                    }
                    else if (randomRange <= 30) // 20% số đơn (trong tuần)
                    {
                        ngayTao = DateTime.Now.AddDays(-rand.Next(0, 7)); // Ngẫu nhiên trong vòng 1 tuần qua
                    }
                    else if (randomRange <= 60) // 30% số đơn (trong tháng)
                    {
                        ngayTao = DateTime.Now.AddDays(-rand.Next(0, 30)); // Ngẫu nhiên trong vòng 1 tháng qua
                    }
                    else // 40% số đơn (trong năm)
                    {
                        ngayTao = DateTime.Now.AddDays(-rand.Next(0, 365 * 4)); // Ngẫu nhiên trong vòng 1 năm qua
                    }

                    // Tạo thời gian giao hàng (cộng thêm 3 ngày so với ngày tạo)
                    DateTime thoiGianTao = ngayTao.AddDays(3);
                    DateTime ngayNhan = thoiGianTao.AddDays(7);

                    // Tạo hóa đơn (lưu ý không gán ID thủ công - ID sẽ được auto-generate)
                    var hoaDon = new Hoadon
                    {
                        MaKh = khachHang.MaKh,
                        MaNv = maNhanVien,
                        DiaChiNhanHang = $"Địa chỉ {i + 1}",
                        NgayTao = ngayTao,
                        BatDauGiao = thoiGianTao,
                        NgayNhan = ngayNhan,
                        HinhThucTt = rand.Next(0, 2) == 0 ? "COD" : "VNPAY", // Thanh Toán
                        TinhTrang = tinhTrang,
                        HoTen = khachHang.HoTen,
                        Sdt = khachHang.Sdt ?? "N/A",
                        MoTa = "Tạo tự động hóa đơn cho mục đích test",
                        //GiamGiaMaCoupon = rand.Next(2, 6) * 4000,
                        PhiVanChuyen = rand.NextDouble() > 0.5 ? 40000 : 50000
                    };

                    if (tinhTrang == "Đã hủy" || tinhTrang == "Hoàn trả/Hoàn tiền")
                    {
                        hoaDon.LyDoHuy = "Đơn hàng bị hủy vì được lựa chọn.";
                    }

                    // Thêm hóa đơn vào danh sách (chưa lưu vào DbContext)
                    hoaDons.Add(hoaDon);
                }

                // Lưu toàn bộ danh sách hóa đơn vào DbContext trước để ID được generate
                _db.Hoadons.AddRange(hoaDons);

                // Lưu tất cả thay đổi
                _db.SaveChanges();

                foreach (var hoaDon in hoaDons)
                {
                    // Tạo ngẫu nhiên số sản phẩm trong giỏ hàng (1-5 sản phẩm)
                    int soLuongSanPham = rand.Next(1, 5);
                    var chiTietSanPhamOfHoaDon = new List<Cthoadon>();

                    // Đảm bảo danh sách sản phẩm không bị trùng lặp trong một hóa đơn
                    var sanPhamsDuocChon = new HashSet<(int MaSp, int? MaMau)>();

                    for (int j = 0; j < soLuongSanPham; j++)
                    {
                        var sanPham = sanPhams[rand.Next(sanPhams.Count)];
                        var key = (sanPham.MaSp, sanPham.MaCtsp);

                        // Kiểm tra sản phẩm đã được thêm vào hóa đơn hay chưa
                        if (sanPhamsDuocChon.Contains(key))
                        {
                            j--; // Thử chọn sản phẩm khác
                            continue;
                        }

                        // Kiểm tra số lượng tồn kho của sản phẩm
                        if (sanPham.SoLuongTon > 0)
                        {
                            int soLuongMua = rand.Next(1, Math.Min(5, sanPham.SoLuongTon.Value + 1)); // Đặt giới hạn mua
                            var chiTiet = new Cthoadon
                            {
                                MaCtsp = sanPham.MaSp,
                                MaHd = hoaDon.MaHd, // Đây là ID đã được auto-generate
                                SoLuong = soLuongMua,
                                //Gia = (decimal)sanPham.DonGia,
                                //ThanhTien = soLuongMua * (decimal)sanPham.DonGia
                            };

                            // Thêm vào danh sách
                            Cthoadons.Add(chiTiet);
                            chiTietSanPhamOfHoaDon.Add(chiTiet);

                            // Đánh dấu sản phẩm đã thêm
                            sanPhamsDuocChon.Add(key);
                        }
                    }

                    // Tính các tổng giá trị hóa đơn
                    //var tienGoc = chiTietSanPhamOfHoaDon.Sum(ct => ct.ThanhTien);
                    //hoaDon.TienGoc = (float)tienGoc;
                    //hoaDon.TongTien = (float)tienGoc + hoaDon.PhiVanChuyen - hoaDon.GiamGiaMaCoupon;
                }


                // Lưu danh sách chi tiết hóa đơn và cập nhật hóa đơn
                _db.Cthoadons.AddRange(Cthoadons);
                _db.Hoadons.UpdateRange(hoaDons);

                // Lưu tất cả thay đổi
                _db.SaveChanges();

                Console.WriteLine("Tạo hóa đơn và chi tiết hóa đơn thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi tạo hóa đơn: {ex.Message}");
            }
        }
    }
}
