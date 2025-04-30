using Microsoft.ML.Data;

namespace APIQuanLyCuaHang.DTO
{
    public class PurchaseRating
    {
        [ColumnName("userId")]
        public int UserId { get; set; }

        [ColumnName("productId")]
        public int ProductId { get; set; }

        [ColumnName("Label")]
        public float Rating { get; set; } // Có thể là 1.0 nếu mua, 0.0 nếu không mua
    }
    public class RatingPrediction
    {
        public float Score { get; set; }
    }
}
