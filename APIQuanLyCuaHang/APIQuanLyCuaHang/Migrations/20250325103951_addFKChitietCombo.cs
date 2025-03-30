using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class addFKChitietCombo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột MaSp với giá trị mặc định tạm thời
            migrationBuilder.AddColumn<int>(
                name: "MaSp",
                table: "CHITIETCOMBO",
                type: "int",
                nullable: false,
                defaultValue: 1); // Giá trị tạm thời, sẽ được cập nhật sau

            // Xóa các bản ghi trùng lặp MaCombo (giữ bản ghi đầu tiên)
            migrationBuilder.Sql(
                @"WITH Duplicates AS (
                    SELECT MaCombo, ROW_NUMBER() OVER (PARTITION BY MaCombo ORDER BY MaCombo) AS RowNum
                    FROM CHITIETCOMBO
                  )
                  DELETE FROM CHITIETCOMBO
                  WHERE MaCombo IN (
                      SELECT MaCombo
                      FROM Duplicates
                      WHERE RowNum > 1
                  );");

            // Cập nhật MaSp với giá trị hợp lệ từ SANPHAM (lấy MaSP nhỏ nhất)
            migrationBuilder.Sql(
                @"UPDATE CHITIETCOMBO 
                  SET MaSp = (SELECT TOP 1 MaSP FROM SANPHAM WHERE MaSP > 0 ORDER BY MaSP)
                  WHERE EXISTS (SELECT 1 FROM SANPHAM WHERE MaSP > 0);");

            // Thêm khóa chính composite gồm MaCombo và MaSp
            migrationBuilder.AddPrimaryKey(
                name: "PK__CHITIETC__020940946F0EEEE6",
                table: "CHITIETCOMBO",
                columns: new[] { "MaCombo", "MaSp" });

            // Thêm khóa ngoại cho MaSp
            migrationBuilder.AddForeignKey(
                name: "FK__CHITIETCO__MaSP__5535A963",
                table: "CHITIETCOMBO",
                column: "MaSp",
                principalTable: "SANPHAM",
                principalColumn: "MaSP",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa khóa ngoại của MaSp
            migrationBuilder.DropForeignKey(
                name: "FK__CHITIETCO__MaSP__5535A963",
                table: "CHITIETCOMBO");

            // Xóa khóa chính composite
            migrationBuilder.DropPrimaryKey(
                name: "PK__CHITIETC__020940946F0EEEE6",
                table: "CHITIETCOMBO");

            // Xóa cột MaSp
            migrationBuilder.DropColumn(
                name: "MaSp",
                table: "CHITIETCOMBO");
        }
    }
}