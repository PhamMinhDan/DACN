using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLTT.ModelFromDB;

[Table("DanhSachThanhVien")]
public partial class DanhSachThanhVien
{
    [Key]
    public int MaDanhSach { get; set; }

    public int MaAdmin { get; set; }

    public int MaThanhVien { get; set; }
    public DateTime? NgayTao { get; set; }
    public int MaGoiDichVu { get; set; }

    [ForeignKey("MaAdmin")]
    [InverseProperty("DanhSachThanhVienMaAdminNavigations")]
    public virtual NguoiDung MaAdminNavigation { get; set; } = null!;

    [ForeignKey("MaGoiDichVu")]
    [InverseProperty("DanhSachThanhViens")]
    public virtual GoiDichVu MaGoiDichVuNavigation { get; set; } = null!;

    [ForeignKey("MaThanhVien")]
    [InverseProperty("DanhSachThanhVienMaThanhVienNavigations")]
    public virtual NguoiDung MaThanhVienNavigation { get; set; } = null!;
}
