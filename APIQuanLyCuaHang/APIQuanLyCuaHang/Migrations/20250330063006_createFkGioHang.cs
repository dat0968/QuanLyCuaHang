using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    public partial class createFkGioHang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột MaComboNavigation vào bảng GIOHANG nếu chưa có (nullable)
            migrationBuilder.AddColumn<int>(
                name: "MaComboNavigation",
                table: "GIOHANG",
                nullable: true);  // Cho phép null

            // Thêm khóa ngoại từ MaComboNavigation trong GIOHANG tới MaCombo trong COMBO
            migrationBuilder.AddForeignKey(
                name: "FK_GIOHANG_COMBO_MaComboNavigationMaCombo",
                table: "GIOHANG",
                column: "MaComboNavigation",  // Cột MaComboNavigation trong bảng GIOHANG
                principalTable: "COMBO",     // Bảng COMBO
                principalColumn: "MaCombo",  // Cột tham chiếu MaCombo trong bảng COMBO
                onDelete: ReferentialAction.SetNull); // Xử lý khi xóa: SetNull (cột này sẽ nhận giá trị NULL nếu bản ghi trong COMBO bị xóa)
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa khóa ngoại khi rollback migration
            migrationBuilder.DropForeignKey(
                name: "FK_GIOHANG_COMBO_MaComboNavigationMaCombo",
                table: "GIOHANG");

            // Xóa cột MaComboNavigation khi rollback migration
            migrationBuilder.DropColumn(
                name: "MaComboNavigation",
                table: "GIOHANG");
        }
    }
}
