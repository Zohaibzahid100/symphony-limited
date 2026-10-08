using AdminDashbord.Helpers;
using AdminDashbord.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class DashbordController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public DashbordController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> dashbord()
        {
            // Basic counts for stat cards
            ViewBag.TotalStudents   = await _context.Students.CountAsync();
            ViewBag.TotalCourses    = await _context.Courses.CountAsync();
            ViewBag.TotalBranches   = await _context.Branches.CountAsync();
            ViewBag.TotalEnrollments= await _context.StudentEnrollments.CountAsync();
            ViewBag.TotalPayments   = await _context.Payments.SumAsync(p => (decimal?)p.Amount) ?? 0;
            ViewBag.TotalExams      = await _context.EntranceExams.CountAsync();

            // Recent 5 students
            ViewBag.RecentStudents  = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Branch)
                .OrderByDescending(s => s.StudentId)
                .Take(5)
                .ToListAsync();

            // Recent 5 payments
            ViewBag.RecentPayments  = await _context.Payments
                .Include(p => p.Student).ThenInclude(s => s!.User)
                .OrderByDescending(p => p.PaymentId)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}
