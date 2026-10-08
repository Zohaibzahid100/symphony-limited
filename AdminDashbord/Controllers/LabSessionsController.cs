using AdminDashbord.Models;
using AdminDashbord.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class LabSessionsController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public LabSessionsController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Sessions = await _context.LabSessions
                .Include(s => s.Course)
                .OrderByDescending(s => s.LabSessionId)
                .ToListAsync();

            ViewBag.Registrations = await _context.StudentLabRegistrations
                .Include(r => r.Student).ThenInclude(s => s.User)
                .Include(r => r.LabSession).ThenInclude(ls => ls!.Course)
                .OrderByDescending(r => r.LabRegistrationId)
                .ToListAsync();

            ViewBag.Courses = await _context.Courses.OrderBy(c => c.CourseName).ToListAsync();
            ViewBag.Students = await _context.Students.Include(s => s.User).OrderBy(s => s.RollNumber).ToListAsync();

            return View();
        }

        // ===================== LAB SESSION ===================== //

        [HttpPost]
        public async Task<IActionResult> SaveSession(int LabSessionId, int CourseId, string SessionName, decimal SessionFee)
        {
            try
            {
                if (CourseId == 0 || string.IsNullOrWhiteSpace(SessionName))
                    return Json(new { success = false, message = "Course and session name are required." });

                if (LabSessionId == 0)
                {
                    _context.LabSessions.Add(new LabSession
                    {
                        CourseId = CourseId,
                        SessionName = SessionName,
                        SessionFee = SessionFee
                    });
                }
                else
                {
                    var existing = await _context.LabSessions.FindAsync(LabSessionId);
                    if (existing == null)
                        return Json(new { success = false, message = "Lab session not found." });

                    existing.CourseId = CourseId;
                    existing.SessionName = SessionName;
                    existing.SessionFee = SessionFee;
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
        public async Task<IActionResult> DeleteSession(int id)
        {
            try
            {
                var session = await _context.LabSessions.FindAsync(id);
                if (session == null)
                    return Json(new { success = false, message = "Lab session not found." });

                _context.LabSessions.Remove(session);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "This session has student registrations and cannot be deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===================== REGISTRATION ===================== //

        [HttpPost]
        public async Task<IActionResult> SaveRegistration(StudentLabRegistration model)
        {
            try
            {
                if (model.StudentId == 0 || model.LabSessionId == 0)
                    return Json(new { success = false, message = "Student and lab session are required." });

                if (model.LabRegistrationId == 0)
                {
                    model.RegisterDate ??= DateOnly.FromDateTime(DateTime.Now);
                    _context.StudentLabRegistrations.Add(model);
                }
                else
                {
                    var existing = await _context.StudentLabRegistrations.FindAsync(model.LabRegistrationId);
                    if (existing == null)
                        return Json(new { success = false, message = "Registration not found." });

                    existing.StudentId = model.StudentId;
                    existing.LabSessionId = model.LabSessionId;
                    existing.RegisterDate = model.RegisterDate;
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
        public async Task<IActionResult> DeleteRegistration(int id)
        {
            try
            {
                var reg = await _context.StudentLabRegistrations.FindAsync(id);
                if (reg == null)
                    return Json(new { success = false, message = "Registration not found." });

                _context.StudentLabRegistrations.Remove(reg);
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
