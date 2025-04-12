
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IProduct productRepository;
        private readonly QuanLyCuaHangContext db;
        private readonly ILogger<HomeController> _logger;
        private readonly AuthSettings _authSettings;
        public HomeController(IProduct productRepository, QuanLyCuaHangContext db, ILogger<HomeController> logger, IOptions<AuthSettings> authSettings)
        {
            this.productRepository = productRepository;
            this.db = db;
            this._logger = logger;
            this._authSettings = authSettings.Value;
        }

        // Lấy tất cả sản phẩm
        [HttpGet("all-products")]
        public async Task<IActionResult> GetAllProducts([FromQuery] string? search, int? filterCategories, string? sort, string? filterPrices)
        {
            var products = await productRepository.GetAll(search, filterCategories, sort, filterPrices);

            return Ok(new
            {
                Data = products,
            });
        }

        // Lấy chi tiết sản phẩm theo ID
        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await productRepository.GetById(id);
            if (product == null)
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm với ID {id}." });
            }
            return Ok(product);
        }

        // Lấy tất cả combo
        [HttpGet("combos")]
        public async Task<IActionResult> GetAllCombos()
        {
            var combos = await db.Combos
                .Where(c => c.IsDelete == false)
                .Include(c => c.Chitietcombos)
                .ThenInclude(ct => ct.MaSpNavigation)
                .ThenInclude(sp => sp.Chitietsanphams)
                .Select(c => new ComboResponseDTO
                {
                    MaCombo = c.MaCombo,
                    TenCombo = c.TenCombo,
                    Hinh = c.Hinh,
                    SoTienGiam = c.SoTienGiam,
                    PhanTramGiam = c.PhanTramGiam,
                    SoLuong = c.SoLuong,
                    MoTa = c.MoTa,
                    IsDelete = c.IsDelete,
                    Chitietcombos = c.Chitietcombos.Select(ct => new DetaisComboResponseDTO
                    {
                        MaSp = ct.MaSp,
                        TenSp = ct.MaSpNavigation.TenSanPham,
                        SoLuongSp = ct.SoLuongSp,
                        Chitietsanphams = ct.MaSpNavigation.Chitietsanphams.Select(ctsp => new DetailProductResponseDTO
                        {
                            MaCtsp = ctsp.MaCtsp,
                            MaSp = ctsp.MaSp,
                            KichThuoc = ctsp.KichThuoc,
                            HuongVi = ctsp.HuongVi,
                            SoLuongTon = ctsp.SoLuongTon,
                            DonGia = ctsp.DonGia,
                            AnhDaiDien = ctsp.Hinhanhs.OrderBy(img => img.MaHinhAnh).Select(img => img.TenHinhAnh).FirstOrDefault(),
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();

            return Ok(combos);
        }

        [HttpGet("combos/{id}")]
        public async Task<IActionResult> GetComboById(int id)
        {
            try
            {
                var comboQuery = await db.Combos
                .Include(c => c.Chitietcombos)
                .ThenInclude(ct => ct.MaSpNavigation)
                .ThenInclude(sp => sp.Chitietsanphams)
                .ThenInclude(ctsp => ctsp.Hinhanhs)
                .Select(c => new ComboResponseDTO
                {
                    MaCombo = c.MaCombo,
                    TenCombo = c.TenCombo,
                    Hinh = c.Hinh,
                    SoTienGiam = c.SoTienGiam,
                    PhanTramGiam = c.PhanTramGiam,
                    SoLuong = c.SoLuong,
                    MoTa = c.MoTa,
                    IsDelete = c.IsDelete,
                    Chitietcombos = c.Chitietcombos.Select(ct => new DetaisComboResponseDTO
                    {
                        MaSp = ct.MaSp,
                        TenSp = ct.MaSpNavigation.TenSanPham,
                        SoLuongSp = ct.SoLuongSp,
                        Chitietsanphams = ct.MaSpNavigation.Chitietsanphams.Select(ctsp => new DetailProductResponseDTO
                        {
                            MaCtsp = ctsp.MaCtsp,
                            TenSanPham = ct.MaSpNavigation.TenSanPham,
                            MaSp = ctsp.MaSp,
                            KichThuoc = ctsp.KichThuoc,
                            HuongVi = ctsp.HuongVi,
                            SoLuongTon = ctsp.SoLuongTon,
                            DonGia = ctsp.DonGia,
                            AnhDaiDien = ctsp.Hinhanhs.OrderBy(img => img.MaHinhAnh).Select(img => img.TenHinhAnh).FirstOrDefault(),
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync(p => p.MaCombo == id);

                if (comboQuery == null)
                {
                    return NotFound(new { message = $"Không tìm thấy combo với ID {id}." });
                }

                //var combo = comboQuery.Combo;
                //var chitietcombos = comboQuery.Chitietcombos;

                //// Tính số lượng miếng gà (giả sử danh mục "Gà" có MaDanhMuc = 1)
                //var chickenCategoryId = 1; // Thay đổi theo danh mục thực tế
                //var soLuongMiengGa = chitietcombos
                //    .Where(ct => ct.MaDanhMuc == chickenCategoryId)
                //    .Sum(ct => ct.SoLuongSp ?? 0);

                //var totalPrice = chitietcombos.Any()
                //    ? chitietcombos.Sum(ct => (ct.DonGia ?? 0) * (ct.SoLuongSp ?? 0))
                //    : 0;

                //var comboResponse = new ComboResponseDTO
                //{
                //    MaCombo = combo.MaCombo,
                //    TenCombo = combo.TenCombo,
                //    Hinh = combo.Hinh,
                //    SoTienGiam = combo.SoTienGiam,
                //    PhanTramGiam = combo.PhanTramGiam,
                //    SoLuong = combo.SoLuong,
                //    MoTa = combo.MoTa,
                //    IsDelete = combo.IsDelete,
                //    SoLuongMiengGa = soLuongMiengGa, // Thêm trường này
                //    TotalPrice = totalPrice,
                //    DiscountedPrice = combo.SoTienGiam.HasValue ? totalPrice - combo.SoTienGiam : totalPrice,
                //    Chitietcombos = chitietcombos.Select(ct => new DetaisComboResponseDTO
                //    {
                //        MaSp = ct.MaSp,
                //        TenSp = ct.MaSpNavigation != null ? ct.MaSpNavigation.TenSanPham : "Không xác định",
                //        SoLuongSp = ct.SoLuongSp
                //    }).ToList()
                //};

                return Ok(comboQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetComboById: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Lỗi server khi lấy chi tiết combo.", detail = ex.Message });
            }
        }

        //Lấy sản phẩm bán chạy
        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers()
        {
            //var bestSellers = await db.Cthoadons
            //    .GroupBy(ct => ct.MaCtsp)
            //    .Select(g => new
            //    {
            //        MaCtsp = g.Key,
            //        TotalSold = g.Sum(ct => ct.SoLuong)
            //    })
            //    .OrderByDescending(x => x.TotalSold)
            //    .Take(5)
            //    .Join(db.Chitietsanphams
            //        .Include(ct => ct.MaSpNavigation)
            //        .Include(ct => ct.Hinhanhs),
            //        ct => ct.MaCtsp,
            //        sp => sp.MaCtsp,
            //        (ct, sp) => new DetailProductResponseDTO
            //        {
            //            MaCtsp = sp.MaCtsp,
            //            MaSp = sp.MaSp,
            //            TenSanPham = sp.MaSpNavigation.TenSanPham,
            //            KichThuoc = sp.KichThuoc ?? "NO",
            //            HuongVi = sp.HuongVi ?? "NO",
            //            DonGia = sp.DonGia,
            //            SoLuongTon = ct.TotalSold,
            //            AnhDaiDien = sp.Hinhanhs.FirstOrDefault() != null ? sp.Hinhanhs.FirstOrDefault().TenHinhAnh : null,
            //            Hinhanhs = sp.Hinhanhs.Select(img => new ImageProductResponseDTO
            //            {
            //                MaHinhAnh = img.MaHinhAnh,
            //                TenHinhAnh = img.TenHinhAnh,
            //                MaCtsp = img.MaCtsp,
            //            }).ToList(),
            //        })
            //    .ToListAsync();
            var bestSellers = await db.Cthoadons.GroupBy(cthd => cthd.MaCtsp)
                                .Select(g => new
                                {
                                    MaCtsp = g.Key,
                                    SoLuongBan = g.Sum(ct => ct.SoLuong)
                                })
                                .OrderByDescending(x => x.SoLuongBan)
                                .Take(5)
                                .ToListAsync();
            var bestSellerIds = bestSellers.Select(bs => bs.MaCtsp).ToList();
            var detailproductSellers = await db.Sanphams
            .Include(p => p.Chitietsanphams)  // Load danh sách chi tiết sản phẩm
            .ThenInclude(img => img.Hinhanhs)
            .Where(p => p.Chitietsanphams.Any(ct => bestSellerIds.Contains(ct.MaCtsp)))
            .Select(product => new ProductResponseDTO
            {
                MaSp = product.MaSp,
                TenSanPham = product.TenSanPham,
                KhoangGia = product.Chitietsanphams.Where(p => p.IsDelete == false).Any()
                    ? (product.Chitietsanphams.Where(p => p.IsDelete == false).Min(p => p.DonGia) == product.Chitietsanphams.Where(p => p.IsDelete == false).Max(p => p.DonGia)
                        ? $"{product.Chitietsanphams.Where(p => p.IsDelete == false).Min(p => p.DonGia)} VNĐ"
                        : $"{product.Chitietsanphams.Where(p => p.IsDelete == false).Min(p => p.DonGia)} VNĐ - {product.Chitietsanphams.Where(p => p.IsDelete == false).Max(p => p.DonGia)} VNĐ")
                    : "Chưa có giá",
                MaDanhMuc = product.MaDanhMuc,
                TenDanhMuc = product.MaDanhMucNavigation != null ? product.MaDanhMucNavigation.TenDanhMuc : "Không xác định",
                MoTa = product.MoTa,
                IsDelete = product.IsDelete,
                Chitietsanphams = product.Chitietsanphams
                    .Where(p => p.IsDelete == false)
                    .Select(details => new DetailProductResponseDTO
                    {
                        MaCtsp = details.MaCtsp,
                        MaSp = details.MaSp,
                        TenSanPham = details.MaSpNavigation.TenSanPham,
                        KichThuoc = details.KichThuoc ?? "NO",
                        HuongVi = details.HuongVi ?? "NO",
                        SoLuongTon = details.SoLuongTon,
                        DonGia = details.DonGia,
                        AnhDaiDien = details.Hinhanhs
                            .OrderBy(img => img.MaHinhAnh)
                            .Select(img => img.TenHinhAnh)
                            .FirstOrDefault() ?? "no_image.png"
                    })
                    .ToList()
            })
            .ToListAsync();

            return Ok(detailproductSellers);
        }

        // Lấy chi tiết sản phẩm bán chạy theo ID
        //[HttpGet("best-sellers/{id}")]
        //public async Task<IActionResult> GetBestSellerById(int id)
        //{
        //    var bestSeller = await db.Chitietsanphams
        //        .Where(ct => ct.MaCtsp == id && ct.IsDelete == false)
        //        .Include(ct => ct.MaSpNavigation)
        //        .Include(ct => ct.Hinhanhs)
        //        .Select(sp => new ProductBestSellerDTO
        //        {
        //            MaCtsp = sp.MaCtsp,
        //            MaSp = sp.MaSp,
        //            TenSanPham = sp.MaSpNavigation.TenSanPham,
        //            KichThuoc = sp.KichThuoc ?? "NO",
        //            HuongVi = sp.HuongVi ?? "NO",
        //            DonGia = sp.DonGia,
        //            Hinh = sp.Hinhanhs.FirstOrDefault() != null ? sp.Hinhanhs.FirstOrDefault().TenHinhAnh : null
        //        })
        //        .FirstOrDefaultAsync();

        //    if (bestSeller == null)
        //    {
        //        return NotFound(new { message = $"Không tìm thấy sản phẩm bán chạy với ID {id}." });
        //    }

        //    return Ok(bestSeller);
        //}

        // Lấy danh sách danh mục
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await db.Danhmucs
                .Where(d => d.IsDelete == false)
                .Select(d => new CategoryResponseDTO
                {
                    MaDanhMuc = d.MaDanhMuc,
                    TenDanhMuc = d.TenDanhMuc,

                    HinhDaiDien = db.Sanphams
                        .Where(s => s.MaDanhMuc == d.MaDanhMuc && s.IsDelete == false)
                        .Include(s => s.Chitietsanphams)
                        .ThenInclude(ct => ct.Hinhanhs)
                        .Select(s => s.Chitietsanphams.FirstOrDefault() != null
                            ? (s.Chitietsanphams.FirstOrDefault().Hinhanhs.FirstOrDefault() != null
                                ? s.Chitietsanphams.FirstOrDefault().Hinhanhs.FirstOrDefault().TenHinhAnh
                                : null)
                            : null)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var bestSellerData = await db.Cthoadons
                .GroupBy(ct => ct.MaCtsp)
                .Select(g => new
                {
                    MaCtsp = g.Key,
                    TotalSold = g.Sum(ct => ct.SoLuong)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(1)
                .Join(db.Chitietsanphams,
                    ct => ct.MaCtsp,
                    sp => sp.MaCtsp,
                    (ct, sp) => new { sp.MaCtsp })
                .FirstOrDefaultAsync();

            var specialCategories = new List<CategoryResponseDTO>
            {
                new CategoryResponseDTO
                {
                    MaDanhMuc = -3,
                    TenDanhMuc = "Tất cả đồ ăn",
                    HinhDaiDien = await db.Sanphams
                        .Where(s => s.IsDelete == false)
                        .Include(s => s.Chitietsanphams)
                        .ThenInclude(ct => ct.Hinhanhs)
                        .Select(s => s.Chitietsanphams.FirstOrDefault() != null
                            ? (s.Chitietsanphams.FirstOrDefault().Hinhanhs.FirstOrDefault() != null
                                ? s.Chitietsanphams.FirstOrDefault().Hinhanhs.FirstOrDefault().TenHinhAnh
                                : null)
                            : null)
                        .FirstOrDefaultAsync()
                },
                new CategoryResponseDTO
                {
                    MaDanhMuc = -1,
                    TenDanhMuc = "Món ngon phải thử",
                    HinhDaiDien = await db.Combos
                        .Where(c => c.IsDelete == false)
                        .Select(c => c.Hinh)
                        .FirstOrDefaultAsync()
                },
                new CategoryResponseDTO
                {
                    MaDanhMuc = -2,
                    TenDanhMuc = "Món bán chạy",
                    HinhDaiDien = bestSellerData != null
                        ? await db.Chitietsanphams
                            .Include(ct => ct.Hinhanhs)
                            .Where(ct => ct.MaCtsp == bestSellerData.MaCtsp)
                            .Select(ct => ct.Hinhanhs.FirstOrDefault() != null
                                ? ct.Hinhanhs.FirstOrDefault().TenHinhAnh
                                : null)
                            .FirstOrDefaultAsync()
                        : null
                }
            };

            categories.InsertRange(0, specialCategories);
            return Ok(categories);
        }
        [HttpPost("TraLoi")]
        public async Task<IActionResult> TraLoi([FromBody] ChatRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Bắt đầu TraLoi với input: {UserInput}, Confirmation: {Confirmation}", request.UserInput, request.Confirmation);

                var maKhachHang = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                bool isLoggedIn = !string.IsNullOrEmpty(maKhachHang);

                // Product list as HTML table
                string productList = "<table style='width:100%; border-collapse: collapse;'><tr><th>Mã</th><th>Tên</th><th>Giá</th><th>Còn</th><th>Chi Tiết</th></tr>";
                var products = await db.Sanphams
                    .Where(p => p.IsDelete == false)
                    .Include(p => p.Chitietsanphams)
                    .Where(p => p.Chitietsanphams.Any(ct => ct.SoLuongTon > 0 && ct.IsDelete == false))
                    .ToListAsync();
                foreach (var product in products.Select((p, i) => new { Product = p, Index = i }))
                {
                    var ctsp = product.Product.Chitietsanphams.FirstOrDefault(ct => ct.SoLuongTon > 0 && ct.IsDelete == false);
                    if (ctsp == null) continue;
                    productList += $"<tr>" +
                                   $"<td>{product.Product.MaSp}</td>" +
                                   $"<td>{product.Product.TenSanPham}</td>" +
                                   $"<td>{ctsp.DonGia} VNĐ</td>" +
                                   $"<td>{ctsp.SoLuongTon}</td>" +
                                   $"<td><button class='btn btn-primary' onclick='navigateToProduct({product.Product.MaSp})'>Xem Chi Tiết</button></td>" +
                                   $"</tr>";
                }
                productList += "</table>";

                // Combo list as HTML table
                string comboList = "<table style='width:100%; border-collapse: collapse;'><tr><th>Mã</th><th>Tên</th><th>Giá</th><th>Còn</th><th>Chi Tiết</th></tr>";
                var combos = await db.Combos
                    .Where(c => c.IsDelete == false && c.SoLuong > 0)
                    .Include(c => c.Chitietcombos)
                    .ThenInclude(ct => ct.MaSpNavigation)
                    .ThenInclude(sp => sp.Chitietsanphams)
                    .ToListAsync();
                foreach (var combo in combos.Select((c, i) => new { Combo = c, Index = i }))
                {
                    comboList += $"<tr>" +
                                 $"<td>{combo.Combo.MaCombo}</td>" +
                                 $"<td>{combo.Combo.TenCombo}</td>" +
                                 $"<td>{CalculateComboPrice(combo.Combo)} VNĐ</td>" +
                                 $"<td>{combo.Combo.SoLuong}</td>" +
                                 $"<td><button class='btn btn-primary' onclick='navigateToCombo({combo.Combo.MaCombo})'>Xem Chi Tiết</button></td>" +
                                 $"</tr>";
                    comboList += "<tr><td colspan='5'>Chi tiết: ";
                    foreach (var detail in combo.Combo.Chitietcombos)
                    {
                        var variants = detail.MaSpNavigation.Chitietsanphams.Where(ct => ct.IsDelete == false);
                        comboList += $"{detail.SoLuongSp} {detail.MaSpNavigation.TenSanPham} (Tùy chọn: {string.Join(", ", variants.Select(v => $"{v.HuongVi ?? "Thường"}"))}) ";
                    }
                    comboList += "</td></tr>";
                }
                comboList += "</table>";

                string instructions = "Hỏi 'Thêm [mã sản phẩm] vào giỏ' hoặc 'Thêm combo [mã combo] [tùy chọn: cay/thường]' để thêm, 'Thanh toán' để thanh toán.";
                string answer = "";

                // Xử lý xác nhận từ lần trước (ưu tiên)
                if (!string.IsNullOrEmpty(request.PreviousAnswer) && request.Confirmation.HasValue)
                {
                    if (request.PreviousAnswer.Contains("Bạn có muốn đưa sản phẩm này vào giỏ hàng không?"))
                    {
                        if (request.Confirmation.Value)
                        {
                            if (!isLoggedIn)
                            {
                                answer = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng!";
                            }
                            else
                            {
                                var maSanPham = ExtractMaSanPhamFromRecommendation(request.PreviousAnswer);
                                if (maSanPham != null && products.Any(p => p.MaSp == int.Parse(maSanPham)))
                                {
                                    using (var client = new HttpClient())
                                    {
                                        answer = await AddToCart(client, maSanPham, maKhachHang);
                                        if (answer == "Đã thêm vào giỏ!")
                                        {
                                            answer += "<br>Bạn muốn mua tiếp hay thanh toán? [Continue/Checkout]";
                                        }
                                    }
                                }
                                else
                                {
                                    answer = "Sản phẩm không hợp lệ!";
                                }
                            }
                        }
                        else
                        {
                            answer = "Đã hủy thêm sản phẩm vào giỏ.";
                        }
                    }
                    else if (request.PreviousAnswer.Contains("Bạn muốn mua tiếp hay thanh toán?"))
                    {
                        if (request.Confirmation.Value) // Continue
                        {
                            answer = productList + "<br>" + instructions;
                        }
                        else // Checkout
                        {
                            using (var client = new HttpClient())
                            {
                                answer = await ProcessCheckout(client, maKhachHang);
                            }
                        }
                    }
                    // ... (các logic xác nhận khác)
                }
                // Xử lý yêu cầu "Danh sách sản phẩm"
                else if (!string.IsNullOrEmpty(request.UserInput) && request.UserInput.ToLower().Contains("danh sách sản phẩm"))
                {
                    answer = $"Dưới đây là danh sách sản phẩm:<br>{productList}<br>{instructions}";
                }
                // Xử lý yêu cầu "Tư vấn sản phẩm"
                else if (!string.IsNullOrEmpty(request.UserInput) && request.UserInput.ToLower().Contains("tư vấn cho tôi sản phẩm"))
                {
                    var recommendedProduct = products.FirstOrDefault(p => p.MaSp == 1001); // Tư vấn sản phẩm 1001
                    if (recommendedProduct != null)
                    {
                        var ctsp = recommendedProduct.Chitietsanphams.FirstOrDefault(ct => ct.SoLuongTon > 0 && ct.IsDelete == false);
                        answer = $"Tôi khuyên bạn nên thử sản phẩm:<br>" +
                                 $"<strong>Mã:</strong> {recommendedProduct.MaSp}<br>" +
                                 $"<strong>Tên:</strong> {recommendedProduct.TenSanPham}<br>" +
                                 $"<strong>Giá:</strong> {ctsp?.DonGia ?? 0} VNĐ<br>" +
                                 $"<button class='btn btn-primary' onclick='navigateToProduct({recommendedProduct.MaSp})'>Xem Chi Tiết</button><br>" +
                                 $"Bạn có muốn đưa sản phẩm này vào giỏ hàng không? [Yes/No]";
                    }
                    else
                    {
                        answer = "Sản phẩm với mã 1001 không tồn tại hoặc đã hết hàng.";
                    }
                }
                else if (string.IsNullOrEmpty(request.UserInput) && !request.Confirmation.HasValue)
                {
                    return BadRequest(new { response = "Vui lòng nhập câu hỏi hoặc xác nhận!" });
                }
                else
                {
                    // Gửi yêu cầu tới Gemini API cho các câu hỏi khác
                    var requestBody = new
                    {
                        contents = new[]
                        {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = $"{productList}<br>{comboList}<br>{instructions}<br>" +
                                       "Yêu cầu: Trả lời ngắn gọn, dùng <br> để xuống dòng. Nếu cần xác nhận, thêm [Yes/No] vào cuối câu trả lời.<br>" +
                                       "Hỗ trợ: Trả lời câu hỏi về cửa hàng, thêm sản phẩm/combo vào giỏ nếu yêu cầu, thanh toán khi yêu cầu.<br>" +
                                       "Câu hỏi: " + request.UserInput
                            }
                        }
                    }
                }
                    };

                    var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync($"{_authSettings.Google.GoogleAPIUrl}?key={_authSettings.Google.GoogleAPIKey}", content);
                        response.EnsureSuccessStatusCode();
                        var responseString = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                        answer = responseObject?.candidates[0].content?.parts[0]?.text ?? "Xin lỗi, không nhận được phản hồi từ API!";

                        // Kiểm tra các yêu cầu khác (Thêm vào giỏ, Thanh toán)
                        if (answer.Contains("Thêm") && answer.Contains("vào giỏ") && !request.Confirmation.HasValue)
                        {
                            if (answer.Contains("combo"))
                            {
                                var (maCombo, _) = ExtractComboDetails(answer);
                                if (combos.Any(c => c.MaCombo == int.Parse(maCombo)))
                                {
                                    answer += " [Yes/No]";
                                }
                            }
                            else
                            {
                                var maSanPham = ExtractMaSanPham(answer);
                                if (products.Any(p => p.MaSp == int.Parse(maSanPham)))
                                {
                                    answer += " [Yes/No]";
                                }
                            }
                        }
                        else if (answer.Contains("Thanh toán") && !request.Confirmation.HasValue)
                        {
                            answer += " [Yes/No]";
                        }
                    }
                }

                return Ok(new { response = answer });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong TraLoi");
                return StatusCode(500, new { response = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        private string ExtractMaSanPhamFromRecommendation(string text)
        {
            var match = Regex.Match(text, @"<strong>Mã:</strong>\s*(\d+)<br>");
            return match.Success ? match.Groups[1].Value : null;
        }

        // Hàm trích xuất tên sản phẩm
        private string ExtractProductName(string text)
        {
            var match = Regex.Match(text, @"Thêm\s+(.+?)(?:\s+vào\s+giỏ|giỏ\s+hàng)", RegexOptions.IgnoreCase);
            if (match.Success && !Regex.IsMatch(match.Groups[1].Value, @"^\d+$")) // Không phải số
            {
                return match.Groups[1].Value.Trim().ToLower();
            }
            return null;
        }

        // Hàm trích xuất tên combo
        private string ExtractComboName(string text)
        {
            var match = Regex.Match(text, @"Thêm\s+combo\s+(.+?)(?:\s+vào\s+giỏ|giỏ\s+hàng)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var name = match.Groups[1].Value.Trim().ToLower();
                // Loại bỏ phần "(mã X)" nếu có
                var cleanedName = Regex.Replace(name, @"\s*\(mã\s+\d+\)\s*", "").Trim();
                return cleanedName;
            }
            return null;
        }

        // Hàm trích xuất mã combo từ tên nếu có
        private string ExtractMaFromComboName(string text)
        {
            var match = Regex.Match(text, @"\(mã\s+(\d+)\)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : null;
        }

        // Phương thức tính giá combo dựa trên Chitietcombo
        private decimal CalculateComboPrice(Combo combo)
        {
            var totalPrice = combo.Chitietcombos
                .Sum(ct => ct.MaSpNavigation.Chitietsanphams
                    .Where(ctsp => ctsp.IsDelete == false)
                    .FirstOrDefault()?.DonGia * ct.SoLuongSp ?? 0);
            return combo.PhanTramGiam.HasValue
                ? totalPrice * (decimal)(1 - combo.PhanTramGiam.Value / 100)
                : totalPrice - (combo.SoTienGiam ?? 0);
        }

        // Thêm combo vào giỏ hàng
        private async Task<string> AddComboToCart(HttpClient client, string maCombo, string options, string maKhachHang)
        {
            var combo = await db.Combos
                .Include(c => c.Chitietcombos)
                .ThenInclude(ct => ct.MaSpNavigation)
                .ThenInclude(sp => sp.Chitietsanphams)
                .FirstOrDefaultAsync(c => c.MaCombo == int.Parse(maCombo));
            if (combo == null) return "Combo không tồn tại!";

            if (combo.SoLuong <= 0) return "Combo đã hết hàng!";

            var optionList = options?.Split(',').Select(o => o.Trim().ToLower()).ToList() ?? new List<string>();
            var cartDetails = new List<object>();
            foreach (var detail in combo.Chitietcombos)
            {
                var variant = detail.MaSpNavigation.Chitietsanphams
                    .FirstOrDefault(ct => ct.IsDelete == false && (optionList.Contains(ct.HuongVi?.ToLower() ?? "thường") || optionList.Count == 0));
                if (variant == null) return $"Không tìm thấy biến thể '{options}' cho {detail.MaSpNavigation.TenSanPham}!";
                if (variant.SoLuongTon < detail.SoLuongSp) return $"Không đủ hàng cho {detail.MaSpNavigation.TenSanPham}!";
                cartDetails.Add(new { MaCTSp = variant.MaCtsp, DonGia = variant.DonGia, SoLuong = detail.SoLuongSp });
            }

            try
            {
                var payload = new
                {
                    MaKh = int.Parse(maKhachHang),
                    MaCtsp = (int?)null,
                    MaCombo = int.Parse(maCombo),
                    SoLuong = 1,
                    DonGia = CalculateComboPrice(combo),
                    CartDetailRequestCombos = cartDetails
                };

                var addToCartResponse = await client.PostAsync("https://localhost:7139/api/Cart", new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8, "application/json"));

                var responseContent = await addToCartResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Phản hồi từ API Cart: {Response}", responseContent);

                if (!addToCartResponse.IsSuccessStatusCode)
                {
                    return $"Lỗi khi thêm combo vào giỏ: {responseContent}";
                }

                var addToCartResult = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return addToCartResult.success == true
                    ? "Đã thêm combo vào giỏ!"
                    : $"Lỗi: {addToCartResult.message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API AddToCart cho combo");
                return $"Lỗi khi thêm combo vào giỏ: {ex.Message}";
            }
        }

        // Thêm sản phẩm vào giỏ hàng (giữ nguyên)
        private async Task<string> AddToCart(HttpClient client, string maSanPham, string maKhachHang)
        {
            var product = await db.Sanphams
                .Include(p => p.Chitietsanphams)
                .FirstOrDefaultAsync(p => p.MaSp == int.Parse(maSanPham) && p.IsDelete == false);
            if (product == null) return "Sản phẩm không tồn tại!";

            var ctsp = product.Chitietsanphams.FirstOrDefault(ct => ct.IsDelete == false && ct.SoLuongTon > 0);
            if (ctsp == null) return "Sản phẩm đã hết hàng!";

            try
            {
                var payload = new
                {
                    MaKh = int.Parse(maKhachHang),
                    MaCtsp = ctsp.MaCtsp,
                    MaCombo = (int?)null,
                    SoLuong = 1,
                    DonGia = ctsp.DonGia
                };

                var addToCartResponse = await client.PostAsync("https://localhost:7139/api/Cart", new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8, "application/json"));

                var responseContent = await addToCartResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Phản hồi từ API Cart: {Response}", responseContent);

                if (!addToCartResponse.IsSuccessStatusCode)
                {
                    return $"Lỗi khi thêm vào giỏ: {responseContent}";
                }

                var addToCartResult = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return addToCartResult.success == true
                    ? "Đã thêm vào giỏ!"
                    : $"Lỗi: {addToCartResult.message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API AddToCart");
                return $"Lỗi khi thêm vào giỏ: {ex.Message}";
            }
        }

        // Thanh toán (cập nhật với CheckoutController)
        private async Task<string> ProcessCheckout(HttpClient client, string maKhachHang)
        {
            try
            {
                var payload = new
                {
                    MaKhachHang = maKhachHang,
                    HinhThucTt = "COD"
                };

                var checkoutResponse = await client.PostAsync("https://localhost:7139/api/Checkout", new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8, "application/json"));

                var responseContent = await checkoutResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Phản hồi từ API Checkout: {Response}", responseContent);

                if (!checkoutResponse.IsSuccessStatusCode)
                {
                    return $"Lỗi thanh toán: {responseContent}";
                }

                var checkoutResult = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return checkoutResult.success == true
                    ? "Thanh toán thành công! Đơn hàng của bạn đang được xử lý."
                    : $"Lỗi thanh toán: {checkoutResult.message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API Checkout");
                return $"Lỗi thanh toán: {ex.Message}";
            }
        }

        // Trích xuất mã sản phẩm và combo (giữ nguyên)
        private string ExtractMaSanPham(string text)
        {
            var match = Regex.Match(text, @"Thêm\s+(\d+)\s+vào giỏ");
            return match.Success ? match.Groups[1].Value : null;
        }

        private (string maCombo, string options) ExtractComboDetails(string text)
        {
            var match = Regex.Match(text, @"Thêm combo\s+(\d+)\s+\[(.*?)\]\s+vào giỏ", RegexOptions.IgnoreCase);
            return match.Success ? (match.Groups[1].Value, match.Groups[2].Value) : (null, null);
        }
        // Định nghĩa ChatRequestDTO
        public class ChatRequestDTO
        {
            public string UserInput { get; set; }
            public string? PreviousAnswer { get; set; }
            public bool? Confirmation { get; set; }
        }

        // Định nghĩa AuthSettings
        public class AuthSettings
        {
            public GoogleSettings Google { get; set; }
        }

        public class GoogleSettings
        {
            public string GoogleAPIUrl { get; set; }
            public string GoogleAPIKey { get; set; }
        }

    }
}
