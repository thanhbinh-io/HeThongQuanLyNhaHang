using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Models;

namespace NhaHangThanhThao.Data;

public partial class NhaHangThanhThaoContext : DbContext
{
    public NhaHangThanhThaoContext(DbContextOptions<NhaHangThanhThaoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BanChoi> BanChois { get; set; }

    public virtual DbSet<ChiTietDonDatBan> ChiTietDonDatBans { get; set; }

    public virtual DbSet<ChiTietDonGoiMon> ChiTietDonGoiMons { get; set; }

    public virtual DbSet<DanhMucMon> DanhMucMons { get; set; }

    public virtual DbSet<DonDatBan> DonDatBans { get; set; }

    public virtual DbSet<DonGoiMon> DonGoiMons { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<HoaDonChiTiet> HoaDonChiTiets { get; set; }

    public virtual DbSet<MonAn> MonAns { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<ThamSo> ThamSos { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BanChoi>(entity =>
        {
            entity.HasKey(e => e.MaBan).HasName("PK__BanChoi__3520ED6C1059939D");

            entity.ToTable("BanChoi");

            entity.HasIndex(e => e.Qrtoken, "UQ__BanChoi__7CC967E509B56F0F").IsUnique();

            entity.Property(e => e.MaBan)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KhuVuc).HasMaxLength(50);
            entity.Property(e => e.Qrtoken)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("QRToken");
            entity.Property(e => e.TenBan).HasMaxLength(100);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Trong");
        });

        modelBuilder.Entity<ChiTietDonDatBan>(entity =>
        {
            entity.HasKey(e => new { e.MaDonDatBan, e.MaBan });

            entity.ToTable("ChiTietDonDatBan");

            entity.Property(e => e.MaDonDatBan)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.MaBan)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.TienCoc).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaBanNavigation).WithMany(p => p.ChiTietDonDatBans)
                .HasForeignKey(d => d.MaBan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDonDatBan_BanChoi");

            entity.HasOne(d => d.MaDonDatBanNavigation).WithMany(p => p.ChiTietDonDatBans)
                .HasForeignKey(d => d.MaDonDatBan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDonDatBan_DonDatBan");
        });

        modelBuilder.Entity<ChiTietDonGoiMon>(entity =>
        {
            entity.HasKey(e => e.MaChiTietDonGoiMon).HasName("PK__ChiTietD__9F85C1BD2609F03E");

            entity.ToTable("ChiTietDonGoiMon");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.MaDonGoiMon)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.MaMon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ThanhTien)
                .HasComputedColumnSql("([SoLuong]*[DonGia])", true)
                .HasColumnType("decimal(29, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Moi");

            entity.HasOne(d => d.MaDonGoiMonNavigation).WithMany(p => p.ChiTietDonGoiMons)
                .HasForeignKey(d => d.MaDonGoiMon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDonGoiMon_DonGoiMon");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.ChiTietDonGoiMons)
                .HasForeignKey(d => d.MaMon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDonGoiMon_MonAn");
        });

        modelBuilder.Entity<DanhMucMon>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PK__DanhMucM__B375088760020E47");

            entity.ToTable("DanhMucMon");

            entity.HasIndex(e => e.TenDanhMuc, "UQ__DanhMucM__650CAE4E26D5A112").IsUnique();

            entity.Property(e => e.MaDanhMuc)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
        });

        modelBuilder.Entity<DonDatBan>(entity =>
        {
            entity.HasKey(e => e.MaDonDatBan).HasName("PK__DonDatBa__5AA6E33CC159B704");

            entity.ToTable("DonDatBan");

            entity.Property(e => e.MaDonDatBan)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.NgayLap).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TenKhachHang).HasMaxLength(150);

            entity.HasOne(d => d.MaNguoiLapNavigation).WithMany(p => p.DonDatBans)
                .HasForeignKey(d => d.MaNguoiLap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonDatBan_NguoiDung");
        });

        modelBuilder.Entity<DonGoiMon>(entity =>
        {
            entity.HasKey(e => e.MaDonGoiMon).HasName("PK__DonGoiMo__A3F67122507DADB9");

            entity.ToTable("DonGoiMon");

            entity.HasIndex(e => e.MaBan, "UX_DonGoiMon_MotDonDangPhucVuMoiBan")
                .IsUnique()
                .HasFilter("([TrangThai]=N'DangPhucVu')");

            entity.Property(e => e.MaDonGoiMon)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.MaBan)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaDonDatBan)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.NguonTao)
                .HasMaxLength(30)
                .HasDefaultValue("KhachQR");
            entity.Property(e => e.ThoiGianTao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("DangPhucVu");

            entity.HasOne(d => d.MaBanNavigation).WithMany(p => p.DonGoiMons)
                .HasForeignKey(d => d.MaBan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonGoiMon_BanChoi");

            entity.HasOne(d => d.MaDonDatBanNavigation).WithMany(p => p.DonGoiMons)
                .HasForeignKey(d => d.MaDonDatBan)
                .HasConstraintName("FK_DonGoiMon_DonDatBan");

            entity.HasOne(d => d.MaNguoiTaoNavigation).WithMany(p => p.DonGoiMons)
                .HasForeignKey(d => d.MaNguoiTao)
                .HasConstraintName("FK_DonGoiMon_NguoiDung");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__835ED13BEAAE6E68");

            entity.ToTable("HoaDon");

            entity.HasIndex(e => e.MaDonGoiMon, "UQ__HoaDon__A3F67123E676ACF5").IsUnique();

            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.GioRa).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.MaDonGoiMon)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PhuThu).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TienCocKhauTru).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongThanhToan)
                .HasComputedColumnSql("(([TongTienMon]+[PhuThu])-[TienCocKhauTru])", true)
                .HasColumnType("decimal(20, 2)");
            entity.Property(e => e.TongTienMon).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaDonGoiMonNavigation).WithOne(p => p.HoaDon)
                .HasForeignKey<HoaDon>(d => d.MaDonGoiMon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_DonGoiMon");
        });

        modelBuilder.Entity<HoaDonChiTiet>(entity =>
        {
            entity.HasKey(e => new { e.MaHoaDon, e.MaMon });

            entity.ToTable("HoaDonChiTiet");

            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.MaMon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TenMon).HasMaxLength(150);
            entity.Property(e => e.ThanhTien)
                .HasComputedColumnSql("([SoLuong]*[DonGia])", true)
                .HasColumnType("decimal(29, 2)");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.HoaDonChiTiets)
                .HasForeignKey(d => d.MaHoaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDonChiTiet_HoaDon");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.HoaDonChiTiets)
                .HasForeignKey(d => d.MaMon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDonChiTiet_MonAn");
        });

        modelBuilder.Entity<MonAn>(entity =>
        {
            entity.HasKey(e => e.MaMon).HasName("PK__MonAn__3A5B29A8E5C014CC");

            entity.ToTable("MonAn");

            entity.Property(e => e.MaMon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DonViTinh)
                .HasMaxLength(50)
                .HasDefaultValue("Phần");
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.MaDanhMuc)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MoTa).HasMaxLength(1000);
            entity.Property(e => e.TenMon).HasMaxLength(150);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("DangKinhDoanh");

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.MonAns)
                .HasForeignKey(d => d.MaDanhMuc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MonAn_DanhMucMon");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D762BD0F0612");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.SoDienThoai, "UQ__NguoiDun__0389B7BDF61435D2").IsUnique();

            entity.HasIndex(e => e.TenDangNhap, "UQ__NguoiDun__55F68FC0A0B333D0").IsUnique();

            entity.Property(e => e.HoTen).HasMaxLength(150);
            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("HoatDong");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NguoiDung_VaiTro");
        });

        modelBuilder.Entity<ThamSo>(entity =>
        {
            entity.HasKey(e => e.TenThamSo).HasName("PK__ThamSo__A49B1359C6A2D5A3");

            entity.ToTable("ThamSo");

            entity.Property(e => e.TenThamSo).HasMaxLength(100);
            entity.Property(e => e.GiaTri).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MoTa).HasMaxLength(255);
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__C24C41CF0A9BEFC7");

            entity.ToTable("VaiTro");

            entity.HasIndex(e => e.TenVaiTro, "UQ__VaiTro__1DA558142C3AC514").IsUnique();

            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
