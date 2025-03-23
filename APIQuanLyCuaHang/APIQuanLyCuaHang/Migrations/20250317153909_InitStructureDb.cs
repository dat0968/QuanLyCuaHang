using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIQuanLyCuaHang.Migrations
{
    /// <inheritdoc />
    public partial class InitStructureDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Nếu tạo CSDL bằng Migrations thì Comment lệnh return dưới
            return;

            migrationBuilder.CreateTable(
                name: "CAKIP",
                columns: table => new
                {
                    MaCaKip = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoNguoiToiDa = table.Column<int>(type: "int", nullable: false),
                    SoNguoiHienTai = table.Column<int>(type: "int", nullable: false),
                    GioBatDau = table.Column<DateTime>(type: "datetime", nullable: false),
                    GioKetThuc = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CAKIP__68C49E4C467ECFEF", x => x.MaCaKip);
                });

            migrationBuilder.CreateTable(
                name: "CHUCVU",
                columns: table => new
                {
                    MaChucVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChucVu = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    LuongTheoGio = table.Column<decimal>(type: "decimal(11,2)", nullable: true),
                    LuongCung = table.Column<decimal>(type: "decimal(11,2)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CHUCVU__D4639533676EA44D", x => x.MaChucVu);
                });

            migrationBuilder.CreateTable(
                name: "COMBO",
                columns: table => new
                {
                    MaCombo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCombo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Hinh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GiaCombo = table.Column<decimal>(type: "decimal(11,2)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__COMBO__C3EDBC78C31DA0C5", x => x.MaCombo);
                });

            migrationBuilder.CreateTable(
                name: "DANHMUC",
                columns: table => new
                {
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DANHMUC__B37508874C124C7E", x => x.MaDanhMuc);
                });

            migrationBuilder.CreateTable(
                name: "KHACHHANG",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CCCD = table.Column<string>(type: "char(12)", unicode: false, fixedLength: true, maxLength: 12, nullable: true),
                    SDT = table.Column<string>(type: "char(11)", unicode: false, fixedLength: true, maxLength: 11, nullable: true),
                    Email = table.Column<string>(type: "char(50)", unicode: false, fixedLength: true, maxLength: 50, nullable: false),
                    TenTaiKhoan = table.Column<string>(type: "char(15)", unicode: false, fixedLength: true, maxLength: 15, nullable: true),
                    MatKhau = table.Column<string>(type: "char(30)", unicode: false, fixedLength: true, maxLength: 30, nullable: true),
                    HinhDaiDien = table.Column<string>(type: "text", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: false),
                    TinhTrang = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true, defaultValue: "Đang hoạt động"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KHACHHAN__2725CF1E85FFC142", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "REFRESHTOKEN",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__REFRESHT__3214EC27AF9EE8AD", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NHANVIEN",
                columns: table => new
                {
                    MaNV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CCCD = table.Column<string>(type: "char(12)", unicode: false, fixedLength: true, maxLength: 12, nullable: true),
                    SDT = table.Column<string>(type: "char(11)", unicode: false, fixedLength: true, maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "char(40)", unicode: false, fixedLength: true, maxLength: 40, nullable: false),
                    NgayVaoLam = table.Column<DateOnly>(type: "date", nullable: false),
                    TenTaiKhoan = table.Column<string>(type: "char(15)", unicode: false, fixedLength: true, maxLength: 15, nullable: true),
                    MatKhau = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    TinhTrang = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true, defaultValue: "Đang hoạt động"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    MaChucVu = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NHANVIEN__2725D70A5E4858B0", x => x.MaNV);
                    table.ForeignKey(
                        name: "FK__NHANVIEN__MaChuc__4222D4EF",
                        column: x => x.MaChucVu,
                        principalTable: "CHUCVU",
                        principalColumn: "MaChucVu");
                });

            migrationBuilder.CreateTable(
                name: "SANPHAM",
                columns: table => new
                {
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false),
                    TenSanPham = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SANPHAM__2725081C40D91290", x => x.MaSP);
                    table.ForeignKey(
                        name: "FK__SANPHAM__IsDelet__48CFD27E",
                        column: x => x.MaDanhMuc,
                        principalTable: "DANHMUC",
                        principalColumn: "MaDanhMuc");
                });

            migrationBuilder.CreateTable(
                name: "HOADON",
                columns: table => new
                {
                    MaHD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaNV = table.Column<int>(type: "int", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: false),
                    BatDauGiao = table.Column<DateTime>(type: "datetime", nullable: true),
                    NgayNhan = table.Column<DateTime>(type: "datetime", nullable: true),
                    DiaChiNhanHang = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime", nullable: true),
                    HinhThucTT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TinhTrang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SDT = table.Column<string>(type: "char(11)", unicode: false, fixedLength: true, maxLength: 11, nullable: false),
                    LyDoHuy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    PhiVanChuyen = table.Column<decimal>(type: "decimal(11,2)", nullable: false),
                    TienGoc = table.Column<decimal>(type: "decimal(11,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HOADON__2725A6E0A656C6A7", x => x.MaHD);
                    table.ForeignKey(
                        name: "FK__HOADON__MaKH__59063A47",
                        column: x => x.MaKH,
                        principalTable: "KHACHHANG",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK__HOADON__MaNV__59FA5E80",
                        column: x => x.MaNV,
                        principalTable: "NHANVIEN",
                        principalColumn: "MaNV");
                });

            migrationBuilder.CreateTable(
                name: "LICHSULAMVIEC",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNV = table.Column<int>(type: "int", nullable: false),
                    MaCaKip = table.Column<int>(type: "int", nullable: false),
                    NgayThangNam = table.Column<DateOnly>(type: "date", nullable: false),
                    SoGioLam = table.Column<double>(type: "float", nullable: false),
                    LyDoNghi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TongLuong = table.Column<decimal>(type: "decimal(11,2)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LICHSULA__3214EC27C0159D37", x => x.ID);
                    table.ForeignKey(
                        name: "FK__LICHSULAMV__MaNV__6477ECF3",
                        column: x => x.MaNV,
                        principalTable: "NHANVIEN",
                        principalColumn: "MaNV");
                    table.ForeignKey(
                        name: "FK__LICHSULAM__MaCaK__656C112C",
                        column: x => x.MaCaKip,
                        principalTable: "CAKIP",
                        principalColumn: "MaCaKip");
                });

            migrationBuilder.CreateTable(
                name: "CHITIETSANPHAM",
                columns: table => new
                {
                    MaCTSP = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    KichThuoc = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    HuongVi = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SoLuongTon = table.Column<int>(type: "int", nullable: true),
                    DonGia = table.Column<decimal>(type: "decimal(11,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CHITIETS__1E4FCECD83115908", x => x.MaCTSP);
                    table.ForeignKey(
                        name: "FK__CHITIETSAN__MaSP__4BAC3F29",
                        column: x => x.MaSP,
                        principalTable: "SANPHAM",
                        principalColumn: "MaSP");
                });

            migrationBuilder.CreateTable(
                name: "CHITIETCOMBO",
                columns: table => new
                {
                    MaCombo = table.Column<int>(type: "int", nullable: false),
                    MaCTSP = table.Column<int>(type: "int", nullable: false),
                    SoLuongSP = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CHITIETC__020940946F0EEEE6", x => new { x.MaCombo, x.MaCTSP });
                    table.ForeignKey(
                        name: "FK__CHITIETCO__MaCTS__5535A963",
                        column: x => x.MaCTSP,
                        principalTable: "CHITIETSANPHAM",
                        principalColumn: "MaCTSP");
                    table.ForeignKey(
                        name: "FK__CHITIETCO__MaCom__5441852A",
                        column: x => x.MaCombo,
                        principalTable: "COMBO",
                        principalColumn: "MaCombo");
                });

            migrationBuilder.CreateTable(
                name: "CTHOADON",
                columns: table => new
                {
                    MaHD = table.Column<int>(type: "int", nullable: false),
                    MaCTSP = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CTHOADON__E6C15A0CD99CD838", x => new { x.MaHD, x.MaCTSP });
                    table.ForeignKey(
                        name: "FK__CTHOADON__MaCTSP__5DCAEF64",
                        column: x => x.MaCTSP,
                        principalTable: "CHITIETSANPHAM",
                        principalColumn: "MaCTSP");
                    table.ForeignKey(
                        name: "FK__CTHOADON__MaHD__5CD6CB2B",
                        column: x => x.MaHD,
                        principalTable: "HOADON",
                        principalColumn: "MaHD");
                });

            migrationBuilder.CreateTable(
                name: "GIOHANG",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaCTSP = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(11,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GIOHANG__3214EC27376B130B", x => x.ID);
                    table.ForeignKey(
                        name: "FK__GIOHANG__MaCTSP__6B24EA82",
                        column: x => x.MaCTSP,
                        principalTable: "CHITIETSANPHAM",
                        principalColumn: "MaCTSP");
                    table.ForeignKey(
                        name: "FK__GIOHANG__MaKH__6A30C649",
                        column: x => x.MaKH,
                        principalTable: "KHACHHANG",
                        principalColumn: "MaKH");
                });

            migrationBuilder.CreateTable(
                name: "HINHANH",
                columns: table => new
                {
                    MaHinhAnh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCTSP = table.Column<int>(type: "int", nullable: false),
                    TenHinhAnh = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HINHANH__A9C37A9BE6F7EFCA", x => x.MaHinhAnh);
                    table.ForeignKey(
                        name: "FK__HINHANH__MaCTSP__4E88ABD4",
                        column: x => x.MaCTSP,
                        principalTable: "CHITIETSANPHAM",
                        principalColumn: "MaCTSP");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHITIETCOMBO_MaCTSP",
                table: "CHITIETCOMBO",
                column: "MaCTSP");

            migrationBuilder.CreateIndex(
                name: "IX_CHITIETSANPHAM_MaSP",
                table: "CHITIETSANPHAM",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_CTHOADON_MaCTSP",
                table: "CTHOADON",
                column: "MaCTSP");

            migrationBuilder.CreateIndex(
                name: "IX_GIOHANG_MaCTSP",
                table: "GIOHANG",
                column: "MaCTSP");

            migrationBuilder.CreateIndex(
                name: "IX_GIOHANG_MaKH",
                table: "GIOHANG",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_HINHANH_MaCTSP",
                table: "HINHANH",
                column: "MaCTSP");

            migrationBuilder.CreateIndex(
                name: "IX_HOADON_MaKH",
                table: "HOADON",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_HOADON_MaNV",
                table: "HOADON",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "IX_LICHSULAMVIEC_MaCaKip",
                table: "LICHSULAMVIEC",
                column: "MaCaKip");

            migrationBuilder.CreateIndex(
                name: "IX_LICHSULAMVIEC_MaNV",
                table: "LICHSULAMVIEC",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "IX_NHANVIEN_MaChucVu",
                table: "NHANVIEN",
                column: "MaChucVu");

            migrationBuilder.CreateIndex(
                name: "IX_SANPHAM_MaDanhMuc",
                table: "SANPHAM",
                column: "MaDanhMuc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHITIETCOMBO");

            migrationBuilder.DropTable(
                name: "CTHOADON");

            migrationBuilder.DropTable(
                name: "GIOHANG");

            migrationBuilder.DropTable(
                name: "HINHANH");

            migrationBuilder.DropTable(
                name: "LICHSULAMVIEC");

            migrationBuilder.DropTable(
                name: "REFRESHTOKEN");

            migrationBuilder.DropTable(
                name: "COMBO");

            migrationBuilder.DropTable(
                name: "HOADON");

            migrationBuilder.DropTable(
                name: "CHITIETSANPHAM");

            migrationBuilder.DropTable(
                name: "CAKIP");

            migrationBuilder.DropTable(
                name: "KHACHHANG");

            migrationBuilder.DropTable(
                name: "NHANVIEN");

            migrationBuilder.DropTable(
                name: "SANPHAM");

            migrationBuilder.DropTable(
                name: "CHUCVU");

            migrationBuilder.DropTable(
                name: "DANHMUC");
        }
    }
}
