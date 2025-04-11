using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class AddMaComboToCthoadon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaCombo",
                table: "CTHOADON",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CTHOADON_MaCombo",
                table: "CTHOADON",
                column: "MaCombo");

            migrationBuilder.AddForeignKey(
                name: "FK_CTHOADON_COMBO_MaCombo",
                table: "CTHOADON",
                column: "MaCombo",
                principalTable: "COMBO",
                principalColumn: "MaCombo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CTHOADON_COMBO_MaCombo",
                table: "CTHOADON");

            migrationBuilder.DropIndex(
                name: "IX_CTHOADON_MaCombo",
                table: "CTHOADON");

            migrationBuilder.DropColumn(
                name: "MaCombo",
                table: "CTHOADON");
        }
    }
}
