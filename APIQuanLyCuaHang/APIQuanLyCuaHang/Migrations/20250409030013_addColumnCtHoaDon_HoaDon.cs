using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class addColumnCtHoaDon_HoaDon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<decimal>(
                name: "GiamGiaCoupon",
                table: "HOADON",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DonGia",
                table: "CTHOADON",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GiamGia",
                table: "CTHOADON",
                type: "decimal(18,2)",
                nullable: true);
            migrationBuilder.Sql("UPDATE CTHOADON SET Dongia = 50000");
            migrationBuilder.Sql(@"
            UPDATE HOADON
            SET TienGoc = COALESCE((
                SELECT SUM(
                    CASE 
                        WHEN (ct.DonGia * ct.SoLuong - COALESCE(ct.GiamGia, 0)) < 0 
                        THEN 0 
                        ELSE (ct.DonGia * ct.SoLuong - COALESCE(ct.GiamGia, 0)) 
                    END
                )
                FROM CTHOADON ct
                WHERE ct.MaHD = HoaDon.MaHD
            ), 0)
            WHERE EXISTS (
                SELECT 1
                FROM CTHOADON ct
                WHERE ct.MaHD = HoaDon.MaHD
            )");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiamGiaCoupon",
                table: "HOADON");

            migrationBuilder.DropColumn(
                name: "DonGia",
                table: "CTHOADON");

            migrationBuilder.DropColumn(
                name: "GiamGia",
                table: "CTHOADON");

            migrationBuilder.Sql("UPDATE HOADON SET TienGoc = 0");
        }
    }
}
