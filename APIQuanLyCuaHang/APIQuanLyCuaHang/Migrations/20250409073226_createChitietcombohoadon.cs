using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class createChitietcombohoadon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CHITIETCOMBOHOADON",
                columns: table => new
                {
                    MaHd = table.Column<int>(type: "int", nullable: false),
                    MaCombo = table.Column<int>(type: "int", nullable: false),
                    MaCTSp = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHITIETCOMBOHOADON", x => new { x.MaHd, x.MaCombo, x.MaCTSp });
                    table.ForeignKey(
                        name: "FK_CHITIETCOMBOHOADON_CHITIETSANPHAM_MaCTSp",
                        column: x => x.MaCTSp,
                        principalTable: "CHITIETSANPHAM",
                        principalColumn: "MaCTSP",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CHITIETCOMBOHOADON_COMBO_MaCombo",
                        column: x => x.MaCombo,
                        principalTable: "COMBO",
                        principalColumn: "MaCombo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CHITIETCOMBOHOADON_HOADON_MaHd",
                        column: x => x.MaHd,
                        principalTable: "HOADON",
                        principalColumn: "MaHD",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHITIETCOMBOHOADON_MaCombo",
                table: "CHITIETCOMBOHOADON",
                column: "MaCombo");

            migrationBuilder.CreateIndex(
                name: "IX_CHITIETCOMBOHOADON_MaCTSp",
                table: "CHITIETCOMBOHOADON",
                column: "MaCTSp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHITIETCOMBOHOADON");
        }
    }
}
