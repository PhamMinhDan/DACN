using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLTT.ModelFromDB;

[Table("XacThucNguoiDung")]
public partial class XacThucNguoiDung
{
    [Key]
    public int MaXacThuc { get; set; }

    public int? MaNguoiDung { get; set; }

    [Column("MaOTP")]
    [StringLength(10)]
    public string MaOtp { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ThoiGianHetHan { get; set; }

    public bool? DaSuDung { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("XacThucNguoiDungs")]
    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}
