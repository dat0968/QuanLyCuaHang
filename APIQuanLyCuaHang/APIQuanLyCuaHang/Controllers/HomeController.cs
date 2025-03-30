
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
    }
}