using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AboutController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public AboutController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> about()
        {
            ViewBag.AboutSections = await _context.AboutUs.ToListAsync();
            ViewBag.StudentCount = await _context.Students.CountAsync();
            ViewBag.BranchCount = await _context.Branch.CountAsync(b => b.BranchStatus == "open");
            ViewBag.CourseCount = await _context.Courses.CountAsync(c => c.CourseStatus == "Available");

            var totalResults = await _context.FinalResults.CountAsync();
            var passedResults = await _context.FinalResults.CountAsync(r => r.ResultStatus == "Pass");
            ViewBag.PassRate = totalResults > 0 ? Math.Round((double)passedResults / totalResults * 100) : 0;

            ViewBag.Faqs = await _context.Faqs.ToListAsync();
            return View();
        }
    }
}
