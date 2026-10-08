using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CourseController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public CourseController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> course()
        {
            var courses = await _context.Courses
                .Include(c => c.CourseTracks)
                .ToListAsync();

            ViewBag.Courses = courses;
            ViewBag.CourseCount = courses.Count;
            return View();
        }

        // Backs the "View Details" course modal — returns the course + its tracks/topics as JSON.
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Include(c => c.CourseTracks)
                    .ThenInclude(t => t.CourseTopics)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return NotFound();

            string[] icons = { "🌐", "🗄️", "🎨", "🔌" };

            var result = new
            {
                title = course.CourseName,
                icon = icons[course.CourseId % icons.Length],
                desc = course.Description,
                tracks = course.CourseTracks.Select(t => new
                {
                    name = t.TrackName,
                    duration = t.DurationMonths,
                    fee = t.CourseFee,
                    topics = t.CourseTopics.Select(tp => tp.TopicName).ToList()
                })
            };

            return Json(result);
        }
    }
}
