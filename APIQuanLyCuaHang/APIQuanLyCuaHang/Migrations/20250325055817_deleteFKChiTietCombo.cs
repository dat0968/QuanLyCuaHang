using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class deleteFKChiTietCombo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__CHITIETCO__MaCTS__5535A963",
                table: "CHITIETCOMBO");

            migrationBuilder.DropPrimaryKey(
                name: "PK__CHITIETC__020940946F0EEEE6",
                table: "CHITIETCOMBO");

        

            migrationBuilder.DropColumn(
                name: "MaCTSP",
                table: "CHITIETCOMBO");

         
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK__CHITIETC__020940946F0EEEE6",
                table: "CHITIETCOMBO");

            migrationBuilder.AddColumn<int>(
                name: "MaCTSP",
                table: "CHITIETCOMBO",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK__CHITIETC__020940946F0EEEE6",
                table: "CHITIETCOMBO",
                columns: new[] { "MaCombo", "MaCTSP" });

           

     
        }
    }
}
