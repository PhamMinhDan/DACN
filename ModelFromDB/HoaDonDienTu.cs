using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLTT.ModelFromDB;

[Table("HoaDonDienTu")]
public partial class HoaDonDienTu
{
    [Key]
    public int MaHoaDon { get; set; }

    public int MaGiaoDich { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TongTien { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaGiaoDich")]
    [InverseProperty("HoaDonDienTus")]
    public virtual GiaoDich MaGiaoDichNavigation { get; set; } = null!;
}
