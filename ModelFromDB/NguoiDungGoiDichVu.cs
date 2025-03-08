using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLTT.ModelFromDB;

[Table("NguoiDungGoiDichVu")]
public partial class NguoiDungGoiDichVu
{
    [Key]
    public int MaNguoiDungGoiDichVu { get; set; }

    public int? MaNguoiDung { get; set; }

    public int? MaGoiDichVu { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayMua { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayHetHan { get; set; }

    [ForeignKey("MaGoiDichVu")]
    [InverseProperty("NguoiDungGoiDichVus")]
    public virtual GoiDichVu? MaGoiDichVuNavigation { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("NguoiDungGoiDichVus")]
    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}
