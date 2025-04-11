using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class MakeMaCtspNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Bước 1: Xóa khóa chính hiện tại
            migrationBuilder.DropPrimaryKey(
                name: "PK__CTHOADON__E6C15A0CD99CD838",
                table: "CTHOADON");

            // Bước 2: Thêm cột Id tự tăng làm khóa chính mới
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CTHOADON",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"); // Tự tăng bắt đầu từ 1, bước nhảy 1

            // Bước 3: Thêm khóa chính mới trên cột Id
            migrationBuilder.AddPrimaryKey(
                name: "PK_CTHOADON",
                table: "CTHOADON",
                column: "Id");

            // Bước 4: Thay đổi cột MaCtsp để cho phép NULL
            migrationBuilder.AlterColumn<int>(
                name: "MaCtsp",
                table: "CTHOADON",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Bước 1: Xử lý dữ liệu: Đặt giá trị mặc định cho các dòng có MaCtsp là NULL
            migrationBuilder.Sql(
                @"
                UPDATE CTHOADON
                SET MaCtsp = 0 -- Hoặc một giá trị hợp lệ trong bảng Chitietsanpham
                WHERE MaCtsp IS NULL;
                "
            );

            // Bước 2: Khôi phục cột MaCtsp về NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "MaCtsp",
                table: "CTHOADON",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Bước 3: Xóa khóa chính mới
            migrationBuilder.DropPrimaryKey(
                name: "PK_CTHOADON",
                table: "CTHOADON");

            // Bước 4: Xóa cột Id
            migrationBuilder.DropColumn(
                name: "Id",
                table: "CTHOADON");

            // Bước 5: Thêm lại khóa chính cũ
            migrationBuilder.AddPrimaryKey(
                name: "PK__CTHOADON__E6C15A0CD99CD838",
                table: "CTHOADON",
                columns: new[] { "MaHd", "MaCtsp" });
        }
    }
}
