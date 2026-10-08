using AdminDashbord.Models;
using AdminDashbord.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class FinalResultsController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public FinalResultsController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Results = await _context.FinalResults
                .Include(r => r.Student).ThenInclude(s => s.User)
                .Include(r => r.FinalExam)
                .OrderByDescending(r => r.ResultId)
                .ToListAsync();

            ViewBag.Answers = await _context.FinalMcqanswers
                .Include(a => a.Student).ThenInclude(s => s.User)
                .Include(a => a.FinalExam)
                .Include(a => a.Mcq)
                .OrderByDescending(a => a.AnswerId)
                .ToListAsync();

            ViewBag.Students = await _context.Students.Include(s => s.User).OrderBy(s => s.RollNumber).ToListAsync();
            ViewBag.Exams = await _context.FinalExams.OrderBy(e => e.ExamTitle).ToListAsync();
            ViewBag.Mcqs = await _context.FinalMcqs.Include(m => m.FinalExam).OrderBy(m => m.McqId).ToListAsync();

            return View();
        }

        // ===================== RESULT ===================== //

        [HttpPost]
        public async Task<IActionResult> SaveResult(FinalResult model)
        {
            try
            {
                if (model.StudentId == 0 || model.FinalExamId == 0)
                    return Json(new { success = false, message = "Student and exam are required." });

                if (model.ResultId == 0)
                {
                    model.ResultDate ??= DateOnly.FromDateTime(DateTime.Now);
                    _context.FinalResults.Add(model);
                }
                else
                {
                    var existing = await _context.FinalResults.FindAsync(model.ResultId);
                    if (existing == null)
                        return Json(new { success = false, message = "Result not found." });

                    existing.StudentId = model.StudentId;
                    existing.FinalExamId = model.FinalExamId;
                    existing.TotalQuestions = model.TotalQuestions;
                    existing.TotalMarks = model.TotalMarks;
                    existing.CorrectAnswers = model.CorrectAnswers;
                    existing.WrongAnswers = model.WrongAnswers;
                    existing.MarksObtained = model.MarksObtained;
                    existing.Percentage = model.Percentage;
                    existing.Grade = model.Grade;
                    existing.ResultStatus = model.ResultStatus;
                    existing.Remarks = model.Remarks;
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
                var r = await _context.FinalResults.FindAsync(id);
                if (r == null)
                    return Json(new { success = false, message = "Result not found." });

                _context.FinalResults.Remove(r);
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
        public async Task<IActionResult> SaveAnswer(FinalMcqanswer model)
        {
            try
            {
                if (model.StudentId == 0 || model.FinalExamId == 0 || model.McqId == 0)
                    return Json(new { success = false, message = "Student, exam and MCQ are required." });

                if (model.AnswerId == 0)
                {
                    model.SubmittedAt ??= DateTime.Now;
                    _context.FinalMcqanswers.Add(model);
                }
                else
                {
                    var existing = await _context.FinalMcqanswers.FindAsync(model.AnswerId);
                    if (existing == null)
                        return Json(new { success = false, message = "Answer not found." });

                    existing.StudentId = model.StudentId;
                    existing.FinalExamId = model.FinalExamId;
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
                var a = await _context.FinalMcqanswers.FindAsync(id);
                if (a == null)
                    return Json(new { success = false, message = "Answer not found." });

                _context.FinalMcqanswers.Remove(a);
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
