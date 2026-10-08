using AdminDashbord.Models;
using AdminDashbord.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class TrackRulesController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public TrackRulesController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rules = await _context.TrackAssignmentRules
                .Include(r => r.Course)
                .Include(r => r.AssignedTrack)
                .OrderByDescending(r => r.RuleId)
                .ToListAsync();

            ViewBag.Courses = await _context.Courses.OrderBy(c => c.CourseName).ToListAsync();
            ViewBag.Tracks = await _context.CourseTracks.Include(t => t.Course).OrderBy(t => t.TrackName).ToListAsync();

            return View(rules);
        }

        [HttpPost]
        public async Task<IActionResult> Save(TrackAssignmentRule model)
        {
            try
            {
                if (model.CourseId == 0 || model.AssignedTrackId == 0)
                    return Json(new { success = false, message = "Course and assigned track are required." });

                if (model.MinPercentage > model.MaxPercentage)
                    return Json(new { success = false, message = "Minimum percentage cannot be greater than maximum percentage." });

                if (model.RuleId == 0)
                {
                    _context.TrackAssignmentRules.Add(model);
                }
                else
                {
                    var existing = await _context.TrackAssignmentRules.FindAsync(model.RuleId);
                    if (existing == null)
                        return Json(new { success = false, message = "Rule not found." });

                    existing.CourseId = model.CourseId;
                    existing.MinPercentage = model.MinPercentage;
                    existing.MaxPercentage = model.MaxPercentage;
                    existing.AssignedTrackId = model.AssignedTrackId;
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
                var rule = await _context.TrackAssignmentRules.FindAsync(id);
                if (rule == null)
                    return Json(new { success = false, message = "Rule not found." });

                _context.TrackAssignmentRules.Remove(rule);
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
