using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLTT.ModelFromDB;

[Table("GiaoDich")]
public partial class GiaoDich
{
    [Key]
    public int MaGiaoDich { get; set; }

    public int? MaNguoiDung { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SoTien { get; set; }

    [StringLength(50)]
    public string? LoaiGiaoDich { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [InverseProperty("MaGiaoDichNavigation")]
    public virtual ICollection<HoaDonDienTu> HoaDonDienTus { get; set; } = new List<HoaDonDienTu>();

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("GiaoDiches")]
    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}
