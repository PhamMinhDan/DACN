using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using QLTT.ModelFromDB;
using QLTT.Models;

namespace QLTT.Controllers
{
    public class AccountController : Controller
    {
        private readonly QLTK _context;

        public AccountController(QLTK context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.NguoiDungs
                    .FirstOrDefault(u => (u.Email == model.Username || u.SoDienThoai == model.Username));

                if (user != null && VerifyPassword(model.Password, user.MatKhau, user.Salt))
                {
                    HttpContext.Session.SetString("UserId", user.MaNguoiDung.ToString());
                    HttpContext.Session.SetString("UserName", user.HoTen);
                    HttpContext.Session.SetString("UserRole", user.VaiTro); // Lưu vai trò vào session

                    // Khôi phục PackageId nếu người dùng là AdminGoiDichVu
                    if (user.VaiTro == "AdminGoiDichVu")
                    {
                        var package = _context.DanhSachThanhViens
                            .Where(ds => ds.MaAdmin == user.MaNguoiDung)
                            .OrderByDescending(ds => ds.MaDanhSach)
                            .Select(ds => ds.MaGoiDichVu)
                            .FirstOrDefault();

                        if (package != 0)
                        {
                            HttpContext.Session.SetInt32("PackageId", package);
                        }
                        else
                        {
                            HttpContext.Session.Remove("PackageId");
                        }
                    }
                    else
                    {
                        HttpContext.Session.Remove("PackageId");
                    }

                    return RedirectToAction("Dashboard", "Home");
                }
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
            }
            return View(model);
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email hoặc số điện thoại đã tồn tại (chỉ kiểm tra giá trị không NULL, không giới hạn null)
                if (!string.IsNullOrEmpty(model.EmailOrPhone))
                {
                    bool emailOrPhoneExists = _context.NguoiDungs.Any(u =>
                        (!string.IsNullOrEmpty(u.Email) && u.Email == (model.EmailOrPhone.Contains("@") ? model.EmailOrPhone : null)) ||
                        (!string.IsNullOrEmpty(u.SoDienThoai) && u.SoDienThoai == (!model.EmailOrPhone.Contains("@") ? model.EmailOrPhone : null)));

                    if (emailOrPhoneExists)
                    {
                        ModelState.AddModelError("", "Email hoặc số điện thoại đã được sử dụng.");
                        return View(model);
                    }
                }

                // Tạo salt và hash mật khẩu
                string salt = GenerateSalt();
                string hashedPassword = HashPassword(model.Password, salt);

                var user = new NguoiDung
                {
                    HoTen = model.HoTen,
                    Email = model.EmailOrPhone.Contains("@") ? model.EmailOrPhone : null,
                    SoDienThoai = !model.EmailOrPhone.Contains("@") ? model.EmailOrPhone : null,
                    MatKhau = hashedPassword,
                    Salt = salt,
                    VaiTro = "NguoiDung",
                    NgayTao = DateTime.Now,
                    SoDu = 0m
                };

                try
                {
                    _context.NguoiDungs.Add(user);
                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi đăng ký: {ex.Message}");
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: /Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.NguoiDungs.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Email không tồn tại.");
                    return View(model);
                }

                // Thêm logic gửi email hoặc tạo OTP (tùy thuộc vào yêu cầu)
                return RedirectToAction("ResetPassword");
            }
            return View(model);
        }

        // GET: /Account/Profile
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung.ToString() == userId);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(user);
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chủ chưa đăng nhập
        }

        // Helper methods
        private string GenerateSalt()
        {
            byte[] bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHash, string salt)
        {
            string hashToCheck = HashPassword(enteredPassword, salt);
            return hashToCheck == storedHash;
        }
    }
}