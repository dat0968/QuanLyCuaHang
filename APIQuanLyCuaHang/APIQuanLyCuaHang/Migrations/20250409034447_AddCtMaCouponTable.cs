using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class AddCtMaCouponTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CHITIETMACOUPON",
                columns: table => new
                {
                    MaCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaKh = table.Column<int>(type: "int", nullable: false),
                    NgaySuDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHITIETMACOUPON", x => new { x.MaKh, x.MaCode });
                    
                    table.ForeignKey(
                        name: "FK_CHITIETMACOUPON_KHACHHANG_MaKh",
                        column: x => x.MaKh,
                        principalTable: "KHACHHANG",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CHITIETMACOUPON_MaCoupons_MaCode",
                        column: x => x.MaCode,
                        principalTable: "MaCoupons",
                        principalColumn: "MaCode",
                        onDelete: ReferentialAction.Cascade);
                });

          

            migrationBuilder.CreateIndex(
                name: "IX_CHITIETMACOUPON_MaCode",
                table: "CHITIETMACOUPON",
                column: "MaCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHITIETMACOUPON");
        }
    }
}
