using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class addFkMacouponforHoaDon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiamGiaCoupon",
                table: "HOADON");
        
            migrationBuilder.AddColumn<string>(
                name: "MaCoupon",
                table: "HOADON",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HOADON_MaCoupon",
                table: "HOADON",
                column: "MaCoupon");

            migrationBuilder.AddForeignKey(
                name: "FK_HOADON_MaCoupons_MaCoupon",
                table: "HOADON",
                column: "MaCoupon",
                principalTable: "MaCoupons",
                principalColumn: "MaCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HOADON_MaCoupons_MaCoupon",
                table: "HOADON");

            migrationBuilder.DropIndex(
                name: "IX_HOADON_MaCoupon",
                table: "HOADON");

            migrationBuilder.DropColumn(
                name: "MaCoupon",
                table: "HOADON");
        }
    }
}
