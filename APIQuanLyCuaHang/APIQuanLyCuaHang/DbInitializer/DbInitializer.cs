using APIQuanLyCuaHang.Constants;
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
                CreateATestAccount();
            }
            if (!_db.Nhanviens.Any(nv => nv.Email == "staff666@gmail.com"))
            {
                CreateTestStaffAccount();
            }
        }
        private void CreateATestAccount()
        {
            // ? Tạo tài khoản mặc định
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
        private void CreateTestStaffAccount()
        {
            // ? Tài khoản mặc định
            // @param MaChucVu 1=Admin, 2==Cửa hàng trưởng, 3=Nhân viên bếp,4=Thu ngân,5=Phục vụ
            var defaultStaff = new Nhanvien
            {
                HoTen = RandomData_DB.Instance.rdName(),
                NgaySinh = DateOnly.FromDateTime(DateTime.Now.AddYears(-20)),
                DiaChi = RandomData_DB.Instance.rdAddress(),
                Cccd = "",
                GioiTinh = "Nam",
                Sdt = RandomData_DB.Instance.RandomPhone(),
                Email = "staff666@gmail.com",
                NgayVaoLam = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)),
                TenTaiKhoan = "staff666",
                MatKhau = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                TinhTrang = "Ổn",
                IsDelete = false,
                MaChucVu = 1,
            };
            // Lưu vào cơ sở dữ liệu
            _db.Nhanviens.Add(defaultStaff);
            _db.SaveChanges();
        }
        // Sửa đổi GenerateOrders để nhận kiểu dữ liệu cụ thể thay vì dynamic và bổ sung các trường còn thiếu
        private List<Hoadon> GenerateOrders(int numberOfOrders, Random rand, List<Khachhang> customers, List<int> employeeIds, List<Chitietsanpham> products)
        {
            var hoaDons = new List<Hoadon>();

            for (int i = 0; i < numberOfOrders; i++)
            {
                // Chọn ngẫu nhiên khách hàng và nhân viên
                var customer = customers[rand.Next(customers.Count)];
                int? employeeId = employeeIds.Count > 0 ? employeeIds[rand.Next(employeeIds.Count)] : (int?)null;

                // Chọn trạng thái đơn hàng và tạo thời gian ngẫu nhiên
                string orderStatus = TrangThaiDonHang.TatCaTrangThai[rand.Next(TrangThaiDonHang.TatCaTrangThai.Count)];
                DateTime creationDate = GenerateOrderDate(rand);
                DateTime startShippingDate = creationDate.AddDays(3);
                DateTime receivedDate = startShippingDate.AddDays(7);

                // Xử lý ngày thanh toán
                DateTime? paymentDate = null;
                if (orderStatus == TrangThaiDonHang.DaXacNhan || orderStatus == TrangThaiDonHang.HoanTra_HoanTien)
                {
                    paymentDate = creationDate.AddDays(rand.Next(1, 3)); // Ngày thanh toán sau 1-3 ngày từ ngày tạo đơn
                }

                // Tính toán tiền gốc ngẫu nhiên (giả lập)
                decimal basePrice = products.Count > 0
                    ? rand.Next(100000, 1000000) / 1000 * 1000 // Tạo giá trị ngẫu nhiên từ 100,000 - 1,000,000 (làm tròn nghìn đồng)
                    : 0;

                var order = new Hoadon
                {
                    MaKh = customer.MaKh,
                    MaNv = employeeId,
                    DiaChiNhanHang = RandomData_DB.Instance.rdAddress(), // Sinh địa chỉ ngẫu nhiên
                    NgayTao = creationDate,
                    BatDauGiao = startShippingDate,
                    NgayNhan = receivedDate,
                    HinhThucTt = rand.Next(0, 2) == 0 ? "COD" : "VNPAY", // Xác định hình thức thanh toán
                    TinhTrang = orderStatus, // Trạng thái đơn hàng
                    HoTen = customer.HoTen,
                    Sdt = customer.Sdt ?? "N/A",
                    NgayThanhToan = paymentDate, // Gán ngày thanh toán nếu có áp dụng
                    TienGoc = basePrice, // Gán số tiền gốc tạm tính
                    MoTa = RandomData_DB.Instance._Descript(), // Mô tả ngẫu nhiên
                    PhiVanChuyen = rand.NextDouble() > 0.5 ? 40000 : 50000 // Tính phí vận chuyển ngẫu nhiên (40k hoặc 50k)
                };

                // Xử lý khi trạng thái là "Đã hủy" hoặc "Hoàn trả/Hoàn tiền"
                if (orderStatus == TrangThaiDonHang.DaHuy || orderStatus == TrangThaiDonHang.HoanTra_HoanTien)
                {
                    order.LyDoHuy = "Đơn hàng bị hủy vì được lựa chọn.";
                }

                // Thêm hóa đơn vào danh sách
                hoaDons.Add(order);
            }

            return hoaDons;
        }

        // Cập nhật GenerateOrderDetails đảm bảo không có trùng lặp trong một hóa đơn
        private void GenerateOrderDetails(List<Hoadon> orders, Random rand, List<Chitietsanpham> products, List<Cthoadon> orderDetails)
        {
            foreach (var order in orders)
            {
                int numOfProducts = rand.Next(1, 5); // Random number of products (1-5)
                var selectedProducts = new HashSet<int>(); // Tránh trùng lặp MaCtsp trong cùng hóa đơn

                for (int j = 0; j < numOfProducts; j++)
                {
                    // Lấy sản phẩm ngẫu nhiên
                    var product = products[rand.Next(products.Count)];

                    // Kiểm tra nếu sản phẩm có tồn tại
                    if (product != null && product.SoLuongTon > 0 && !selectedProducts.Contains(product.MaCtsp))
                    {
                        int quantity = rand.Next(1, Math.Min(5, product.SoLuongTon.Value + 1));

                        var detail = new Cthoadon
                        {
                            MaCtsp = product.MaCtsp, // Khóa chính với bảng CHITIETSANPHAM
                            MaHd = order.MaHd,       // Khóa chính với Hoadon
                            SoLuong = quantity
                        };

                        // Xác minh tồn tại trong bảng CHITIETSANPHAM thông qua _db context hoặc danh sách `products`
                        if (_db.Chitietsanphams.Any(p => p.MaCtsp == detail.MaCtsp))
                        {
                            orderDetails.Add(detail);
                            selectedProducts.Add(product.MaCtsp); // Đánh dấu sản phẩm đã chọn
                        }

                    }
                }
            }
        }



        // Phương thức tạo ngày ngẫu nhiên
        private DateTime GenerateOrderDate(Random rand)
        {
            int randomRange = rand.Next(1, 101); // Random percentage
            if (randomRange <= 10)
                return DateTime.Now; // 10% orders created today
            else if (randomRange <= 30)
                return DateTime.Now.AddDays(-rand.Next(0, 7)); // 20% orders created within the last week
            else if (randomRange <= 60)
                return DateTime.Now.AddDays(-rand.Next(0, 30)); // 30% orders created within the last month
            else
                return DateTime.Now.AddDays(-rand.Next(0, 365 * 4)); // 40% orders created within the last year
        }

        // Phương thức dành cho tạo ngẫu nhiên hóa đơn cho charts
        public void CreateOrderForCharts()
        {
            try
            {
                var customers = _db.Khachhangs.ToList();
                var employeeIds = _db.Nhanviens.Select(x => x.MaNv).ToList();
                var products = _db.Chitietsanphams.ToList();
                Random rand = new Random();

                var orders = GenerateOrders(500, rand, customers, employeeIds, products);
                _db.Hoadons.AddRange(orders);
                _db.SaveChanges();

                var orderDetails = new List<Cthoadon>();
                GenerateOrderDetails(orders, rand, products, orderDetails);

                _db.Cthoadons.AddRange(orderDetails);
                _db.SaveChanges();

                Console.WriteLine("Tạo hóa đơn và chi tiết hóa đơn thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi tạo hóa đơn: {ex.Message}");
            }
        }

        // Phương thức dành cho khách hàng test
        private void CreateOrderForTestUser(string emailUser)
        {
            try
            {
                // Lấy thông tin khách hàng
                var customer = _db.Khachhangs.FirstOrDefault(kh => kh.Email == emailUser);
                if (customer == null) throw new Exception($"Không tìm thấy khách hàng với email: {emailUser}");

                // Lấy sản phẩm với kiểm tra tồn kho
                var products = _db.Chitietsanphams.Where(p => p.SoLuongTon > 0).ToList();
                if (!products.Any())
                {
                    throw new Exception("Không có sản phẩm nào tồn tại trong kho (CHITIETSANPHAM) để tạo hóa đơn!");
                }

                Random rand = new Random();

                // Tạo đơn hàng cho khách hàng
                var orders = GenerateOrders(10, rand, new List<Khachhang> { customer }, new List<int>(), products);

                // Lưu danh sách hóa đơn
                _db.Hoadons.AddRange(orders);
                _db.SaveChanges(); // Lưu hóa đơn để có MaHd (ID tự tăng)

                // Tạo danh sách chi tiết hóa đơn
                var orderDetails = new List<Cthoadon>();
                GenerateOrderDetails(orders, rand, products, orderDetails);

                // Lưu chi tiết hóa đơn
                _db.Cthoadons.AddRange(orderDetails);
                _db.SaveChanges(); // Lưu chi tiết  

                Console.WriteLine($"Tạo đơn hàng thành công cho khách hàng: {customer.HoTen}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi tạo đơn hàng cho khách hàng test: {ex.Message}");
            }
        }

    }
}
