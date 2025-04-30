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
using APIQuanLyCuaHang.Services;

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

        // Dictionary để lưu ngữ cảnh của phiên chat (theo user)
        private static readonly Dictionary<string, ChatContext> ChatContexts = new Dictionary<string, ChatContext>();

        public HomeController(IProduct productRepository, QuanLyCuaHangContext db, ILogger<HomeController> logger, IOptions<AuthSettings> authSettings)
        {
            this.productRepository = productRepository;
            this.db = db;
            this._logger = logger;
            this._authSettings = authSettings.Value;
        }
        [HttpGet("recommend/{userId}")]
        public async Task<IActionResult> GetRecommendations(int userId)
        {
            var recommender = new MLRecommendationSystem(db);
            var recommendations = await recommender.Recommend(userId);
            return Ok(recommendations);
        }
        // Các endpoint hiện có (giữ nguyên)
        [HttpGet("all-products")]
        public async Task<IActionResult> GetAllProducts([FromQuery] string? search, int? filterCategories, string? sort, string? filterPrices)
        {
            var products = await productRepository.GetAll(search, filterCategories, sort, filterPrices);
            return Ok(new { Data = products });
        }

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
                return Ok(comboQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetComboById: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Lỗi server khi lấy chi tiết combo.", detail = ex.Message });
            }
        }

        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers()
        {
            var bestSellers = await db.Cthoadons
                .GroupBy(cthd => cthd.MaCtsp)
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
                .Include(p => p.Chitietsanphams)
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
                    MaDanhMuc = -1,
                    TenDanhMuc = "Combo đang hot",
                    HinhDaiDien = await db.Combos
                        .Where(c => c.IsDelete == false)
                        .Select(c => c.Hinh)
                        .FirstOrDefaultAsync()
                },              
            };

            categories.InsertRange(0, specialCategories);
            return Ok(categories);
        }

        [HttpPost("TraLoi")]
        public async Task<IActionResult> TraLoi([FromBody] ChatRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Bắt đầu TraLoi với input: {UserInput}, Confirmation: {Confirmation}, PreviousAnswer: {PreviousAnswer}", request.UserInput, request.Confirmation, request.PreviousAnswer);

                var maKhachHang = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                bool isLoggedIn = !string.IsNullOrEmpty(maKhachHang);
                string sessionId = maKhachHang ?? Guid.NewGuid().ToString(); // Tạo sessionId nếu không đăng nhập

                // Lấy hoặc tạo ngữ cảnh cho phiên chat
                if (!ChatContexts.ContainsKey(sessionId))
                {
                    ChatContexts[sessionId] = new ChatContext();
                }
                var context = ChatContexts[sessionId];

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

                string instructions = "Hỏi 'Thêm [tên sản phẩm] vào giỏ' hoặc 'Thêm combo [tên combo] [tùy chọn: cay/thường]' để thêm, 'Thanh toán' để thanh toán.";
                string answer = "";

                // Xử lý xác nhận Yes/No (ưu tiên)
                if (request.Confirmation.HasValue && !string.IsNullOrEmpty(request.PreviousAnswer))
                {
                    if (context.Action == "AddProductToCart")
                    {
                        if (request.Confirmation.Value) // Yes
                        {
                            if (!isLoggedIn)
                            {
                                answer = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng!";
                            }
                            else
                            {
                                using (var client = new HttpClient())
                                {
                                    answer = await AddToCart(client, context.ProductId, maKhachHang);
                                    if (answer == "Đã thêm vào giỏ!")
                                    {
                                        answer += "<br>Bạn muốn thêm món khác không? [Yes/No]";
                                        context.Action = "AskAddMore";
                                    }
                                }
                            }
                        }
                        else // No
                        {
                            answer = "Đã hủy thêm sản phẩm vào giỏ.<br>Bạn muốn xem danh sách sản phẩm không? [Yes/No]";
                            context.Action = "AskProductList";
                        }
                        context.ProductId = null;
                    }
                    else if (context.Action == "AddComboToCart")
                    {
                        if (request.Confirmation.Value) // Yes
                        {
                            if (!isLoggedIn)
                            {
                                answer = "Vui lòng đăng nhập để thêm combo vào giỏ hàng!";
                            }
                            else
                            {
                                using (var client = new HttpClient())
                                {
                                    answer = await AddComboToCart(client, context.ComboId, context.ComboOptions, maKhachHang);
                                    if (answer == "Đã thêm combo vào giỏ!")
                                    {
                                        answer += "<br>Bạn muốn thêm món khác không? [Yes/No]";
                                        context.Action = "AskAddMore";
                                    }
                                }
                            }
                        }
                        else // No
                        {
                            answer = "Đã hủy thêm combo vào giỏ.<br>Bạn muốn xem danh sách combo không? [Yes/No]";
                            context.Action = "AskComboList";
                        }
                        context.ComboId = null;
                        context.ComboOptions = null;
                    }
                    else if (context.Action == "Checkout")
                    {
                        if (request.Confirmation.Value) // Yes
                        {
                            if (!isLoggedIn)
                            {
                                answer = "Vui lòng đăng nhập để thanh toán!";
                            }
                            else
                            {
                                using (var client = new HttpClient())
                                {
                                    answer = await ProcessCheckout(client, maKhachHang);
                                    context.Clear(); // Xóa ngữ cảnh sau khi thanh toán
                                }
                            }
                        }
                        else // No
                        {
                            answer = "Đã hủy thanh toán.<br>Bạn muốn tiếp tục mua sắm không? [Yes/No]";
                            context.Action = "AskContinueShopping";
                        }
                    }
                    else if (context.Action == "AskAddMore")
                    {
                        if (request.Confirmation.Value) // Yes
                        {
                            answer = productList + "<br>" + comboList + "<br>" + instructions;
                            context.Clear();
                        }
                        else // No
                        {
                            answer = "Bạn muốn thanh toán ngay không? [Yes/No]";
                            context.Action = "Checkout";
                        }
                    }
                    else if (context.Action == "AskProductList")
                    {
                        if (request.Confirmation.Value) // Yes
                        {
                            answer = $"Dưới đây là danh sách sản phẩm:<br>{productList}<br>{instructions}";
                            context.Clear();
                        }
                        else // No
                        {
                            answer = "Bạn muốn tôi tư vấn món ăn không? [Yes/No]";
                            context.Action = "AskRecommendProduct";
                        }
                    }
                    else if (context.Action == "AskComboList")
                    {
                        if (request.Confirmation.Value) // Yes
                        {
                            answer = $"Dưới đây là danh sách combo:<br>{comboList}<br>{instructions}";
                            context.Clear();
                        }
                        else // No
                        {
                            answer = "Bạn muốn tôi tư vấn món ăn không? [Yes/No]";
                            context.Action = "AskRecommendProduct";
                        }
                    }
                    else if (context.Action == "AskContinueShopping")
                    {
                        if (request.Confirmation.Value) // Yes
                        {
                            answer = productList + "<br>" + comboList + "<br>" + instructions;
                            context.Clear();
                        }
                        else // No
                        {
                            answer = "Cảm ơn bạn đã mua sắm! Hẹn gặp lại.";
                            context.Clear();
                        }
                    }
                    else if (context.Action == "AskRecommendProduct")
                    {
                        if (request.Confirmation.Value) // Yes
                        {
                            var bestSellers = await GetBestSellers();
                            var recommendedProduct = ((List<ProductResponseDTO>)((ObjectResult)bestSellers).Value).FirstOrDefault();
                            if (recommendedProduct != null)
                            {
                                var ctsp = recommendedProduct.Chitietsanphams.FirstOrDefault();
                                answer = $"Tôi khuyên bạn nên thử sản phẩm:<br>" +
                                         $"<strong>Mã:</strong> {recommendedProduct.MaSp}<br>" +
                                         $"<strong>Tên:</strong> {recommendedProduct.TenSanPham}<br>" +
                                         $"<strong>Giá:</strong> {ctsp?.DonGia ?? 0} VNĐ<br>" +
                                         $"<button class='btn btn-primary' onclick='navigateToProduct({recommendedProduct.MaSp})'>Xem Chi Tiết</button><br>" +
                                         $"Bạn có muốn đưa sản phẩm này vào giỏ hàng không? [Yes/No]";
                                context.Action = "AddProductToCart";
                                context.ProductId = recommendedProduct.MaSp.ToString();
                            }
                            else
                            {
                                answer = "Hiện tại không có sản phẩm nào để gợi ý.";
                                context.Clear();
                            }
                        }
                        else // No
                        {
                            answer = "Bạn cần tôi giúp gì khác không?";
                            context.Clear();
                        }
                    }
                }
                // Xử lý yêu cầu mới
                else
                {
                    if (string.IsNullOrEmpty(request.UserInput))
                    {
                        return BadRequest(new { response = "Vui lòng nhập câu hỏi!" });
                    }

                    var userInputLower = request.UserInput.ToLower();

                    // Xử lý yêu cầu "Danh sách sản phẩm"
                    if (userInputLower.Contains("danh sách sản phẩm"))
                    {
                        if (context.LastAction == "ShowProductList")
                        {
                            answer = "Bạn đã xem danh sách sản phẩm rồi. Bạn muốn lọc theo danh mục hoặc giá không? [Yes/No]";
                            context.Action = "AskFilterProducts";
                        }
                        else
                        {
                            answer = $"Dưới đây là danh sách sản phẩm:<br>{productList}<br>{instructions}";
                            context.LastAction = "ShowProductList";
                        }
                    }
                    // Xử lý yêu cầu "Danh sách combo"
                    else if (userInputLower.Contains("danh sách combo"))
                    {
                        if (context.LastAction == "ShowComboList")
                        {
                            answer = "Bạn đã xem danh sách combo rồi. Bạn muốn tôi gợi ý combo không? [Yes/No]";
                            context.Action = "AskRecommendCombo";
                        }
                        else
                        {
                            answer = $"Dưới đây là danh sách combo:<br>{comboList}<br>{instructions}";
                            context.LastAction = "ShowComboList";
                        }
                    }
                    // Xử lý yêu cầu "Tư vấn sản phẩm" hoặc "Có món nào ngon không?"
                    else if (userInputLower.Contains("tư vấn") || userInputLower.Contains("món nào ngon"))
                    {
                        var bestSellers = await GetBestSellers();
                        var recommendedProduct = ((List<ProductResponseDTO>)((ObjectResult)bestSellers).Value).FirstOrDefault();
                        if (recommendedProduct != null)
                        {
                            var ctsp = recommendedProduct.Chitietsanphams.FirstOrDefault();
                            answer = $"Tôi khuyên bạn nên thử sản phẩm:<br>" +
                                     $"<strong>Mã:</strong> {recommendedProduct.MaSp}<br>" +
                                     $"<strong>Tên:</strong> {recommendedProduct.TenSanPham}<br>" +
                                     $"<strong>Giá:</strong> {ctsp?.DonGia ?? 0} VNĐ<br>" +
                                     $"<button class='btn btn-primary' onclick='navigateToProduct({recommendedProduct.MaSp})'>Xem Chi Tiết</button><br>" +
                                     $"Bạn có muốn đưa sản phẩm này vào giỏ hàng không? [Yes/No]";
                            context.Action = "AddProductToCart";
                            context.ProductId = recommendedProduct.MaSp.ToString();
                        }
                        else
                        {
                            answer = "Hiện tại không có sản phẩm nào để gợi ý.";
                        }
                    }
                    // Xử lý yêu cầu "Thêm sản phẩm vào giỏ"
                    else if (userInputLower.Contains("thêm") && userInputLower.Contains("vào giỏ"))
                    {
                        // Thử tìm theo mã sản phẩm
                        var maSanPham = ExtractMaSanPham(userInputLower);
                        if (maSanPham != null && products.Any(p => p.MaSp == int.Parse(maSanPham)))
                        {
                            answer = $"Bạn muốn thêm sản phẩm mã {maSanPham} vào giỏ không? [Yes/No]";
                            context.Action = "AddProductToCart";
                            context.ProductId = maSanPham;
                        }
                        // Thử tìm theo tên sản phẩm
                        else
                        {
                            var productName = ExtractProductName(userInputLower);
                            if (!string.IsNullOrEmpty(productName))
                            {
                                var product = products.FirstOrDefault(p => p.TenSanPham.ToLower() == productName);
                                if (product != null)
                                {
                                    answer = $"Bạn muốn thêm sản phẩm {product.TenSanPham} (Mã: {product.MaSp}) vào giỏ không? [Yes/No]";
                                    context.Action = "AddProductToCart";
                                    context.ProductId = product.MaSp.ToString();
                                }
                                else
                                {
                                    answer = $"Không tìm thấy sản phẩm {productName}. Bạn có muốn xem danh sách sản phẩm không? [Yes/No]";
                                    context.Action = "AskProductList";
                                }
                            }
                            // Thử tìm combo
                            else
                            {
                                var comboName = ExtractComboName(userInputLower);
                                if (!string.IsNullOrEmpty(comboName))
                                {
                                    var combo = combos.FirstOrDefault(c => c.TenCombo.ToLower() == comboName);
                                    if (combo != null)
                                    {
                                        var optionsMatch = Regex.Match(userInputLower, @"\[(.*?)\]");
                                        string options = optionsMatch.Success ? optionsMatch.Groups[1].Value : "thường";
                                        answer = $"Bạn muốn thêm combo {combo.TenCombo} (Mã: {combo.MaCombo}) với tùy chọn {options} vào giỏ không? [Yes/No]";
                                        context.Action = "AddComboToCart";
                                        context.ComboId = combo.MaCombo.ToString();
                                        context.ComboOptions = options;
                                    }
                                    else
                                    {
                                        answer = $"Không tìm thấy combo {comboName}. Bạn có muốn xem danh sách combo không? [Yes/No]";
                                        context.Action = "AskComboList";
                                    }
                                }
                                else
                                {
                                    answer = "Không hiểu yêu cầu thêm vào giỏ. Vui lòng nhập lại, ví dụ: 'Thêm Gà Giòn Vui Vẻ vào giỏ' hoặc 'Thêm combo Gà Giòn Hương Vị Cay [cay]'.";
                                }
                            }
                        }
                    }
                    // Xử lý yêu cầu "Thanh toán"
                    else if (userInputLower.Contains("thanh toán"))
                    {
                        if (!isLoggedIn)
                        {
                            answer = "Vui lòng đăng nhập để thanh toán!";
                        }
                        else
                        {
                            var cartItems = await db.Giohangs
                                .Where(g => g.MaKh == int.Parse(maKhachHang))
                                .ToListAsync();
                            if (!cartItems.Any())
                            {
                                answer = "Giỏ hàng của bạn đang trống! Bạn muốn mua sắm không? [Yes/No]";
                                context.Action = "AskContinueShopping";
                            }
                            else
                            {
                                answer = "Bạn có chắc muốn thanh toán giỏ hàng không? [Yes/No]";
                                context.Action = "Checkout";
                            }
                        }
                    }
                    // Gửi yêu cầu tới Gemini API cho các câu hỏi khác
                    else
                    {
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

        private string ExtractProductName(string text)
        {
            var match = Regex.Match(text, @"Thêm\s+(.+?)(?:\s+vào\s+giỏ|giỏ\s+hàng)", RegexOptions.IgnoreCase);
            if (match.Success && !Regex.IsMatch(match.Groups[1].Value, @"^\d+$")) // Không phải số
            {
                return match.Groups[1].Value.Trim().ToLower();
            }
            return null;
        }

        private string ExtractComboName(string text)
        {
            var match = Regex.Match(text, @"Thêm\s+combo\s+(.+?)(?:\s+vào\s+giỏ|giỏ\s+hàng)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var name = match.Groups[1].Value.Trim().ToLower();
                var cleanedName = Regex.Replace(name, @"\s*\(mã\s+\d+\)\s*", "").Trim();
                return cleanedName;
            }
            return null;
        }

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

        [Authorize(Roles = "Customer")]
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

        [Authorize(Roles = "Customer")]
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

                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                {
                    return "Không tìm thấy token xác thực!";
                }

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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

        private async Task<string> ProcessCheckout(HttpClient client, string maKhachHang)
        {
            try
            {
                var khachHang = await db.Khachhangs
                    .FirstOrDefaultAsync(kh => kh.MaKh == int.Parse(maKhachHang));

                if (khachHang == null)
                {
                    return "Không tìm thấy thông tin khách hàng!";
                }

                if (string.IsNullOrEmpty(khachHang.HoTen) || string.IsNullOrEmpty(khachHang.Sdt))
                {
                    return "Vui lòng cập nhật họ tên và số điện thoại trước khi thanh toán!";
                }

                var cartItems = await db.Giohangs
                    .Include(g => g.MaCtspNavigation)
                    .Include(g => g.MaComboNavigation)
                    .ThenInclude(c => c!.Chitietcombos)
                    .ThenInclude(ct => ct.MaSpNavigation)
                    .ThenInclude(sp => sp!.Chitietsanphams)
                    .Where(g => g.MaKh == int.Parse(maKhachHang))
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return "Giỏ hàng của bạn đang trống!";
                }

                decimal tienGoc = 0;
                var cthoadons = new List<OrderDetailRequestDTO>();
                var detailComboRequests = new List<DetailCombo_OrderResquest>();

                foreach (var item in cartItems)
                {
                    if (item.MaCtsp.HasValue)
                    {
                        var ctsp = item.MaCtspNavigation;
                        if (ctsp == null || ctsp.IsDelete || ctsp.SoLuongTon < item.SoLuong)
                        {
                            return $"Sản phẩm {ctsp?.MaCtsp} không khả dụng hoặc không đủ hàng!";
                        }
                        if (!ctsp.DonGia.HasValue)
                        {
                            return $"Sản phẩm {ctsp.MaCtsp} không có giá hợp lệ!";
                        }
                        decimal donGia = ctsp.DonGia.Value;
                        tienGoc += donGia * item.SoLuong;

                        cthoadons.Add(new OrderDetailRequestDTO
                        {
                            MaCtsp = item.MaCtsp.Value,
                            SoLuong = item.SoLuong,
                            DonGia = donGia,
                            GiamGia = 0
                        });
                    }
                    else if (item.MaCombo.HasValue)
                    {
                        var combo = item.MaComboNavigation;
                        if (combo == null || combo.SoLuong < item.SoLuong)
                        {
                            return $"Combo {item.MaCombo} không khả dụng hoặc không đủ hàng!";
                        }

                        decimal donGiaCombo = CalculateComboPrice(combo);
                        tienGoc += donGiaCombo * item.SoLuong;

                        cthoadons.Add(new OrderDetailRequestDTO
                        {
                            MaCombo = item.MaCombo.Value,
                            SoLuong = item.SoLuong,
                            DonGia = donGiaCombo,
                            GiamGia = 0
                        });

                        foreach (var detail in combo.Chitietcombos)
                        {
                            var variant = detail.MaSpNavigation.Chitietsanphams
                                .FirstOrDefault(ct => ct.IsDelete == false && ct.SoLuongTon >= detail.SoLuongSp);
                            if (variant == null)
                            {
                                return $"Không đủ hàng cho sản phẩm trong combo {combo.MaCombo}!";
                            }
                            if (!variant.DonGia.HasValue)
                            {
                                return $"Sản phẩm trong combo {combo.MaCombo} không có giá hợp lệ!";
                            }

                            detailComboRequests.Add(new DetailCombo_OrderResquest
                            {
                                MaCombo = combo.MaCombo,
                                MaCTSp = variant.MaCtsp,
                                SoLuong = detail.SoLuongSp.Value,
                                DonGia = variant.DonGia.Value
                            });
                        }
                    }
                }

                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                {
                    return "Không tìm thấy token xác thực!";
                }

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var payload = new
                {
                    MaKh = int.Parse(maKhachHang),
                    HoTen = khachHang.HoTen,
                    Sdt = khachHang.Sdt,
                    DiaChiNhanHang = khachHang.DiaChi ?? "Không cung cấp",
                    HinhThucTt = "COD",
                    MoTa = "Thanh toán qua chatbot",
                    PhiVanChuyen = 30000m,
                    TienGoc = tienGoc,
                    MaCoupon = (string?)null,
                    GiamGiaCoupon = 0m,
                    DetailCombo_OrderResquests = detailComboRequests,
                    Cthoadons = cthoadons
                };

                _logger.LogInformation("Payload gửi đến Checkout: {Payload}", JsonConvert.SerializeObject(payload));

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

        // Lớp để lưu ngữ cảnh phiên chat
        private class ChatContext
        {
            public string Action { get; set; }
            public string LastAction { get; set; }
            public string ProductId { get; set; }
            public string ComboId { get; set; }
            public string ComboOptions { get; set; }

            public void Clear()
            {
                Action = null;
                ProductId = null;
                ComboId = null;
                ComboOptions = null;
            }
        }

        public class ChatRequestDTO
        {
            public string UserInput { get; set; }
            public string? PreviousAnswer { get; set; }
            public bool? Confirmation { get; set; }
        }

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