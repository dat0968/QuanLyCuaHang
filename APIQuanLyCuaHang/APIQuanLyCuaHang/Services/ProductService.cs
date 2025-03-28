using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.DetailProduct;
using APIQuanLyCuaHang.Repositories.ImageProduct;
using APIQuanLyCuaHang.Repositories.Product;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Services
{
    public class ProductService
    {
        private readonly IProduct ProductRepository;
        private readonly IDetailProduct DetailProductRepository;

        public readonly IimageProduct imageProductRepository;

        private readonly QuanLyCuaHangContext db;

        public ProductService(IProduct ProductRepository, IDetailProduct DetailProductRepository, IimageProduct imageProductRepository,  QuanLyCuaHangContext db)
        {
            this.ProductRepository = ProductRepository;
            this.DetailProductRepository = DetailProductRepository;
            this.imageProductRepository = imageProductRepository;
            this.db = db;
        }

        public async Task AddProduct(ProductCreateRequestDTO productCreateRequestDTO)
        {
            await db.Database.BeginTransactionAsync();
            try
            {
                var NewProduct = new Sanpham
                {
                    MaDanhMuc = productCreateRequestDTO.MaDanhMuc,
                    TenSanPham = productCreateRequestDTO.TenSanPham,
                    MoTa = productCreateRequestDTO.MoTa,
                    IsDelete = productCreateRequestDTO.IsDelete,
                };
                NewProduct = await ProductRepository.AddProduct(NewProduct);
                foreach (var detail in productCreateRequestDTO.DetailProductCreateRequestDTOs)
                {
                    var NewDetailProduct = new Chitietsanpham
                    {
                        MaSp = NewProduct.MaSp,
                        KichThuoc = detail.KichThuoc,
                        HuongVi = detail.HuongVi,
                        SoLuongTon = detail.SoLuongTon,
                        DonGia = detail.DonGia,
                        IsDelete = false,
                    };
                    NewDetailProduct = await DetailProductRepository.AddDetailProduct(NewDetailProduct);
                    if (detail.ImageProductRequestDTOs != null)
                    {
                        foreach (var image in detail.ImageProductRequestDTOs)
                        {
                            var NewImage = new Hinhanh
                            {
                                MaCtsp = NewDetailProduct.MaCtsp,
                                TenHinhAnh = image.TenHinhAnh,
                            };
                            await imageProductRepository.AddImageProduct(NewImage);
                        }
                    }
                }
                await db.Database.CommitTransactionAsync();
            }
            catch(Exception ex)
            {                
                await db.Database.RollbackTransactionAsync();
                throw;
            }
            
        }

        public async Task EditProduct(int MaSp, ProductEditRequestDTO request )
        {
            
            await db.Database.BeginTransactionAsync();
            try
            {
                if (request == null || request.DetailProductEditRequestDTOs == null || !request.DetailProductEditRequestDTOs.Any())
                {
                    throw new ArgumentException("Request data is invalid or empty");
                }
                // Update Product
                var FindProduct = await db.Sanphams.FirstOrDefaultAsync(p => p.MaSp == MaSp);
                if (FindProduct == null)
                {
                    throw new Exception("Product not found");
                }
                FindProduct.MaDanhMuc = request.MaDanhMuc;
                FindProduct.TenSanPham = request.TenSanPham;
                FindProduct.MoTa = request.MoTa;
                FindProduct.IsDelete = request.IsDelete;
                await ProductRepository.EditProduct(FindProduct);

                // Get Product Details current
                var FindDetailProduct = await DetailProductRepository.GetDetailProductByMaSP(MaSp);
                var FindDetailProductID = FindDetailProduct.Select(p => p.MaCtsp).ToList();
                var RequestDetailID = request.DetailProductEditRequestDTOs.Select(p => p.MaCtsp).ToList();

                //Delete variants that are no longer in the request
                var FindDetailToDelete = FindDetailProduct.Where(p => RequestDetailID.Contains(p.MaCtsp) == false);
                foreach(var detail in FindDetailToDelete)
                {
                    await imageProductRepository.DeleteImageProductByMaCtSp(detail.MaCtsp);
                    await DetailProductRepository.DeleteDetailProduct(detail.MaCtsp);
                }

                // Update or add new variants 
                foreach (var detail in request.DetailProductEditRequestDTOs)
                {
                    if (FindDetailProductID.Contains(detail.MaCtsp))
                    {
                        var UpdateDetails = new Chitietsanpham
                        {
                            MaCtsp = detail.MaCtsp,
                            MaSp = FindProduct.MaSp,
                            KichThuoc = detail.KichThuoc,
                            HuongVi = detail.HuongVi,
                            SoLuongTon = detail.SoLuongTon,
                            DonGia = detail.DonGia,
                            IsDelete = false,
                        };
                        await DetailProductRepository.UpdateDetailProduct(UpdateDetails);

                        // Handle ImageProductDetail
                        await imageProductRepository.DeleteImageProductByMaCtSp(detail.MaCtsp);
                        if (detail.ImageProductRequestDTOs != null)
                        {
                            foreach (var image in detail.ImageProductRequestDTOs)
                            {
                                var NewImage = new Hinhanh
                                {
                                    MaCtsp = detail.MaCtsp,
                                    TenHinhAnh = image.TenHinhAnh,
                                };
                                await imageProductRepository.AddImageProduct(NewImage);
                            }
                        }
                    }
                    else
                    {
                        var NewDetailProduct = new Chitietsanpham
                        {
                            MaSp = FindProduct.MaSp,
                            KichThuoc = detail.KichThuoc,
                            HuongVi = detail.HuongVi,
                            DonGia = detail.DonGia,
                            SoLuongTon = detail.SoLuongTon,
                            IsDelete = false,
                        };
                        NewDetailProduct = await DetailProductRepository.AddDetailProduct(NewDetailProduct);
                        if(detail.ImageProductRequestDTOs != null)
                        {
                            foreach (var image in detail.ImageProductRequestDTOs)
                            {
                                var NewImage = new Hinhanh
                                {
                                    MaCtsp = NewDetailProduct.MaCtsp,
                                    TenHinhAnh = image.TenHinhAnh,
                                };
                                await imageProductRepository.AddImageProduct(NewImage);
                            }
                        }
                    }
                }
                await db.Database.CommitTransactionAsync();
            }
            catch(Exception ex)
            {
                await db.Database.RollbackTransactionAsync();
                throw;
            }
            

        }
    }
}
