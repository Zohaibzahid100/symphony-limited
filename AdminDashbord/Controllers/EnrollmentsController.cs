using AdminDashbord.Models;
using AdminDashbord.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class EnrollmentsController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public EnrollmentsController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.StudentEnrollments
                .Include(e => e.Student).ThenInclude(s => s.User)
                .Include(e => e.Course)
                .Include(e => e.Track)
                .OrderByDescending(e => e.EnrollmentId)
                .ToListAsync();

            ViewBag.Students = await _context.Students.Include(s => s.User).OrderBy(s => s.RollNumber).ToListAsync();
            ViewBag.Courses = await _context.Courses.OrderBy(c => c.CourseName).ToListAsync();
            ViewBag.Tracks = await _context.CourseTracks.Include(t => t.Course).OrderBy(t => t.TrackName).ToListAsync();

            return View(enrollments);
        }

        [HttpPost]
        public async Task<IActionResult> Save(StudentEnrollment model)
        {
            try
            {
                if (model.StudentId == 0 || model.CourseId == 0 || model.TrackId == 0)
                    return Json(new { success = false, message = "Student, course and track are required." });

                if (model.EnrollmentId == 0)
                {
                    model.EnrollmentDate ??= DateOnly.FromDateTime(DateTime.Now);
                    _context.StudentEnrollments.Add(model);
                }
                else
                {
                    var existing = await _context.StudentEnrollments.FindAsync(model.EnrollmentId);
                    if (existing == null)
                        return Json(new { success = false, message = "Enrollment not found." });

                    existing.StudentId = model.StudentId;
                    existing.CourseId = model.CourseId;
                    existing.TrackId = model.TrackId;
                    existing.EnrollmentDate = model.EnrollmentDate;
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var enr = await _context.StudentEnrollments.FindAsync(id);
                if (enr == null)
                    return Json(new { success = false, message = "Enrollment not found." });

                _context.StudentEnrollments.Remove(enr);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "This enrollment has linked records and cannot be deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
