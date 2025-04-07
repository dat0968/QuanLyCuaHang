using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class ThemTruongHinhAnh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "NHANVIEN",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "NHANVIEN");

            
        }
    }
}
