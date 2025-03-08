using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLTT.ModelFromDB;

[Table("GoiDichVu")]
public partial class GoiDichVu
{
    [Key]
    public int MaGoiDichVu { get; set; }

    [StringLength(200)]
    public string TenGoi { get; set; } = null!;

    [StringLength(500)]
    public string? MoTa { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Gia { get; set; }

    public int? TaoBoi { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [InverseProperty("MaGoiDichVuNavigation")]
    public virtual ICollection<DanhSachThanhVien> DanhSachThanhViens { get; set; } = new List<DanhSachThanhVien>();

    [InverseProperty("MaGoiDichVuNavigation")]
    public virtual ICollection<NguoiDungGoiDichVu> NguoiDungGoiDichVus { get; set; } = new List<NguoiDungGoiDichVu>();

    [ForeignKey("TaoBoi")]
    [InverseProperty("GoiDichVus")]
    public virtual NguoiDung? TaoBoiNavigation { get; set; }
}
