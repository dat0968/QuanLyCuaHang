using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace APIQuanLyCuaHang.Services
{
    public class MLRecommendationSystem
    {
        private readonly QuanLyCuaHangContext db;
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private bool _isModelTrained;
        public MLRecommendationSystem(QuanLyCuaHangContext db)
        {
            //TrainModel().GetAwaiter().GetResult();
            _mlContext = new MLContext();
            this.db = db;
            _isModelTrained = false;
        }
        private async Task TrainModel()
        {
            try
            {

                // Tách truy vấn để kiểm tra từng bước
                
                var cthoadons = await db.Cthoadons
                    .AsNoTracking()
                    .Include(p => p.MaHdNavigation)
                    .ToListAsync();

                // Kiểm tra dữ liệu hợp lệ
                var validCthoadons = cthoadons
                    .Where(p => p.MaCtsp.HasValue && p.MaHdNavigation != null && p.MaHdNavigation.MaKh.HasValue)
                    .ToList();
                var purchases = validCthoadons
                    .Select(p => new PurchaseRating
                    {
                        UserId = p.MaHdNavigation.MaKh.Value,
                        ProductId = p.MaCtsp.Value,
                        Rating = p.MaHdNavigation.TinhTrang.ToLower() == "đã thanh toán" ? 1.0f : 0.0f // giá trị 1.0f cho tất cả các giao dịch đã hoàn tất, 0.0f ngược lại
                    })
                    .ToList();
                // Huấn luyện mô hình
                var data = _mlContext.Data.LoadFromEnumerable(purchases);
                // Xây dựng pipeline: Ánh xạ UserId và ProductId thành key type

                // Sử dụng MapValuteToKey chuyển đổi userId và productId sang kiểu numeric key - một kiểu được chấp nhận trong recommendation algorithms
                var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: "userId",
                    inputColumnName: "userId")
                    .Append(_mlContext.Transforms.Conversion.MapValueToKey(
                        outputColumnName: "productId",
                        inputColumnName: "productId"))
                    .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                        new MatrixFactorizationTrainer.Options // MatrixFactorizationTrainer là thuật toán training hệ thống recommendation
                        {
                            MatrixColumnIndexColumnName = "userId",
                            MatrixRowIndexColumnName = "productId",
                            LabelColumnName = "Label",
                            NumberOfIterations = 20, // Số vòng lặp tối ưu hóa mô hình
                            ApproximationRank = 5 // Số lượng mô hình tiềm ẩn mà mô hình học cho mỗi cặp userId-productId   
                        }));
                Console.WriteLine("=============== Training the model ===============");
                _model = pipeline.Fit(data);
                _isModelTrained = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi huấn luyện mô hình: {ex.Message}");
                throw;
            }
        }
        public async Task<List<ProductResponseDTO>> Recommend(int? userId, int numberOfRecommendations = 6)
        {          
            if (!_isModelTrained)
            {
                Console.WriteLine("Mô hình chưa được huấn luyện. Huấn luyện ngay bây giờ...");
                await TrainModel();
            }

            if (_model == null)
            {
                Console.WriteLine("Mô hình chưa được huấn luyện thành công. Trả về danh sách rỗng.");
                return new List<ProductResponseDTO>();
            }
            // Bước 1: Lấy danh sách sản phẩm mà userId đã mua
            var userPurchases = await db.Cthoadons
                .AsNoTracking()
                .Include(p => p.MaHdNavigation)
                .Where(p => p.MaHdNavigation.MaKh == userId && p.MaCtsp.HasValue)
                .Select(p => p.MaCtsp.Value)
                .ToListAsync();

            var allProducts = await db.Cthoadons
                .AsNoTracking()
                .Where(p => p.MaCtsp.HasValue)
                .Select(p => p.MaCtsp.Value)
                .Distinct()
                .ToListAsync();

            var productsToRecommend = allProducts.Except(userPurchases).ToList();

            // Bước 2: Sử dụng mô hình Matrix Factorization để dự đoán điểm số cho các sản phẩm chưa được userId mua
            var predictions = new List<(int ProductId, float Score)>();
            // Tạo một prediction engine để dự đoán giá trị cho các cặp userid-productid, PurchaseRating là data đầu vào, RatingPrediction là dữ liệu đầu ra
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<PurchaseRating, RatingPrediction>(_model);

            foreach (var productId in productsToRecommend)
            {
                var prediction = predictionEngine.Predict(new PurchaseRating
                {
                    UserId = userId.Value,
                    ProductId = productId
                });
                Console.WriteLine($"UserId: {userId.Value}, ProductId: {productId}, Score: {prediction.Score}");
                predictions.Add((productId, prediction.Score));
            }
            // Bước 3: Lấy danh sách sản phẩm mà các khách hàng liên quan đã mua
            var idProductRecommend = predictions
                .OrderByDescending(p => p.Score)
                .Take(numberOfRecommendations)
                .ToList();

            var ListProductRecommend = new List<ProductResponseDTO>();
            if(idProductRecommend.Count > 0)
            {
                foreach(var product in idProductRecommend)
                {
                    var detailproducts = await db.Chitietsanphams.AsNoTracking().FirstOrDefaultAsync(p => p.MaCtsp == product.ProductId);
                    var findproduct = await db.Sanphams.AsNoTracking()
                        .Where(p => p.IsDelete == false)
                        .Include(p => p.MaDanhMucNavigation)
                        .Include(p => p.Chitietsanphams)
                        .ThenInclude(p => p.Hinhanhs)
                        .FirstOrDefaultAsync(p => p.MaSp == detailproducts.MaSp);
                    var checkListProductRecommend = ListProductRecommend.FirstOrDefault(p => p.MaSp == findproduct.MaSp);
                    if (checkListProductRecommend != null)
                    {
                        continue;
                    }
                    ListProductRecommend.Add(new ProductResponseDTO
                    {
                        MaSp = findproduct.MaSp,
                        MaDanhMuc = findproduct.MaDanhMuc,
                        TenDanhMuc = findproduct.MaDanhMucNavigation.TenDanhMuc,
                        TenSanPham = findproduct.TenSanPham,
                        TongSoLuong = (int)findproduct.Chitietsanphams.Where(p => p.IsDelete == false).Sum(p => p.SoLuongTon),
                        KhoangGia = findproduct.Chitietsanphams.Where(p => p.IsDelete == false).Any()
                    ? (findproduct.Chitietsanphams.Where(p => p.IsDelete == false).Min(p => p.DonGia) == findproduct.Chitietsanphams.Where(p => p.IsDelete == false).Max(p => p.DonGia)
                        ? $"{findproduct.Chitietsanphams.Where(p => p.IsDelete == false).Min(p => p.DonGia)} VNĐ"
                        : $"{findproduct.Chitietsanphams.Where(p => p.IsDelete == false).Min(p => p.DonGia)} VNĐ - {findproduct.Chitietsanphams.Where(p => p.IsDelete == false).Max(p => p.DonGia)} VNĐ")
                    : "Chưa có giá",
                        MoTa = findproduct.MoTa,
                        IsDelete = findproduct.IsDelete,
                        Chitietsanphams = findproduct.Chitietsanphams.Where(p => p.IsDelete == false).Select(details => new DetailProductResponseDTO
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
                    });
                }
            }
            return ListProductRecommend;
        }
    }
}
