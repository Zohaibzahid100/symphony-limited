using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class FinalExamController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public FinalExamController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        // Lists final exams for courses the logged-in student is actually enrolled in.
        public async Task<IActionResult> Index()
        {
            var studentId = HttpContext.Session.GetStudentId();
            if (studentId == null)
            {
                TempData["LoginError"] = "Please login first.";
                return RedirectToAction("login", "signin");
            }

            var enrolledCourseIds = await _context.StudentEnrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => e.CourseId)
                .ToListAsync();

            var exams = await _context.FinalExams
                .Include(e => e.Course)
                .Where(e => enrolledCourseIds.Contains(e.CourseId))
                .OrderBy(e => e.ExamDate)
                .ToListAsync();

            var takenIds = await _context.FinalResults
                .Where(r => r.StudentId == studentId)
                .Select(r => r.FinalExamId)
                .ToListAsync();

            ViewBag.Exams = exams;
            ViewBag.TakenIds = takenIds;
            return View();
        }

        public async Task<IActionResult> TakeExam(int examId)
        {
            var studentId = HttpContext.Session.GetStudentId();
            if (studentId == null) return RedirectToAction("login", "signin");

            var exam = await _context.FinalExams.Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.FinalExamId == examId);
            if (exam == null) return RedirectToAction("Index");

            var enrolled = await _context.StudentEnrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == exam.CourseId);
            if (!enrolled)
            {
                TempData["LoginSuccess"] = "You must be enrolled in this course to take its final exam.";
                return RedirectToAction("Index");
            }

            var alreadyTaken = await _context.FinalResults
                .AnyAsync(r => r.StudentId == studentId && r.FinalExamId == examId);
            if (alreadyTaken)
            {
                TempData["LoginSuccess"] = "You have already taken this final exam.";
                return RedirectToAction("Index");
            }

            ViewBag.Exam = exam;
            ViewBag.Mcqs = await _context.FinalMcqs.Where(m => m.FinalExamId == examId).ToListAsync();
            return View();
        }

        public class SubmitFinalExamRequest
        {
            public int ExamId { get; set; }
            public Dictionary<int, string> Answers { get; set; } = new();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitExam([FromBody] SubmitFinalExamRequest request)
        {
            var studentId = HttpContext.Session.GetStudentId();
            if (studentId == null) return Json(new { success = false, message = "Please login first." });

            var examId = request.ExamId;
            var answers = request.Answers;

            var exam = await _context.FinalExams.FirstOrDefaultAsync(e => e.FinalExamId == examId);
            if (exam == null) return Json(new { success = false, message = "Exam not found." });

            var enrolled = await _context.StudentEnrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == exam.CourseId);
            if (!enrolled) return Json(new { success = false, message = "Not enrolled in this course." });

            var alreadyTaken = await _context.FinalResults
                .AnyAsync(r => r.StudentId == studentId && r.FinalExamId == examId);
            if (alreadyTaken) return Json(new { success = false, message = "Already submitted." });

            var mcqs = await _context.FinalMcqs.Where(m => m.FinalExamId == examId).ToListAsync();

            int marksObtained = 0, totalMarks = 0, correctCount = 0;
            foreach (var mcq in mcqs)
            {
                totalMarks += mcq.McqMarks;
                var selected = answers != null && answers.TryGetValue(mcq.McqId, out var opt) ? opt : "";
                bool correct = !string.IsNullOrEmpty(selected) &&
                               selected.Trim().Equals(mcq.CorrectOption.Trim(), StringComparison.OrdinalIgnoreCase);
                int marks = correct ? mcq.McqMarks : 0;
                if (correct) correctCount++;

                _context.FinalMcqanswers.Add(new FinalMcqanswer
                {
                    StudentId = studentId.Value,
                    FinalExamId = examId,
                    McqId = mcq.McqId,
                    SelectedOption = string.IsNullOrEmpty(selected) ? "-" : selected.Trim().ToUpper(),
                    Marks = marks,
                    SubmittedAt = DateTime.Now
                });
                marksObtained += marks;
            }

            if (totalMarks == 0) totalMarks = exam.TotalMarks;
            decimal percentage = totalMarks > 0 ? Math.Round((decimal)marksObtained / totalMarks * 100, 2) : 0;
            string grade = percentage >= 85 ? "A+" : percentage >= 70 ? "A" : percentage >= 60 ? "B" : percentage >= 50 ? "C" : "D";
            string status = percentage >= 50 ? "Pass" : "Fail";

            _context.FinalResults.Add(new FinalResult
            {
                StudentId = studentId.Value,
                FinalExamId = examId,
                TotalQuestions = mcqs.Count,
                CorrectAnswers = correctCount,
                WrongAnswers = mcqs.Count - correctCount,
                TotalMarks = totalMarks,
                MarksObtained = marksObtained,
                Percentage = percentage,
                Grade = grade,
                ResultStatus = status,
                ResultDate = DateOnly.FromDateTime(DateTime.Now)
            });

            await _context.SaveChangesAsync();

            return Json(new { success = true, announcementDate = exam.ResultAnnouncementDate?.ToString("dd MMM yyyy") });
        }

        // Printable certificate for a passed, announced final exam result.
        // "Download" = browser's native Print → Save as PDF (no extra libraries needed).
        public async Task<IActionResult> Certificate(int resultId)
        {
            var studentId = HttpContext.Session.GetStudentId();
            if (studentId == null) return RedirectToAction("login", "signin");

            var result = await _context.FinalResults
                .Include(r => r.FinalExam).ThenInclude(e => e.Course)
                .Include(r => r.Student).ThenInclude(s => s.User)
                .FirstOrDefaultAsync(r => r.ResultId == resultId && r.StudentId == studentId);

            if (result == null) return NotFound();
            if (result.ResultStatus != "Pass") return Forbid();

            var today = DateOnly.FromDateTime(DateTime.Now);
            var announceDate = result.FinalExam?.ResultAnnouncementDate;
            if (announceDate != null && today < announceDate)
            {
                TempData["LoginSuccess"] = "Your certificate will be available once the result is announced.";
                return RedirectToAction("Result", "Dashboard");
            }

            return View(result);
        }
    }
}
