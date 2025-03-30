using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class nullableFkGioHangTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Xóa khóa ngoại trước khi thay đổi cột
            migrationBuilder.DropForeignKey(
                name: "FK__GIOHANG__MaCTSP__6B24EA82",
                table: "GIOHANG");

            // 2️⃣ Đổi cột MaCTSP thành NULLABLE
            migrationBuilder.AlterColumn<int>(
                name: "MaCTSP",
                table: "GIOHANG",
                type: "int",
                nullable: true,  // ✅ Cho phép NULL
                oldClrType: typeof(int),
                oldType: "int");

            // 3️⃣ Thêm lại khóa ngoại với ON DELETE SET NULL (nếu muốn)
            migrationBuilder.AddForeignKey(
                name: "FK__GIOHANG__MaCTSP__6B24EA82",
                table: "GIOHANG",
                column: "MaCTSP",
                principalTable: "CHITIETSANPHAM",
                principalColumn: "MaCTSP",
                onDelete: ReferentialAction.SetNull); // ✅ Nếu bản ghi bị xóa, đặt NULL
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Xóa FK trước khi đổi cột về NOT NULL
            migrationBuilder.DropForeignKey(
                name: "FK__GIOHANG__MaCTSP__6B24EA82",
                table: "GIOHANG");

            // 2️⃣ Đổi cột MaCTSP về NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "MaCTSP",
                table: "GIOHANG",
                type: "int",
                nullable: false,  // ❌ Bắt buộc có giá trị
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 3️⃣ Thêm lại FK nhưng không có SET NULL (trở về như ban đầu)
            migrationBuilder.AddForeignKey(
                name: "FK__GIOHANG__MaCTSP__6B24EA82",
                table: "GIOHANG",
                column: "MaCTSP",
                principalTable: "CHITIETSANPHAM",
                principalColumn: "MaCTSP",
                onDelete: ReferentialAction.Cascade); // Hoặc ClientSetNull nếu muốn
        }
    }
}
