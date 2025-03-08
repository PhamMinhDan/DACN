using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using QLTT.ModelFromDB;

namespace QLTT.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly QLTK _context;

        public UserManagementController(QLTK context)
        {
            _context = context;
        }

        // GET: /UserManagement/ManageUsers
        public IActionResult ManageUsers(string searchString = "")
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "AdminGoiDichVu")
            {
                return Unauthorized();
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdString);
            var packageId = HttpContext.Session.GetInt32("PackageId");
            if (!packageId.HasValue || packageId == 0)
            {
                TempData["Error"] = "Bạn chưa mua gói dịch vụ nào để quản lý thành viên.";
                return View(new List<NguoiDung>());
            }

            var packageUsers = _context.DanhSachThanhViens
                .Include(d => d.MaThanhVienNavigation)
                .Where(d => d.MaAdmin == userId && d.MaGoiDichVu == packageId.Value)
                .Select(d => d.MaThanhVienNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                packageUsers = packageUsers.Where(u =>
                    u.HoTen.Contains(searchString) ||
                    u.Email.Contains(searchString) ||
                    u.SoDienThoai.Contains(searchString));
            }

            var result = packageUsers.ToList();
            ViewBag.SearchString = searchString;
            ViewBag.PackageId = packageId.Value;
            return View(result);
        }

        // GET: /UserManagement/AddToPackage
        public IActionResult AddToPackage()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "AdminGoiDichVu")
            {
                return Unauthorized();
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdString);
            var packageId = HttpContext.Session.GetInt32("PackageId");
            if (!packageId.HasValue || packageId == 0)
            {
                TempData["Error"] = "Bạn chưa mua gói dịch vụ nào để thêm thành viên.";
                return RedirectToAction("ManageUsers");
            }

            ViewBag.PackageId = packageId.Value;
            return View();
        }

     
        // POST: /UserManagement/SearchUser
        [HttpPost]
        public IActionResult SearchUser(string searchQuery)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "AdminGoiDichVu")
            {
                return Unauthorized();
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            var adminId = int.Parse(userIdString);
            var packageId = HttpContext.Session.GetInt32("PackageId");
            if (!packageId.HasValue || packageId == 0)
            {
                TempData["Error"] = "Bạn chưa mua gói dịch vụ nào để thêm thành viên.";
                return RedirectToAction("ManageUsers");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Tìm người dùng trong bảng NguoiDung dựa trên Email hoặc SoDienThoai
                    var user = _context.NguoiDungs
                        .FirstOrDefault(u => (u.Email.Contains(searchQuery) || u.SoDienThoai.Contains(searchQuery)) && u.MaNguoiDung != adminId);

                    if (user == null)
                    {
                        TempData["Error"] = "Không tìm thấy người dùng phù hợp.";
                        return RedirectToAction("AddToPackage");
                    }

                    // Kiểm tra xem người dùng đã tồn tại trong gói dịch vụ chưa
                    var existing = _context.DanhSachThanhViens
                        .FirstOrDefault(d => d.MaAdmin == adminId && d.MaThanhVien == user.MaNguoiDung && d.MaGoiDichVu == packageId.Value);

                    if (existing != null)
                    {
                        TempData["Error"] = "Thành viên đã được thêm vào gói dịch vụ này.";
                        return RedirectToAction("AddToPackage");
                    }

                    // Thêm người dùng vào DanhSachThanhViens
                    var danhSach = new DanhSachThanhVien
                    {
                        MaAdmin = adminId,
                        MaThanhVien = user.MaNguoiDung,
                        MaGoiDichVu = packageId.Value,
                        NgayTao = DateTime.Now
                    };
                    _context.DanhSachThanhViens.Add(danhSach);

                    // Lưu thay đổi
                    _context.SaveChanges();
                    transaction.Commit();

                    TempData["Success"] = $"Thêm thành viên '{user.HoTen}' vào gói dịch vụ thành công!";
                    return RedirectToAction("ManageUsers");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Error"] = $"Lỗi khi tìm kiếm hoặc thêm thành viên: {ex.Message}";
                    return RedirectToAction("AddToPackage");
                }
            }
        }
            // POST: /UserManagement/AddSelectedUser
            [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSelectedUser(int userId)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "AdminGoiDichVu")
            {
                return Unauthorized();
            }

            var adminIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(adminIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            var adminId = int.Parse(adminIdString);
            var packageId = HttpContext.Session.GetInt32("PackageId");
            if (!packageId.HasValue || packageId == 0)
            {
                TempData["Error"] = "Gói dịch vụ không hợp lệ.";
                return RedirectToAction("ManageUsers");
            }

            var user = _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == userId);
            if (user == null)
            {
                TempData["Error"] = "Người dùng không tồn tại.";
                return RedirectToAction("AddToPackage");
            }

            var existing = _context.DanhSachThanhViens
                .FirstOrDefault(d => d.MaAdmin == adminId && d.MaThanhVien == userId && d.MaGoiDichVu == packageId.Value);

            if (existing != null)
            {
                TempData["Error"] = "Thành viên đã được thêm vào gói dịch vụ này.";
                return RedirectToAction("AddToPackage");
            }

            var danhSach = new DanhSachThanhVien
            {
                MaAdmin = adminId,
                MaThanhVien = userId,
                MaGoiDichVu = packageId.Value
            };
            _context.DanhSachThanhViens.Add(danhSach);
            _context.SaveChanges();

            TempData["Success"] = "Thêm thành viên vào gói dịch vụ thành công!";
            return RedirectToAction("ManageUsers");
        }

        // GET: /UserManagement/DeleteFromPackage
        public IActionResult DeleteFromPackage(int userId, int packageId)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "AdminGoiDichVu")
            {
                return Unauthorized();
            }

            var adminIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(adminIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            var adminId = int.Parse(adminIdString);
            var packageIdFromSession = HttpContext.Session.GetInt32("PackageId");
            if (!packageIdFromSession.HasValue || packageIdFromSession == 0 || packageId != packageIdFromSession.Value)
            {
                TempData["Error"] = "Gói dịch vụ không hợp lệ.";
                return RedirectToAction("ManageUsers");
            }

            var danhSach = _context.DanhSachThanhViens
                .Include(d => d.MaThanhVienNavigation)
                .FirstOrDefault(d => d.MaThanhVien == userId && d.MaAdmin == adminId && d.MaGoiDichVu == packageId);

            if (danhSach == null)
            {
                return NotFound();
            }

            ViewBag.PackageId = packageId; // Gán PackageId vào ViewBag
            return View(danhSach.MaThanhVienNavigation);
        }

        // POST: /UserManagement/DeleteFromPackage
        [HttpPost, ActionName("DeleteFromPackage")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFromPackageConfirmed(int userId, int packageId)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "AdminGoiDichVu")
            {
                return Unauthorized();
            }

            var adminIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(adminIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            var adminId = int.Parse(adminIdString);
            var packageIdFromSession = HttpContext.Session.GetInt32("PackageId");
            if (!packageIdFromSession.HasValue || packageIdFromSession == 0 || packageId != packageIdFromSession.Value)
            {
                TempData["Error"] = "Gói dịch vụ không hợp lệ.";
                return RedirectToAction("ManageUsers");
            }

            var danhSach = _context.DanhSachThanhViens
                .FirstOrDefault(d => d.MaThanhVien == userId && d.MaAdmin == adminId && d.MaGoiDichVu == packageId);

            if (danhSach != null)
            {
                _context.DanhSachThanhViens.Remove(danhSach);
                _context.SaveChanges();
                TempData["Success"] = "Xóa thành viên khỏi gói dịch vụ thành công!";
            }
            return RedirectToAction("ManageUsers");
        }
    }
}