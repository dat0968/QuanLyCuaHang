using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class updateLichSuLamViec_CaKip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "LICHSULAMVIEC",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "GioRa",
                table: "LICHSULAMVIEC",
                type: "time(7)",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "GioVao",
                table: "LICHSULAMVIEC",
                type: "time(7)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiXacNhan",
                table: "LICHSULAMVIEC",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "LICHSULAMVIEC",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Chờ xác nhận");

            migrationBuilder.AddColumn<decimal>(
                name: "HeSoLuong",
                table: "CAKIP",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TenCa",
                table: "CAKIP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LICHSULAMVIEC_NguoiXacNhan",
                table: "LICHSULAMVIEC",
                column: "NguoiXacNhan");

            migrationBuilder.AddForeignKey(
                name: "FK__LICHSULAMV__NguoiXacNhan__12345678",
                table: "LICHSULAMVIEC",
                column: "NguoiXacNhan",
                principalTable: "NHANVIEN",
                principalColumn: "MaNV",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__LICHSULAMV__NguoiXacNhan__12345678",
                table: "LICHSULAMVIEC");

            migrationBuilder.DropIndex(
                name: "IX_LICHSULAMVIEC_NguoiXacNhan",
                table: "LICHSULAMVIEC");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "LICHSULAMVIEC");

            migrationBuilder.DropColumn(
                name: "GioRa",
                table: "LICHSULAMVIEC");

            migrationBuilder.DropColumn(
                name: "GioVao",
                table: "LICHSULAMVIEC");

            migrationBuilder.DropColumn(
                name: "NguoiXacNhan",
                table: "LICHSULAMVIEC");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "LICHSULAMVIEC");

            migrationBuilder.DropColumn(
                name: "HeSoLuong",
                table: "CAKIP");

            migrationBuilder.DropColumn(
                name: "TenCa",
                table: "CAKIP");
        }
    }
}
