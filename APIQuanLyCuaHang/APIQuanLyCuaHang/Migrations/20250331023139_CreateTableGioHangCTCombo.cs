using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableGioHangCTCombo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GioHangCTCombos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGioHang = table.Column<int>(type: "int", nullable: false),
                    MaCTSp = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    GioHangId = table.Column<int>(type: "int", nullable: false),
                    ChiTietSanPhamMaCtsp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioHangCTCombos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GioHangCTCombos_CHITIETSANPHAM_ChiTietSanPhamMaCtsp",
                        column: x => x.ChiTietSanPhamMaCtsp,
                        principalTable: "CHITIETSANPHAM",
                        principalColumn: "MaCTSP",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GioHangCTCombos_GIOHANG_GioHangId",
                        column: x => x.GioHangId,
                        principalTable: "GIOHANG",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            
            migrationBuilder.CreateIndex(
                name: "IX_GioHangCTCombos_ChiTietSanPhamMaCtsp",
                table: "GioHangCTCombos",
                column: "ChiTietSanPhamMaCtsp");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangCTCombos_GioHangId",
                table: "GioHangCTCombos",
                column: "GioHangId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GioHangCTCombos");

           
        }
    }
}
