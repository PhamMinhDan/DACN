using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLTT.ModelFromDB;

[Table("NguoiDung")]
public partial class NguoiDung
{
    [Key]
    public int MaNguoiDung { get; set; }

    [StringLength(200)]
    public string HoTen { get; set; } = null!;

    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? SoDienThoai { get; set; }

    [StringLength(255)]
    public string MatKhau { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? SoDu { get; set; }

    [StringLength(50)]
    public string? VaiTro { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column("OTP")]
    [StringLength(10)]
    public string? Otp { get; set; }

    [Column("OTPExpireTime", TypeName = "datetime")]
    public DateTime? OtpexpireTime { get; set; }

    public string? RefreshToken { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TokenExpireTime { get; set; }

    public string? Salt { get; set; }

    [InverseProperty("MaAdminNavigation")]
    public virtual ICollection<DanhSachThanhVien> DanhSachThanhVienMaAdminNavigations { get; set; } = new List<DanhSachThanhVien>();

    [InverseProperty("MaThanhVienNavigation")]
    public virtual ICollection<DanhSachThanhVien> DanhSachThanhVienMaThanhVienNavigations { get; set; } = new List<DanhSachThanhVien>();

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<GiaoDich> GiaoDiches { get; set; } = new List<GiaoDich>();

    [InverseProperty("TaoBoiNavigation")]
    public virtual ICollection<GoiDichVu> GoiDichVus { get; set; } = new List<GoiDichVu>();

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<NguoiDungGoiDichVu> NguoiDungGoiDichVus { get; set; } = new List<NguoiDungGoiDichVu>();

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<NhatKyHeThong> NhatKyHeThongs { get; set; } = new List<NhatKyHeThong>();

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<XacThucNguoiDung> XacThucNguoiDungs { get; set; } = new List<XacThucNguoiDung>();
}
