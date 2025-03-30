using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;
namespace APIQuanLyCuaHang.Repositories.Product
{
    public class Product : IProduct
    {
        private readonly QuanLyCuaHangContext db;

        public Product(QuanLyCuaHangContext db)
        {
            this.db = db;
        }
        async Task IProduct.CancelProduct(int id)
        {
            try
            {
                var FindProduct = await db.Sanphams.FindAsync(id);
                if (FindProduct != null)
                {
                    FindProduct.IsDelete = true;
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        async Task<Sanpham> IProduct.AddProduct(Sanpham product)
        {
            try
            {
                db.Sanphams.Add(product);
                await db.SaveChangesAsync();
                return product;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        async Task IProduct.EditProduct(Sanpham product)
        {
            try
            {
                db.Sanphams.Update(product);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            { 
                throw;
            }
        }

        async Task<List<ProductResponseDTO>> IProduct.GetAll(string? search, int? filterCatories, string? sort, string? filterPrices)
        {
            var GetListProduct = await db.Sanphams
                .AsNoTracking()
                .Where(p => p.IsDelete == false)
                .Include(p => p.MaDanhMucNavigation)
                .Include(p => p.Chitietsanphams)
                .ThenInclude(e => e.Hinhanhs)
                .Select(product => new ProductResponseDTO
                {
                    MaSp = product.MaSp,
                    TenSanPham = product.TenSanPham,
                    MaDanhMuc = product.MaDanhMuc,
                    TenDanhMuc = product.MaDanhMucNavigation != null ? product.MaDanhMucNavigation.TenDanhMuc : "Không có danh mục",
                    TongSoLuong = (int)product.Chitietsanphams.Where(p => p.IsDelete == false).Sum(p => p.SoLuongTon),
                    KhoangGia = product.Chitietsanphams.Where(p => p.IsDelete == false).Any()
                    ? (product.Chitietsanphams.Where(p => p.IsDelete == false).Min(p => p.DonGia) == product.Chitietsanphams.Where(p => p.IsDelete == false).Max(p => p.DonGia)
                        ? $"{product.Chitietsanphams.Where(p => p.IsDelete == false).Min(p => p.DonGia)} VNĐ"
                        : $"{product.Chitietsanphams.Where(p => p.IsDelete == false).Min(p => p.DonGia)} VNĐ - {product.Chitietsanphams.Where(p => p.IsDelete == false).Max(p => p.DonGia)} VNĐ")
                    : "Chưa có giá",
                    MoTa = product.MoTa,
                    IsDelete = product.IsDelete,
                    Chitietsanphams = product.Chitietsanphams.Where(p => p.IsDelete == false).Select(details => new DetailProductResponseDTO
                    {
                        MaCtsp = details.MaCtsp,
                        MaSp = details.MaSp,
                        TenSanPham = details.MaSpNavigation.TenSanPham,
                        KichThuoc = string.IsNullOrEmpty(details.KichThuoc) == true ? "" : details.KichThuoc,
                        HuongVi = string.IsNullOrEmpty(details.HuongVi) == true ? "" : details.HuongVi,
                        SoLuongTon = details.SoLuongTon,
                        DonGia = details.DonGia,
                        Hinhanhs = details.Hinhanhs
                        .Select(image => new ImageProductResponseDTO
                        {
                            MaHinhAnh = image.MaHinhAnh,
                            TenHinhAnh = image.TenHinhAnh,
                            MaCtsp = image.MaCtsp,
                        })
                        .ToList(),
                    }).ToList(),
                })
                .ToListAsync();

            if(!string.IsNullOrEmpty(search))
            {
                GetListProduct = GetListProduct.Where(p => p.MaSp.ToString().Contains(search) || p.TenSanPham.ToLower().Contains(search.ToLower())).ToList();
            }

            if (filterCatories.HasValue)
            {
                GetListProduct = GetListProduct.Where(p => p.MaDanhMuc == filterCatories.Value).ToList();
            }
            switch (sort)
            {
                case "asc":
                    GetListProduct = GetListProduct.OrderBy(p =>
                    {
                        if (!p.Chitietsanphams.Any())
                        {
                            return 0;
                        }
                        return p.Chitietsanphams.Min(p => p.DonGia);
                    }).ToList();
                    break;
                case "des":
                    GetListProduct = GetListProduct.OrderByDescending(p =>
                    {
                        if (!p.Chitietsanphams.Any())
                        {
                            return 0;
                        }
                        return p.Chitietsanphams.Max(p => p.DonGia);
                    }).ToList();
                    break;
                default:
                    break;
            }

            switch (filterPrices)
            {
                case "0 VNĐ - 10.000 VNĐ":
                    GetListProduct = GetListProduct
                        .Where(p => p.Chitietsanphams.Any(ct => ct.DonGia >= 0 && ct.DonGia <= 10000))
                        .ToList();
                    break;

                case "10.000 VNĐ - 30.000 VNĐ":
                    GetListProduct = GetListProduct
                        .Where(p => p.Chitietsanphams.Any(ct => ct.DonGia >= 10000 && ct.DonGia <= 30000))
                        .ToList();
                    break;

                case "30.000 VNĐ - 50.000 VNĐ":
                    GetListProduct = GetListProduct
                        .Where(p => p.Chitietsanphams.Any(ct => ct.DonGia >= 30000 && ct.DonGia <= 50000))
                        .ToList();
                    break;

                case "50.000 VNĐ trở lên":
                    GetListProduct = GetListProduct
                        .Where(p => p.Chitietsanphams.Any(ct => ct.DonGia >= 50000))
                        .ToList();
                    break;

                default:
                    break;
            }

            return GetListProduct;
        }

        async Task<ProductResponseDTO> IProduct.GetById(int id)
        {
            var ProductVM = await db.Sanphams
            .AsNoTracking()
            .Where(p => p.MaSp == id)
            .Include(p => p.MaDanhMucNavigation)
            .Include(p => p.Chitietsanphams)
            .ThenInclude(e => e.Hinhanhs)
            .Select(product => new ProductResponseDTO
            {
                MaSp = product.MaSp,
                TenSanPham = product.TenSanPham,
                MaDanhMuc = product.MaDanhMuc,
                TenDanhMuc = product.MaDanhMucNavigation.TenDanhMuc,
                MoTa = product.MoTa,
                IsDelete = product.IsDelete,
                Chitietsanphams = product.Chitietsanphams.Where(p => p.IsDelete == false).Select(details => new DetailProductResponseDTO
                {
                    MaCtsp = details.MaCtsp,
                    MaSp = details.MaSp,
                    TenSanPham = details.MaSpNavigation.TenSanPham,
                    KichThuoc = details.KichThuoc ?? null,
                    HuongVi = details.HuongVi ?? null,
                    SoLuongTon = details.SoLuongTon,
                    DonGia = details.DonGia,
                    Hinhanhs = details.Hinhanhs
                    .Select(image => new ImageProductResponseDTO
                    {
                        MaHinhAnh = image.MaHinhAnh,
                        TenHinhAnh = image.TenHinhAnh,
                        MaCtsp = image.MaCtsp,
                    })
                    .ToList(),
                })
                .ToList(),
            })
            .FirstOrDefaultAsync();
            if(ProductVM == null)
            {
                throw new Exception($"Product with MaSp {id} not found.");
            }
            return ProductVM;
        }
    }
}
