using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QLTT.ModelFromDB;

namespace QLTT.Controllers
{
    public class TransactionController : Controller
    {
        private readonly QLTK _context;

        public TransactionController(QLTK context)
        {
            _context = context;
        }

        // GET: /Transaction/Deposit
        public IActionResult Deposit()
        {
            return View();
        }

        // POST: /Transaction/Deposit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deposit(decimal amount)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (amount <= 0)
            {
                TempData["Error"] = "Số tiền nạp phải lớn hơn 0.";
                return View();
            }

            var user = _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung.ToString() == userId);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                // Cập nhật số dư
                user.SoDu += amount;

                // Tạo giao dịch
                var transaction = new GiaoDich
                {
                    MaNguoiDung = user.MaNguoiDung,
                    SoTien = amount,
                    LoaiGiaoDich = "NapTien",
                    TrangThai = "ThanhCong",
                    NgayTao = DateTime.Now
                };
                _context.GiaoDiches.Add(transaction);
                _context.SaveChanges(); // Lưu giao dịch để lấy MaGiaoDich

                // Tạo hóa đơn điện tử
                var invoice = new HoaDonDienTu
                {
                    MaGiaoDich = transaction.MaGiaoDich,
                    NgayTao = DateTime.Now,
                    TongTien = amount,
                    TrangThai = "Dathanhtoan" // Giả sử nạp tiền thành công thì hóa đơn đã thanh toán
                };
                _context.HoaDonDienTus.Add(invoice);
                _context.SaveChanges();

                TempData["Success"] = "Nạp tiền thành công!";
                return RedirectToAction("Deposit");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi nạp tiền: {ex.Message}";
                return View();
            }
        }

        // GET: /Transaction/History
        public IActionResult History()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var invoices = _context.HoaDonDienTus
                .Include(h => h.MaGiaoDichNavigation) // Bao gồm dữ liệu từ GiaoDich
                .Where(h => h.MaGiaoDichNavigation.MaNguoiDung.ToString() == userId)
                .OrderByDescending(h => h.NgayTao)
                .ToList();

            return View(invoices);
        }
    }
}