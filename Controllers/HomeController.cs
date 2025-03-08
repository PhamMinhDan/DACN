using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using QLTT.ModelFromDB;

namespace QLTT.Controllers
{
    public class HomeController : Controller
    {
        private readonly QLTK _context;

        public HomeController(QLTK context)
        {
            _context = context;
        }

        // Trang chủ mặc định (cho người chưa đăng nhập hoặc khi truy cập /Home/Index)
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                // Nếu đã đăng nhập, chuyển hướng sang Dashboard
                return RedirectToAction("Dashboard");
            }
            return View(); // Trả về view Trang chủ cho người chưa đăng nhập
        }

        // Dashboard cho người dùng đã đăng nhập
        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userPackages = await _context.DanhSachThanhViens
                .Include(ds => ds.MaGoiDichVuNavigation)
                .Where(ds => ds.MaThanhVien.ToString() == userId)
                .ToListAsync();

            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            return View(userPackages);
        }
    }
}