using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLTT.ModelFromDB;

public partial class QLTK : DbContext
{
    public QLTK()
    {
    }

    public QLTK(DbContextOptions<QLTK> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<DanhSachThanhVien> DanhSachThanhViens { get; set; }

    public virtual DbSet<GiaoDich> GiaoDiches { get; set; }

    public virtual DbSet<GoiDichVu> GoiDichVus { get; set; }

    public virtual DbSet<HoaDonDienTu> HoaDonDienTus { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NguoiDungGoiDichVu> NguoiDungGoiDichVus { get; set; }

    public virtual DbSet<NhatKyHeThong> NhatKyHeThongs { get; set; }

    public virtual DbSet<XacThucNguoiDung> XacThucNguoiDungs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-MFITI26M\\MSSQLSERVER3;Initial Catalog=QLTK;Persist Security Info=True;User ID=sa;Password=phdan261005@;Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<DanhSachThanhVien>(entity =>
        {
            entity.HasKey(e => e.MaDanhSach).HasName("PK__DanhSach__20D8C84498E6836B");

            entity.HasOne(d => d.MaAdminNavigation).WithMany(p => p.DanhSachThanhVienMaAdminNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DanhSachThanhVien_MaAdmin");

            entity.HasOne(d => d.MaGoiDichVuNavigation).WithMany(p => p.DanhSachThanhViens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DanhSachThanhVien_MaGoiDichVu");

            entity.HasOne(d => d.MaThanhVienNavigation).WithMany(p => p.DanhSachThanhVienMaThanhVienNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DanhSachThanhVien_MaThanhVien");
        });

        modelBuilder.Entity<GiaoDich>(entity =>
        {
            entity.HasKey(e => e.MaGiaoDich).HasName("PK__GiaoDich__0A2A24EB9723C233");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.GiaoDiches)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__GiaoDich__MaNguo__4222D4EF");
        });

        modelBuilder.Entity<GoiDichVu>(entity =>
        {
            entity.HasKey(e => e.MaGoiDichVu).HasName("PK__GoiDichV__D08A071AC4368CFC");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TaoBoiNavigation).WithMany(p => p.GoiDichVus).HasConstraintName("FK__GoiDichVu__TaoBo__47DBAE45");
        });

        modelBuilder.Entity<HoaDonDienTu>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDonDi__835ED13B3FE34C49");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaGiaoDichNavigation).WithMany(p => p.HoaDonDienTus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDonDienTu_MaGiaoDich");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D762A26AFD9B");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SoDu).HasDefaultValue(0m);
        });

        modelBuilder.Entity<NguoiDungGoiDichVu>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDungGoiDichVu).HasName("PK__NguoiDun__99344A1E454AFEBD");

            entity.Property(e => e.NgayMua).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaGoiDichVuNavigation).WithMany(p => p.NguoiDungGoiDichVus)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__NguoiDung__MaGoi__4CA06362");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.NguoiDungGoiDichVus)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__NguoiDung__MaNgu__4BAC3F29");
        });

        modelBuilder.Entity<NhatKyHeThong>(entity =>
        {
            entity.HasKey(e => e.MaNhatKy).HasName("PK__NhatKyHe__E42EF42E8D4C7059");

            entity.Property(e => e.ThoiGian).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.NhatKyHeThongs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_NhatKyHeThong_NguoiDung");
        });

        modelBuilder.Entity<XacThucNguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaXacThuc).HasName("PK__XacThucN__71196B3CE05EB741");

            entity.Property(e => e.DaSuDung).HasDefaultValue(false);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.XacThucNguoiDungs)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__XacThucNg__MaNgu__3E52440B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
