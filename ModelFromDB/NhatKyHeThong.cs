using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLTT.ModelFromDB;

[Table("NhatKyHeThong")]
public partial class NhatKyHeThong
{
    [Key]
    public int MaNhatKy { get; set; }

    public int? MaNguoiDung { get; set; }

    [StringLength(500)]
    public string HanhDong { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGian { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("NhatKyHeThongs")]
    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}
