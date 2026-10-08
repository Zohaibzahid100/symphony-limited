using AdminDashbord.Models;
using AdminDashbord.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class StudentsController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public StudentsController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        // GET: /Students
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Branch)
                .OrderByDescending(s => s.StudentId)
                .ToListAsync();

            ViewBag.Users = await _context.Users
                .OrderBy(u => u.FullName)
                .ToListAsync();

            ViewBag.Branches = await _context.Branches
                .OrderBy(b => b.BranchName)
                .ToListAsync();

            return View(students);
        }

        // POST: /Students/Save
        // Used for both Add (StudentId == 0) and Edit (StudentId > 0)
        [HttpPost]
        public async Task<IActionResult> Save(Student model)
        {
            try
            {
                if (model.UserId == 0)
                    return Json(new { success = false, message = "Please select a user account." });

                if (string.IsNullOrWhiteSpace(model.RollNumber))
                    return Json(new { success = false, message = "Roll number is required." });

                if (model.StudentId == 0)
                {
                    // Add new
                    model.RegistrationDate ??= DateOnly.FromDateTime(DateTime.Now);
                    _context.Students.Add(model);
                }
                else
                {
                    var existing = await _context.Students.FindAsync(model.StudentId);
                    if (existing == null)
                        return Json(new { success = false, message = "Student not found." });

                    existing.UserId = model.UserId;
                    existing.BranchId = model.BranchId;
                    existing.RollNumber = model.RollNumber;
                    existing.RegistrationDate = model.RegistrationDate;
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "Roll number must be unique. Please use a different roll number." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Students/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                    return Json(new { success = false, message = "Student not found." });

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "This student has linked records (enrollments, results, payments, etc.) and cannot be deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
