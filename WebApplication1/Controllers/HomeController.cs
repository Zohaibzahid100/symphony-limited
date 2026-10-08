using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public HomeController(SymphonyLimitedContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            // ----- Counters: every number on the homepage now comes straight from the DB -----
            ViewBag.StudentCount = await _context.Students.CountAsync();
            ViewBag.BranchCount = await _context.Branch.CountAsync(b => b.BranchStatus == "open");
            ViewBag.CourseCount = await _context.Courses.CountAsync(c => c.CourseStatus == "Available");

            var totalResults = await _context.FinalResults.CountAsync();
            var passedResults = await _context.FinalResults.CountAsync(r => r.ResultStatus == "Pass");
            ViewBag.PassRate = totalResults > 0 ? Math.Round((double)passedResults / totalResults * 100) : 0;

            // ----- Featured courses (top 3) with their tracks -----
            ViewBag.FeaturedCourses = await _context.Courses
                .Include(c => c.CourseTracks)
                .Where(c => c.CourseStatus == "Available")
                .Take(3)
                .ToListAsync();

            // ----- Upcoming entrance exams (top 2) -----
            ViewBag.UpcomingExams = await _context.EntranceExams
                .Include(e => e.Course)
                .Where(e => e.ExamStatus == "Upcoming")
                .OrderBy(e => e.ExamDate)
                .Take(2)
                .ToListAsync();

            // ----- Branches preview (top 3) -----
            ViewBag.Branches = await _context.Branch.Take(3).ToListAsync();

            // ----- FAQs -----
            ViewBag.Faqs = await _context.Faqs.Take(5).ToListAsync();

            // ----- 4-step journey colour fill -----
            // Step 1 (Register) fills once the visitor has an account (session = logged in).
            // Step 2 (Entrance Exam) fills once they have applied to at least one exam.
            bool isRegistered = HttpContext.Session.IsLoggedIn();
            bool hasApplied = false;
            var studentId = HttpContext.Session.GetStudentId();
            if (studentId.HasValue)
            {
                hasApplied = await _context.EntranceResults.AnyAsync(r => r.StudentId == studentId.Value)
                             || HttpContext.Session.GetAppliedExamIds().Any();
            }
            ViewBag.IsRegistered = isRegistered;
            ViewBag.HasApplied = hasApplied;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
