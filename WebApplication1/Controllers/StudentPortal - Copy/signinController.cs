using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers.StudentPortal
{
    //[Area("StudentPortal")]
    public class signinController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public signinController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> login()
        {
            ViewBag.Branches = await _context.Branch.OrderBy(b => b.BranchName).ToListAsync();
            return View();
        }

        // Called by the Login form on the portal page.
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                TempData["LoginError"] = "Invalid email or password.";
                return RedirectToAction("login");
            }

            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.UserId);

            HttpContext.Session.SetInt32(SessionKeys.UserId, user.UserId);
            HttpContext.Session.SetString(SessionKeys.FullName, user.FullName);
            HttpContext.Session.SetString(SessionKeys.Role, user.Role);
            if (student != null)
            {
                HttpContext.Session.SetInt32(SessionKeys.StudentId, student.StudentId);
            }

            TempData["LoginSuccess"] = $"Welcome back, {user.FullName}!";
            return RedirectToAction("Index", "Home");
        }

        // Called by the Register form on the portal page.
        // Creates a User + linked Student record (this is what unlocks
        // the 4-step journey colour fill and entrance exam apply buttons).
        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string phone, string password, int? branchId)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["RegisterError"] = "Please fill all required fields.";
                return RedirectToAction("login");
            }

            var exists = await _context.Users.AnyAsync(u => u.Email == email);
            if (exists)
            {
                TempData["RegisterError"] = "An account with this email already exists.";
                return RedirectToAction("login");
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Password = password, // NOTE: plain text for demo purposes only — hash this in production.
                Phone = phone,
                UserImg = "default-user.png",
                Role = "student"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            ModelState.Clear();

            var student = new Student
            {
                UserId = user.UserId,
                BranchId = branchId,
                RollNumber = "SL-" + DateTime.Now.Year + "-" + user.UserId.ToString("D4"),
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
            };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32(SessionKeys.UserId, user.UserId);
            HttpContext.Session.SetInt32(SessionKeys.StudentId, student.StudentId);
            HttpContext.Session.SetString(SessionKeys.FullName, user.FullName);
            HttpContext.Session.SetString(SessionKeys.Role, user.Role);

            TempData["LoginSuccess"] = $"Account created! Your roll number is {student.RollNumber}.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
