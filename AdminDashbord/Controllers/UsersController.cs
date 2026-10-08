using AdminDashbord.Helpers;
using AdminDashbord.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class UsersController : Controller
    {
        private readonly SymphonyLimitedContext _context;
        private readonly IWebHostEnvironment _env;
        private const string ImageFolder = "users";

        public UsersController(SymphonyLimitedContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.UserId)
                .ToListAsync();

            return View(users);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        // POST: /Users/Save
        [HttpPost]
        public async Task<IActionResult> Save(
            int UserId,
            string FullName,
            string Email,
            string? Password,
            string? Phone,
            string Role,
            IFormFile? UserImageFile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email))
                    return Json(new { success = false, message = "Full name and email are required." });

                if (UserId == 0)
                {
                    if (string.IsNullOrWhiteSpace(Password))
                        return Json(new { success = false, message = "Password is required for a new user." });

                    string savedImage = "default-avatar.svg";
                    if (UserImageFile != null && UserImageFile.Length > 0)
                    {
                        var uploaded = await FileUploadHelper.SaveImageAsync(UserImageFile, _env, ImageFolder);
                        if (uploaded != null) savedImage = uploaded;
                    }

                    var user = new User
                    {
                        FullName = FullName,
                        Email = Email,
                        Password = HashPassword(Password),
                        Phone = Phone,
                        Role = string.IsNullOrWhiteSpace(Role) ? "student" : Role,
                        UserImg = savedImage
                    };

                    _context.Users.Add(user);
                }
                else
                {
                    var existing = await _context.Users.FindAsync(UserId);
                    if (existing == null)
                        return Json(new { success = false, message = "User not found." });

                    existing.FullName = FullName;
                    existing.Email = Email;
                    existing.Phone = Phone;
                    existing.Role = string.IsNullOrWhiteSpace(Role) ? "student" : Role;

                    if (!string.IsNullOrWhiteSpace(Password))
                        existing.Password = HashPassword(Password);

                    if (UserImageFile != null && UserImageFile.Length > 0)
                    {
                        var savedName = await FileUploadHelper.SaveImageAsync(UserImageFile, _env, ImageFolder);
                        if (savedName != null)
                        {
                            FileUploadHelper.DeleteImage(existing.UserImg, _env, ImageFolder);
                            existing.UserImg = savedName;
                        }
                    }
                }

                await _context.SaveChangesAsync();
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

        // POST: /Users/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return Json(new { success = false, message = "User not found." });

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                FileUploadHelper.DeleteImage(user.UserImg, _env, ImageFolder);

                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "This user has a linked student profile and cannot be deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
