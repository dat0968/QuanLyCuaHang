using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class RenameFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GioHangCTCombos_CHITIETSANPHAM_ChiTietSanPhamMaCtsp",
                table: "GioHangCTCombos");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHangCTCombos_GIOHANG_GioHangId",
                table: "GioHangCTCombos");

            migrationBuilder.RenameColumn(
                name: "GioHangId",
                table: "GioHangCTCombos",
                newName: "MaGioHangNavigationId");

            migrationBuilder.RenameColumn(
                name: "ChiTietSanPhamMaCtsp",
                table: "GioHangCTCombos",
                newName: "MaCTSpNavigationMaCtsp");

            migrationBuilder.RenameIndex(
                name: "IX_GioHangCTCombos_GioHangId",
                table: "GioHangCTCombos",
                newName: "IX_GioHangCTCombos_MaGioHangNavigationId");

            migrationBuilder.RenameIndex(
                name: "IX_GioHangCTCombos_ChiTietSanPhamMaCtsp",
                table: "GioHangCTCombos",
                newName: "IX_GioHangCTCombos_MaCTSpNavigationMaCtsp");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GioHangCTCombos_CHITIETSANPHAM_MaCTSpNavigationMaCtsp",
                table: "GioHangCTCombos");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHangCTCombos_GIOHANG_MaGioHangNavigationId",
                table: "GioHangCTCombos");

            migrationBuilder.RenameColumn(
                name: "MaGioHangNavigationId",
                table: "GioHangCTCombos",
                newName: "GioHangId");

            migrationBuilder.RenameColumn(
                name: "MaCTSpNavigationMaCtsp",
                table: "GioHangCTCombos",
                newName: "ChiTietSanPhamMaCtsp");

            migrationBuilder.RenameIndex(
                name: "IX_GioHangCTCombos_MaGioHangNavigationId",
                table: "GioHangCTCombos",
                newName: "IX_GioHangCTCombos_GioHangId");

            migrationBuilder.RenameIndex(
                name: "IX_GioHangCTCombos_MaCTSpNavigationMaCtsp",
                table: "GioHangCTCombos",
                newName: "IX_GioHangCTCombos_ChiTietSanPhamMaCtsp");

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangCTCombos_CHITIETSANPHAM_ChiTietSanPhamMaCtsp",
                table: "GioHangCTCombos",
                column: "ChiTietSanPhamMaCtsp",
                principalTable: "CHITIETSANPHAM",
                principalColumn: "MaCTSP",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangCTCombos_GIOHANG_GioHangId",
                table: "GioHangCTCombos",
                column: "GioHangId",
                principalTable: "GIOHANG",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
