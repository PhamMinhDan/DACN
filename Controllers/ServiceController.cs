using Microsoft.AspNetCore.Mvc;
using QLTT.ModelFromDB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace QLTT.Controllers
{
    public class ServiceController : Controller
    {
        private readonly QLTK _context;

        public ServiceController(QLTK context)
        {
            _context = context;
        }

        // GET: /Service/Packages
        public IActionResult Packages()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var purchasedPackages = _context.DanhSachThanhViens
                .Where(p => p.MaAdmin.ToString() == userId)
                .Select(p => p.MaGoiDichVu)
                .ToList();

            var availablePackages = _context.GoiDichVus
                .Where(p => !purchasedPackages.Contains(p.MaGoiDichVu))
                .ToList();

            return View(availablePackages);
        }

        // POST: /Service/BuyPackage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuyPackage(int packageId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung.ToString() == userId);
            var package = _context.GoiDichVus.FirstOrDefault(p => p.MaGoiDichVu == packageId);

            if (user == null || package == null)
            {
                return NotFound();
            }

            if (user.SoDu < package.Gia)
            {
                TempData["Error"] = "Số dư không đủ để mua gói dịch vụ này.";
                return RedirectToAction("Packages");
            }

            try
            {
                // Cập nhật số dư
                user.SoDu -= package.Gia;
                user.VaiTro = "AdminGoiDichVu";
                HttpContext.Session.SetString("UserRole", "AdminGoiDichVu");
                HttpContext.Session.SetInt32("PackageId", packageId);

                // Tạo giao dịch
                var transaction = new GiaoDich
                {
                    MaNguoiDung = user.MaNguoiDung,
                    SoTien = package.Gia,
                    LoaiGiaoDich = "MuaGoi",
                    TrangThai = "ThanhCong",
                    NgayTao = DateTime.Now
                };
                _context.GiaoDiches.Add(transaction);

                // Lưu giao dịch để lấy MaGiaoDich
                _context.SaveChanges();

                // Tạo hóa đơn điện tử với MaGiaoDich đã được tạo
                var invoice = new HoaDonDienTu
                {
                    MaGiaoDich = transaction.MaGiaoDich, // MaGiaoDich đã có giá trị hợp lệ
                    NgayTao = DateTime.Now,
                    TongTien = package.Gia,
                    TrangThai = "Dathanhtoan"
                };
                _context.HoaDonDienTus.Add(invoice);

                // Tạo bản ghi DanhSachThanhVien
                var danhSach = new DanhSachThanhVien
                {
                    MaAdmin = user.MaNguoiDung,
                    MaThanhVien = user.MaNguoiDung,
                    MaGoiDichVu = packageId,
                    NgayTao = DateTime.Now
                };
                _context.DanhSachThanhViens.Add(danhSach);

                // Lưu các thay đổi còn lại
                _context.SaveChanges();

                TempData["Success"] = "Mua gói dịch vụ thành công! Bạn đã được nâng cấp vai trò Admin.";
                return RedirectToAction("Packages");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi mua gói: {ex.Message}. Inner Exception: {ex.InnerException?.Message}";
                return RedirectToAction("Packages");
            }
        }
    }
}