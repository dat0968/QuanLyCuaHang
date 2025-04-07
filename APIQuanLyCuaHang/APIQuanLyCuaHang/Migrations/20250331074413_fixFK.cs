using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class fixFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GioHangCTCombos_CHITIETSANPHAM_MaCTSpNavigationMaCtsp",
                table: "GioHangCTCombos");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHangCTCombos_GIOHANG_MaGioHangNavigationId",
                table: "GioHangCTCombos");

            migrationBuilder.DropIndex(
                name: "IX_GioHangCTCombos_MaCTSpNavigationMaCtsp",
                table: "GioHangCTCombos");

            migrationBuilder.DropIndex(
                name: "IX_GioHangCTCombos_MaGioHangNavigationId",
                table: "GioHangCTCombos");

            migrationBuilder.DropColumn(
                name: "MaCTSpNavigationMaCtsp",
                table: "GioHangCTCombos");

            migrationBuilder.DropColumn(
                name: "MaGioHangNavigationId",
                table: "GioHangCTCombos");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangCTCombos_MaCTSp",
                table: "GioHangCTCombos",
                column: "MaCTSp");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangCTCombos_MaGioHang",
                table: "GioHangCTCombos",
                column: "MaGioHang");

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangCTCombos_CHITIETSANPHAM_MaCTSp",
                table: "GioHangCTCombos",
                column: "MaCTSp",
                principalTable: "CHITIETSANPHAM",
                principalColumn: "MaCTSP",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangCTCombos_GIOHANG_MaGioHang",
                table: "GioHangCTCombos",
                column: "MaGioHang",
                principalTable: "GIOHANG",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GioHangCTCombos_CHITIETSANPHAM_MaCTSp",
                table: "GioHangCTCombos");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHangCTCombos_GIOHANG_MaGioHang",
                table: "GioHangCTCombos");

            migrationBuilder.DropIndex(
                name: "IX_GioHangCTCombos_MaCTSp",
                table: "GioHangCTCombos");

            migrationBuilder.DropIndex(
                name: "IX_GioHangCTCombos_MaGioHang",
                table: "GioHangCTCombos");

            migrationBuilder.AddColumn<int>(
                name: "MaCTSpNavigationMaCtsp",
                table: "GioHangCTCombos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaGioHangNavigationId",
                table: "GioHangCTCombos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GioHangCTCombos_MaCTSpNavigationMaCtsp",
                table: "GioHangCTCombos",
                column: "MaCTSpNavigationMaCtsp");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangCTCombos_MaGioHangNavigationId",
                table: "GioHangCTCombos",
                column: "MaGioHangNavigationId");

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangCTCombos_CHITIETSANPHAM_MaCTSpNavigationMaCtsp",
                table: "GioHangCTCombos",
                column: "MaCTSpNavigationMaCtsp",
                principalTable: "CHITIETSANPHAM",
                principalColumn: "MaCTSP",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangCTCombos_GIOHANG_MaGioHangNavigationId",
                table: "GioHangCTCombos",
                column: "MaGioHangNavigationId",
                principalTable: "GIOHANG",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
