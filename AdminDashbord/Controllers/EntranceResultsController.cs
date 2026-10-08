using AdminDashbord.Models;
using AdminDashbord.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class EntranceResultsController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public EntranceResultsController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Results = await _context.EntranceResults
                .Include(r => r.Student).ThenInclude(s => s.User)
                .Include(r => r.EntranceExam)
                .Include(r => r.AssignedTrack).ThenInclude(t => t!.Course)
                .OrderByDescending(r => r.ResultId)
                .ToListAsync();

            ViewBag.Answers = await _context.StudentMcqanswers
                .Include(a => a.Student).ThenInclude(s => s.User)
                .Include(a => a.EntranceExam)
                .Include(a => a.Mcq)
                .OrderByDescending(a => a.AnswerId)
                .ToListAsync();

            ViewBag.Students = await _context.Students.Include(s => s.User).OrderBy(s => s.RollNumber).ToListAsync();
            ViewBag.Exams = await _context.EntranceExams.OrderBy(e => e.ExamTitle).ToListAsync();
            ViewBag.Tracks = await _context.CourseTracks.Include(t => t.Course).OrderBy(t => t.TrackName).ToListAsync();
            ViewBag.Mcqs = await _context.EntranceMcqs.Include(m => m.EntranceExam).OrderBy(m => m.McqId).ToListAsync();

            return View();
        }

        // ===================== RESULT ===================== //

        [HttpPost]
        public async Task<IActionResult> SaveResult(EntranceResult model)
        {
            try
            {
                if (model.StudentId == 0 || model.EntranceExamId == 0)
                    return Json(new { success = false, message = "Student and exam are required." });

                if (model.ResultId == 0)
                {
                    model.ResultDate ??= DateOnly.FromDateTime(DateTime.Now);
                    _context.EntranceResults.Add(model);
                }
                else
                {
                    var existing = await _context.EntranceResults.FindAsync(model.ResultId);
                    if (existing == null)
                        return Json(new { success = false, message = "Result not found." });

                    existing.StudentId = model.StudentId;
                    existing.EntranceExamId = model.EntranceExamId;
                    existing.MarksObtained = model.MarksObtained;
                    existing.TotalMarks = model.TotalMarks;
                    existing.Percentage = model.Percentage;
                    existing.ResultStatus = model.ResultStatus;
                    existing.AssignedTrackId = model.AssignedTrackId;
                    existing.ResultDate = model.ResultDate;
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
        public async Task<IActionResult> DeleteResult(int id)
        {
            try
            {
                var r = await _context.EntranceResults.FindAsync(id);
                if (r == null)
                    return Json(new { success = false, message = "Result not found." });

                _context.EntranceResults.Remove(r);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===================== MCQ ANSWER ===================== //

        [HttpPost]
        public async Task<IActionResult> SaveAnswer(StudentMcqanswer model)
        {
            try
            {
                if (model.StudentId == 0 || model.EntranceExamId == 0 || model.McqId == 0)
                    return Json(new { success = false, message = "Student, exam and MCQ are required." });

                if (model.AnswerId == 0)
                {
                    model.SubmittedAt ??= DateTime.Now;
                    _context.StudentMcqanswers.Add(model);
                }
                else
                {
                    var existing = await _context.StudentMcqanswers.FindAsync(model.AnswerId);
                    if (existing == null)
                        return Json(new { success = false, message = "Answer not found." });

                    existing.StudentId = model.StudentId;
                    existing.EntranceExamId = model.EntranceExamId;
                    existing.McqId = model.McqId;
                    existing.SelectedOption = model.SelectedOption;
                    existing.Marks = model.Marks;
                    existing.SubmittedAt = model.SubmittedAt;
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
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            try
            {
                var a = await _context.StudentMcqanswers.FindAsync(id);
                if (a == null)
                    return Json(new { success = false, message = "Answer not found." });

                _context.StudentMcqanswers.Remove(a);
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
