using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Models;

public partial class QuanLyCuaHangContext : DbContext
{
    public QuanLyCuaHangContext()
    {
    }

    public QuanLyCuaHangContext(DbContextOptions<QuanLyCuaHangContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cakip> Cakips { get; set; }

    public virtual DbSet<Chitietcombo> Chitietcombos { get; set; }

    public virtual DbSet<Chitietsanpham> Chitietsanphams { get; set; }

    public virtual DbSet<Chucvu> Chucvus { get; set; }

    public virtual DbSet<Combo> Combos { get; set; }

    public virtual DbSet<Cthoadon> Cthoadons { get; set; }

    public virtual DbSet<Danhmuc> Danhmucs { get; set; }

    public virtual DbSet<Giohang> Giohangs { get; set; }

    public virtual DbSet<Hinhanh> Hinhanhs { get; set; }

    public virtual DbSet<Hoadon> Hoadons { get; set; }

    public virtual DbSet<Khachhang> Khachhangs { get; set; }

    public virtual DbSet<Lichsulamviec> Lichsulamviecs { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    public virtual DbSet<Refreshtoken> Refreshtokens { get; set; }

    public virtual DbSet<Sanpham> Sanphams { get; set; }
    public virtual DbSet<Sanpham> Bans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ban>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("BAN");

            entity.Property(e => e.TinhTrang).HasMaxLength(60);
        });
        modelBuilder.Entity<Cakip>(entity =>
        {
            entity.HasKey(e => e.MaCaKip).HasName("PK__CAKIP__68C49E4C467ECFEF");

            entity.ToTable("CAKIP");

            entity.Property(e => e.GioBatDau).HasColumnType("datetime");
            entity.Property(e => e.GioKetThuc).HasColumnType("datetime");
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
        });

        modelBuilder.Entity<Chitietcombo>(entity =>
        {
            entity.HasKey(e => new { e.MaCombo, e.MaSp }).HasName("PK__CHITIETC__020940946F0EEEE6");

            entity.ToTable("CHITIETCOMBO");

            entity.Property(e => e.SoLuongSp).HasColumnName("SoLuongSP");

            entity.HasOne(d => d.MaComboNavigation).WithMany(p => p.Chitietcombos)
                .HasForeignKey(d => d.MaCombo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHITIETCO__MaCom__5441852A");

            entity.HasOne(d => d.MaSpNavigation)
            .WithMany(p => p.Chitietcombos)
            .HasForeignKey(d => d.MaSp)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__CHITIETCO__MaSP__5535A963");
        });

        modelBuilder.Entity<Chitietsanpham>(entity =>
        {
            entity.HasKey(e => e.MaCtsp).HasName("PK__CHITIETS__1E4FCECD83115908");

            entity.ToTable("CHITIETSANPHAM");

            entity.Property(e => e.MaCtsp).HasColumnName("MaCTSP");
            entity.Property(e => e.DonGia).HasColumnType("decimal(11, 2)");
            entity.Property(e => e.HuongVi).HasMaxLength(30);
            entity.Property(e => e.KichThuoc).HasMaxLength(30);
            entity.Property(e => e.MaSp).HasColumnName("MaSP");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.Chitietsanphams)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHITIETSAN__MaSP__4BAC3F29");
        });

        modelBuilder.Entity<Chucvu>(entity =>
        {
            entity.HasKey(e => e.MaChucVu).HasName("PK__CHUCVU__D4639533676EA44D");

            entity.ToTable("CHUCVU");

            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.LuongCung).HasColumnType("decimal(11, 2)");
            entity.Property(e => e.LuongTheoGio).HasColumnType("decimal(11, 2)");
            entity.Property(e => e.TenChucVu).HasMaxLength(40);
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.MaCombo).HasName("PK__COMBO__C3EDBC78C31DA0C5");

            entity.ToTable("COMBO");

            entity.Property(e => e.Hinh).HasMaxLength(200);
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenCombo).HasMaxLength(100);
        });

        modelBuilder.Entity<Cthoadon>(entity =>
        {
            entity.HasKey(e => new { e.MaHd, e.MaCtsp }).HasName("PK__CTHOADON__E6C15A0CD99CD838");

            entity.ToTable("CTHOADON");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaCtsp).HasColumnName("MaCTSP");

            entity.HasOne(d => d.MaCtspNavigation).WithMany(p => p.Cthoadons)
                .HasForeignKey(d => d.MaCtsp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHOADON__MaCTSP__5DCAEF64");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.Cthoadons)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHOADON__MaHD__5CD6CB2B");
        });

        modelBuilder.Entity<Danhmuc>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PK__DANHMUC__B37508874C124C7E");

            entity.ToTable("DANHMUC");

            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(40);
        });

        modelBuilder.Entity<Giohang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GIOHANG__3214EC27376B130B");

            entity.ToTable("GIOHANG");

            entity.Property(e => e.Id)
                .HasColumnName("ID");
            entity.Property(e => e.DonGia).HasColumnType("decimal(11, 2)");
            entity.Property(e => e.MaCtsp).HasColumnName("MaCTSP");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");

            entity.HasOne(d => d.MaCtspNavigation).WithMany(p => p.Giohangs)
                .HasForeignKey(d => d.MaCtsp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GIOHANG__MaCTSP__6B24EA82");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Giohangs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GIOHANG__MaKH__6A30C649");
        });

        modelBuilder.Entity<Hinhanh>(entity =>
        {
            entity.HasKey(e => e.MaHinhAnh).HasName("PK__HINHANH__A9C37A9BE6F7EFCA");

            entity.ToTable("HINHANH");

            entity.Property(e => e.MaCtsp).HasColumnName("MaCTSP");
            entity.Property(e => e.TenHinhAnh).HasColumnType("text");

            entity.HasOne(d => d.MaCtspNavigation).WithMany(p => p.Hinhanhs)
                .HasForeignKey(d => d.MaCtsp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HINHANH__MaCTSP__4E88ABD4");
        });

        modelBuilder.Entity<Hoadon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HOADON__2725A6E0A656C6A7");

            entity.ToTable("HOADON");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.BatDauGiao).HasColumnType("datetime");
            entity.Property(e => e.DiaChiNhanHang).HasMaxLength(200);
            entity.Property(e => e.HinhThucTt)
                .HasMaxLength(50)
                .HasColumnName("HinhThucTT");
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.LyDoHuy).HasMaxLength(500);
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.NgayNhan).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayThanhToan).HasColumnType("datetime");
            entity.Property(e => e.PhiVanChuyen).HasColumnType("decimal(11, 2)");
            entity.Property(e => e.Sdt)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDT");
            entity.Property(e => e.TienGoc).HasColumnType("decimal(11, 2)");
            entity.Property(e => e.TinhTrang).HasMaxLength(50);

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Hoadons)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HOADON__MaKH__59063A47");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Hoadons)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK__HOADON__MaNV__59FA5E80");
        });

        modelBuilder.Entity<Khachhang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KHACHHAN__2725CF1E85FFC142");

            entity.ToTable("KHACHHANG");

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CCCD");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GioiTinh).HasMaxLength(5);
            entity.Property(e => e.HinhDaiDien).HasColumnType("text");
            entity.Property(e => e.HoTen).HasMaxLength(40);
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.Sdt)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDT");
            entity.Property(e => e.TenTaiKhoan)
                .HasMaxLength(15)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TinhTrang)
                .HasMaxLength(25)
                .HasDefaultValue("Đang hoạt động");
        });

        modelBuilder.Entity<Lichsulamviec>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LICHSULA__3214EC27C0159D37");

            entity.ToTable("LICHSULAMVIEC");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.LyDoNghi).HasMaxLength(500);
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.TongLuong).HasColumnType("decimal(11, 2)");

            entity.HasOne(d => d.MaCaKipNavigation).WithMany(p => p.Lichsulamviecs)
                .HasForeignKey(d => d.MaCaKip)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LICHSULAM__MaCaK__656C112C");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Lichsulamviecs)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LICHSULAMV__MaNV__6477ECF3");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NHANVIEN__2725D70A5E4858B0");

            entity.ToTable("NHANVIEN");

            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CCCD");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.Email)
                .HasMaxLength(40)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GioiTinh).HasMaxLength(5);
            entity.Property(e => e.HoTen).HasMaxLength(40);
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Sdt)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDT");
            entity.Property(e => e.TenTaiKhoan)
                .HasMaxLength(15)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TinhTrang)
                .HasMaxLength(25)
                .HasDefaultValue("Đang hoạt động");

            entity.HasOne(d => d.MaChucVuNavigation).WithMany(p => p.Nhanviens)
                .HasForeignKey(d => d.MaChucVu)
                .HasConstraintName("FK__NHANVIEN__MaChuc__4222D4EF");
        });

        modelBuilder.Entity<Refreshtoken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__REFRESHT__3214EC27AF9EE8AD");

            entity.ToTable("REFRESHTOKEN");

            entity.Property(e => e.Id)
                .HasColumnName("ID");
            entity.Property(e => e.ExpiredAt).HasColumnType("datetime");
            entity.Property(e => e.IssuedAt).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(500);
        });

            modelBuilder.Entity<Sanpham>(entity =>
            {
                entity.HasKey(e => e.MaSp).HasName("PK__SANPHAM__2725081C40D91290");

                entity.ToTable("SANPHAM");

                entity.Property(e => e.MaSp)
                    .HasColumnName("MaSP");
                entity.Property(e => e.IsDelete).HasDefaultValue(false);
                entity.Property(e => e.MoTa).HasMaxLength(500);
                entity.Property(e => e.TenSanPham).HasMaxLength(100);

                entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.Sanphams)
                    .HasForeignKey(d => d.MaDanhMuc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SANPHAM__IsDelet__48CFD27E");
            });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
