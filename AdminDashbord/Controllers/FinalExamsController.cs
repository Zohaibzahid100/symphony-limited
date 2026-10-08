using AdminDashbord.Models;
using AdminDashbord.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class FinalExamsController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public FinalExamsController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Exams = await _context.FinalExams
                .Include(e => e.Course)
                .OrderByDescending(e => e.FinalExamId)
                .ToListAsync();

            ViewBag.Mcqs = await _context.FinalMcqs
                .Include(m => m.FinalExam)
                .Include(m => m.Topic)
                .OrderByDescending(m => m.McqId)
                .ToListAsync();

            ViewBag.Courses = await _context.Courses.OrderBy(c => c.CourseName).ToListAsync();
            ViewBag.Topics = await _context.CourseTopics.Include(t => t.Track).ThenInclude(tr => tr.Course).OrderBy(t => t.TopicName).ToListAsync();

            return View();
        }

        // ===================== EXAM ===================== //

        [HttpPost]
        public async Task<IActionResult> SaveExam(int FinalExamId, int CourseId, string ExamTitle, DateOnly? ExamDate, int TotalMarks)
        {
            try
            {
                if (CourseId == 0 || string.IsNullOrWhiteSpace(ExamTitle))
                    return Json(new { success = false, message = "Course and exam title are required." });

                if (FinalExamId == 0)
                {
                    _context.FinalExams.Add(new FinalExam
                    {
                        CourseId = CourseId,
                        ExamTitle = ExamTitle,
                        ExamDate = ExamDate,
                        TotalMarks = TotalMarks
                    });
                }
                else
                {
                    var existing = await _context.FinalExams.FindAsync(FinalExamId);
                    if (existing == null)
                        return Json(new { success = false, message = "Exam not found." });

                    existing.CourseId = CourseId;
                    existing.ExamTitle = ExamTitle;
                    existing.ExamDate = ExamDate;
                    existing.TotalMarks = TotalMarks;
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
        public async Task<IActionResult> DeleteExam(int id)
        {
            try
            {
                var exam = await _context.FinalExams.FindAsync(id);
                if (exam == null)
                    return Json(new { success = false, message = "Exam not found." });

                _context.FinalExams.Remove(exam);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "This exam has linked MCQs/results and cannot be deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===================== MCQ ===================== //

        [HttpPost]
        public async Task<IActionResult> SaveMcq(
            int McqId, int FinalExamId, int? TopicId, string QuestionText,
            string OptionA, string OptionB, string OptionC, string OptionD,
            string CorrectOption, int McqMarks)
        {
            try
            {
                if (FinalExamId == 0 || string.IsNullOrWhiteSpace(QuestionText))
                    return Json(new { success = false, message = "Exam and question text are required." });

                if (McqId == 0)
                {
                    _context.FinalMcqs.Add(new FinalMcq
                    {
                        FinalExamId = FinalExamId,
                        TopicId = TopicId,
                        QuestionText = QuestionText,
                        OptionA = OptionA,
                        OptionB = OptionB,
                        OptionC = OptionC,
                        OptionD = OptionD,
                        CorrectOption = CorrectOption,
                        McqMarks = McqMarks <= 0 ? 1 : McqMarks
                    });
                }
                else
                {
                    var existing = await _context.FinalMcqs.FindAsync(McqId);
                    if (existing == null)
                        return Json(new { success = false, message = "MCQ not found." });

                    existing.FinalExamId = FinalExamId;
                    existing.TopicId = TopicId;
                    existing.QuestionText = QuestionText;
                    existing.OptionA = OptionA;
                    existing.OptionB = OptionB;
                    existing.OptionC = OptionC;
                    existing.OptionD = OptionD;
                    existing.CorrectOption = CorrectOption;
                    existing.McqMarks = McqMarks <= 0 ? 1 : McqMarks;
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
        public async Task<IActionResult> DeleteMcq(int id)
        {
            try
            {
                var mcq = await _context.FinalMcqs.FindAsync(id);
                if (mcq == null)
                    return Json(new { success = false, message = "MCQ not found." });

                _context.FinalMcqs.Remove(mcq);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "This MCQ has been answered by students and cannot be deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
