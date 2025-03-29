using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IProduct productRepository;
        private readonly QuanLyCuaHangContext db;

        public HomeController(IProduct productRepository, QuanLyCuaHangContext db)
        {
            this.productRepository = productRepository;
            this.db = db;
        }

        // Lấy tất cả sản phẩm
        [HttpGet("all-products")]
        public async Task<IActionResult> GetAllProducts([FromQuery] string? search, int? filterCategories, string? sort, string? filterPrices, int page = 1)
        {
            var products = await productRepository.GetAll(search, filterCategories, sort, filterPrices);
            page = page >= 1 ? page : 1;
            int pageSize = 10;
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalItems = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return Ok(new
            {
                Data = pagedProducts,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page
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
                        SoLuongSp = ct.SoLuongSp
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
                    .Where(c => c.MaCombo == id && c.IsDelete == false)
                    .Include(c => c.Chitietcombos)
                    .ThenInclude(ct => ct.MaSpNavigation)
                    .ThenInclude(sp => sp.Chitietsanphams)
                    .Select(c => new
                    {
                        Combo = c,
                        Chitietcombos = c.Chitietcombos.Select(ct => new
                        {
                            ct.MaSp,
                            ct.SoLuongSp,
                            MaSpNavigation = ct.MaSpNavigation,
                            DonGia = ct.MaSpNavigation != null && ct.MaSpNavigation.Chitietsanphams.Any()
                                ? ct.MaSpNavigation.Chitietsanphams.FirstOrDefault().DonGia
                                : null,
                            MaDanhMuc = ct.MaSpNavigation != null ? ct.MaSpNavigation.MaDanhMuc : 0
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (comboQuery == null)
                {
                    return NotFound(new { message = $"Không tìm thấy combo với ID {id}." });
                }

                var combo = comboQuery.Combo;
                var chitietcombos = comboQuery.Chitietcombos;

                // Tính số lượng miếng gà (giả sử danh mục "Gà" có MaDanhMuc = 1)
                var chickenCategoryId = 1; // Thay đổi theo danh mục thực tế
                var soLuongMiengGa = chitietcombos
                    .Where(ct => ct.MaDanhMuc == chickenCategoryId)
                    .Sum(ct => ct.SoLuongSp ?? 0);

                var totalPrice = chitietcombos.Any()
                    ? chitietcombos.Sum(ct => (ct.DonGia ?? 0) * (ct.SoLuongSp ?? 0))
                    : 0;

                var comboResponse = new ComboResponseDTO
                {
                    MaCombo = combo.MaCombo,
                    TenCombo = combo.TenCombo,
                    Hinh = combo.Hinh,
                    SoTienGiam = combo.SoTienGiam,
                    PhanTramGiam = combo.PhanTramGiam,
                    SoLuong = combo.SoLuong,
                    MoTa = combo.MoTa,
                    IsDelete = combo.IsDelete,
                    SoLuongMiengGa = soLuongMiengGa, // Thêm trường này
                    TotalPrice = totalPrice,
                    DiscountedPrice = combo.SoTienGiam.HasValue ? totalPrice - combo.SoTienGiam : totalPrice,
                    Chitietcombos = chitietcombos.Select(ct => new DetaisComboResponseDTO
                    {
                        MaSp = ct.MaSp,
                        TenSp = ct.MaSpNavigation != null ? ct.MaSpNavigation.TenSanPham : "Không xác định",
                        SoLuongSp = ct.SoLuongSp
                    }).ToList()
                };

                return Ok(comboResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetComboById: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Lỗi server khi lấy chi tiết combo.", detail = ex.Message });
            }
        }

        // Lấy sản phẩm bán chạy
        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers()
        {
            var bestSellers = await db.Cthoadons
                .GroupBy(ct => ct.MaCtsp)
                .Select(g => new
                {
                    MaCtsp = g.Key,
                    TotalSold = g.Sum(ct => ct.SoLuong)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .Join(db.Chitietsanphams
                    .Include(ct => ct.MaSpNavigation)
                    .Include(ct => ct.Hinhanhs),
                    ct => ct.MaCtsp,
                    sp => sp.MaCtsp,
                    (ct, sp) => new ProductBestSellerDTO
                    {
                        MaCtsp = sp.MaCtsp,
                        MaSp = sp.MaSp,
                        TenSanPham = sp.MaSpNavigation.TenSanPham,
                        KichThuoc = sp.KichThuoc ?? "NO",
                        HuongVi = sp.HuongVi ?? "NO",
                        DonGia = sp.DonGia,
                        TotalSold = ct.TotalSold,
                        Hinh = sp.Hinhanhs.FirstOrDefault() != null ? sp.Hinhanhs.FirstOrDefault().TenHinhAnh : null
                    })
                .ToListAsync();

            return Ok(bestSellers);
        }

        // Lấy chi tiết sản phẩm bán chạy theo ID
        [HttpGet("best-sellers/{id}")]
        public async Task<IActionResult> GetBestSellerById(int id)
        {
            var bestSeller = await db.Chitietsanphams
                .Where(ct => ct.MaCtsp == id && ct.IsDelete == false)
                .Include(ct => ct.MaSpNavigation)
                .Include(ct => ct.Hinhanhs)
                .Select(sp => new ProductBestSellerDTO
                {
                    MaCtsp = sp.MaCtsp,
                    MaSp = sp.MaSp,
                    TenSanPham = sp.MaSpNavigation.TenSanPham,
                    KichThuoc = sp.KichThuoc ?? "NO",
                    HuongVi = sp.HuongVi ?? "NO",
                    DonGia = sp.DonGia,
                    Hinh = sp.Hinhanhs.FirstOrDefault() != null ? sp.Hinhanhs.FirstOrDefault().TenHinhAnh : null
                })
                .FirstOrDefaultAsync();

            if (bestSeller == null)
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm bán chạy với ID {id}." });
            }

            return Ok(bestSeller);
        }

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
    }

    // DTO cho sản phẩm
    public class ProductResponseDTO
    {
        public int MaSp { get; set; }
        public string TenSanPham { get; set; }
        public int? MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public int TongSoLuong { get; set; }
        public string KhoangGia { get; set; }
        public string MoTa { get; set; }
        public bool IsDelete { get; set; }
        public List<DetailProductResponseDTO> Chitietsanphams { get; set; }
    }

    // DTO cho chi tiết sản phẩm
    public class DetailProductResponseDTO
    {
        public int MaCtsp { get; set; }
        public int MaSp { get; set; }
        public string TenSanPham { get; set; }
        public string KichThuoc { get; set; }
        public string HuongVi { get; set; }
        public int? SoLuongTon { get; set; }
        public decimal? DonGia { get; set; }
        public List<ImageProductResponseDTO> Hinhanhs { get; set; }
    }

    // DTO cho hình ảnh sản phẩm
    public class ImageProductResponseDTO
    {
        public int MaHinhAnh { get; set; }
        public string TenHinhAnh { get; set; }
        public int MaCtsp { get; set; }
    }

    // DTO cho sản phẩm bán chạy
    public class ProductBestSellerDTO
    {
        public int MaCtsp { get; set; }
        public int MaSp { get; set; }
        public string TenSanPham { get; set; }
        public string KichThuoc { get; set; }
        public string HuongVi { get; set; }
        public decimal? DonGia { get; set; }
        public int TotalSold { get; set; }
        public string Hinh { get; set; }
    }

    // DTO cho danh mục
    public class CategoryResponseDTO
    {
        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public bool IsDelete { get; set; }
        public string HinhDaiDien { get; set; }
    }
}