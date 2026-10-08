using AdminDashbord.Helpers;
using AdminDashbord.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class CoursesController : Controller
    {
        private readonly SymphonyLimitedContext _context;
        private readonly IWebHostEnvironment _env;
        private const string ImageFolder = "courses";

        public CoursesController(SymphonyLimitedContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Courses
        public async Task<IActionResult> Index()
        {
            ViewBag.Courses = await _context.Courses
                .OrderByDescending(c => c.CourseId)
                .ToListAsync();

            ViewBag.Tracks = await _context.CourseTracks
                .Include(t => t.Course)
                .OrderByDescending(t => t.TrackId)
                .ToListAsync();

            ViewBag.Topics = await _context.CourseTopics
                .Include(t => t.Track).ThenInclude(tr => tr.Course)
                .OrderByDescending(t => t.TopicId)
                .ToListAsync();

            return View();
        }

        // ===================== COURSE ===================== //

        [HttpPost]
        public async Task<IActionResult> SaveCourse(
            int CourseId,
            string CourseName,
            string? Description,
            string CourseStatus,
            IFormFile? CourseImageFile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CourseName))
                    return Json(new { success = false, message = "Course name is required." });

                if (CourseId == 0)
                {
                    if (CourseImageFile == null || CourseImageFile.Length == 0)
                        return Json(new { success = false, message = "Please choose a course image." });

                    var savedName = await FileUploadHelper.SaveImageAsync(CourseImageFile, _env, ImageFolder);

                    var course = new Course
                    {
                        CourseName = CourseName,
                        Description = Description,
                        CourseStatus = string.IsNullOrWhiteSpace(CourseStatus) ? "Available" : CourseStatus,
                        CourseImg = savedName!
                    };
                    _context.Courses.Add(course);
                }
                else
                {
                    var existing = await _context.Courses.FindAsync(CourseId);
                    if (existing == null)
                        return Json(new { success = false, message = "Course not found." });

                    existing.CourseName = CourseName;
                    existing.Description = Description;
                    existing.CourseStatus = string.IsNullOrWhiteSpace(CourseStatus) ? "Available" : CourseStatus;

                    if (CourseImageFile != null && CourseImageFile.Length > 0)
                    {
                        var savedName = await FileUploadHelper.SaveImageAsync(CourseImageFile, _env, ImageFolder);
                        FileUploadHelper.DeleteImage(existing.CourseImg, _env, ImageFolder);
                        existing.CourseImg = savedName!;
                    }
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
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                    return Json(new { success = false, message = "Course not found." });

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                FileUploadHelper.DeleteImage(course.CourseImg, _env, ImageFolder);

                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "This course has linked tracks/exams/etc. and cannot be deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===================== TRACK ===================== //

        [HttpPost]
        public async Task<IActionResult> SaveTrack(int TrackId, int CourseId, string TrackName, int DurationMonths, decimal CourseFee)
        {
            try
            {
                if (CourseId == 0 || string.IsNullOrWhiteSpace(TrackName))
                    return Json(new { success = false, message = "Course and track name are required." });

                if (TrackId == 0)
                {
                    _context.CourseTracks.Add(new CourseTrack
                    {
                        CourseId = CourseId,
                        TrackName = TrackName,
                        DurationMonths = DurationMonths,
                        CourseFee = CourseFee
                    });
                }
                else
                {
                    var existing = await _context.CourseTracks.FindAsync(TrackId);
                    if (existing == null)
                        return Json(new { success = false, message = "Track not found." });

                    existing.CourseId = CourseId;
                    existing.TrackName = TrackName;
                    existing.DurationMonths = DurationMonths;
                    existing.CourseFee = CourseFee;
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
        public async Task<IActionResult> DeleteTrack(int id)
        {
            try
            {
                var track = await _context.CourseTracks.FindAsync(id);
                if (track == null)
                    return Json(new { success = false, message = "Track not found." });

                _context.CourseTracks.Remove(track);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "This track has linked topics/enrollments and cannot be deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===================== TOPIC ===================== //

        [HttpPost]
        public async Task<IActionResult> SaveTopic(int TopicId, int TrackId, string TopicName)
        {
            try
            {
                if (TrackId == 0 || string.IsNullOrWhiteSpace(TopicName))
                    return Json(new { success = false, message = "Track and topic name are required." });

                if (TopicId == 0)
                {
                    _context.CourseTopics.Add(new CourseTopic
                    {
                        TrackId = TrackId,
                        TopicName = TopicName
                    });
                }
                else
                {
                    var existing = await _context.CourseTopics.FindAsync(TopicId);
                    if (existing == null)
                        return Json(new { success = false, message = "Topic not found." });

                    existing.TrackId = TrackId;
                    existing.TopicName = TopicName;
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
        public async Task<IActionResult> DeleteTopic(int id)
        {
            try
            {
                var topic = await _context.CourseTopics.FindAsync(id);
                if (topic == null)
                    return Json(new { success = false, message = "Topic not found." });

                _context.CourseTopics.Remove(topic);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "This topic is used in MCQs and cannot be deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
