using AdminDashbord.Helpers;
using AdminDashbord.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class AdminProfileController : Controller
    {
        private readonly SymphonyLimitedContext _context;
        private readonly IWebHostEnvironment _env;
        private const string ImageFolder = "users";

        public AdminProfileController(SymphonyLimitedContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Profile()
        {
            // Session se current logged-in admin ka ID lo
            var adminIdStr = HttpContext.Session.GetString("AdminId");
            if (!int.TryParse(adminIdStr, out int adminId))
                return RedirectToAction("Login", "Account");

            var admin = await _context.Users.FindAsync(adminId);
            return View(admin);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(int UserId, string FullName, string Email, string? Phone, IFormFile? ProfileImageFile)
        {
            try
            {
                var user = await _context.Users.FindAsync(UserId);
                if (user == null)
                    return Json(new { success = false, message = "Admin account not found." });

                if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email))
                    return Json(new { success = false, message = "Full name and email are required." });

                user.FullName = FullName;
                user.Email = Email;
                user.Phone = Phone;

                if (ProfileImageFile != null && ProfileImageFile.Length > 0)
                {
                    var savedName = await FileUploadHelper.SaveImageAsync(ProfileImageFile, _env, ImageFolder);
                    if (savedName != null)
                    {
                        FileUploadHelper.DeleteImage(user.UserImg, _env, ImageFolder);
                        user.UserImg = savedName;
                    }
                }

                await _context.SaveChangesAsync();

                // Session mein naam update karo taake topbar turant reflect kare
                HttpContext.Session.SetString("AdminName", FullName);
                HttpContext.Session.SetString("AdminEmail", Email);

                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "Email must be unique. Please use a different email." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(int UserId, string CurrentPassword, string NewPassword, string ConfirmNewPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(UserId);
                if (user == null)
                    return Json(new { success = false, message = "Admin account not found." });

                if (user.Password != HashPassword(CurrentPassword ?? ""))
                    return Json(new { success = false, message = "Current password is incorrect." });

                if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 4)
                    return Json(new { success = false, message = "New password must be at least 4 characters." });

                if (NewPassword != ConfirmNewPassword)
                    return Json(new { success = false, message = "New password and confirmation do not match." });

                user.Password = HashPassword(NewPassword);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
