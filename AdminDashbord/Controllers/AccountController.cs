using AdminDashbord.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AdminDashbord.Controllers
{
    public class AccountController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public AccountController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            // Agar pehle se logged in hai to dashboard par bhejo
            if (HttpContext.Session.GetString("AdminId") != null)
                return RedirectToAction("dashbord", "Dashbord");

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return Json(new { success = false, message = "Email and password are required." });

            var hashedPassword = HashPassword(password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == hashedPassword && u.Role == "Admin");

            if (user == null)
                return Json(new { success = false, message = "Invalid email or password. Please try again." });

            // Session mein admin info save karo
            HttpContext.Session.SetString("AdminId", user.UserId.ToString());
            HttpContext.Session.SetString("AdminName", user.FullName);
            HttpContext.Session.SetString("AdminEmail", user.Email);

            return Json(new { success = true });
        }

        // POST: /Account/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}
