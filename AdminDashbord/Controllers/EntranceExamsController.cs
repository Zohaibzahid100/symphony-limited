using AdminDashbord.Models;
using AdminDashbord.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class EntranceExamsController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public EntranceExamsController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Exams = await _context.EntranceExams
                .Include(e => e.Course)
                .OrderByDescending(e => e.EntranceExamId)
                .ToListAsync();

            ViewBag.Mcqs = await _context.EntranceMcqs
                .Include(m => m.EntranceExam)
                .Include(m => m.Topic)
                .OrderByDescending(m => m.McqId)
                .ToListAsync();

            ViewBag.Courses = await _context.Courses.OrderBy(c => c.CourseName).ToListAsync();
            ViewBag.Topics = await _context.CourseTopics.Include(t => t.Track).ThenInclude(tr => tr.Course).OrderBy(t => t.TopicName).ToListAsync();

            return View();
        }

        // ===================== EXAM ===================== //

        [HttpPost]
        public async Task<IActionResult> SaveExam(EntranceExam model)
        {
            try
            {
                if (model.CourseId == 0 || string.IsNullOrWhiteSpace(model.ExamTitle))
                    return Json(new { success = false, message = "Course and exam title are required." });

                if (model.EntranceExamId == 0)
                {
                    model.ExamStatus = string.IsNullOrWhiteSpace(model.ExamStatus) ? "Upcoming" : model.ExamStatus;
                    _context.EntranceExams.Add(model);
                }
                else
                {
                    var existing = await _context.EntranceExams.FindAsync(model.EntranceExamId);
                    if (existing == null)
                        return Json(new { success = false, message = "Exam not found." });

                    existing.CourseId = model.CourseId;
                    existing.ExamTitle = model.ExamTitle;
                    existing.ExamDate = model.ExamDate;
                    existing.LastDateToApply = model.LastDateToApply;
                    existing.ExamFee = model.ExamFee;
                    existing.TotalMarks = model.TotalMarks;
                    existing.ExamStatus = string.IsNullOrWhiteSpace(model.ExamStatus) ? "Upcoming" : model.ExamStatus;
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
                var exam = await _context.EntranceExams.FindAsync(id);
                if (exam == null)
                    return Json(new { success = false, message = "Exam not found." });

                _context.EntranceExams.Remove(exam);
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
            int McqId, int EntranceExamId, int? TopicId, string QuestionText,
            string OptionA, string OptionB, string OptionC, string OptionD,
            string CorrectOption, int Mcqmarks)
        {
            try
            {
                if (EntranceExamId == 0 || string.IsNullOrWhiteSpace(QuestionText))
                    return Json(new { success = false, message = "Exam and question text are required." });

                if (McqId == 0)
                {
                    _context.EntranceMcqs.Add(new EntranceMcq
                    {
                        EntranceExamId = EntranceExamId,
                        TopicId = TopicId,
                        QuestionText = QuestionText,
                        OptionA = OptionA,
                        OptionB = OptionB,
                        OptionC = OptionC,
                        OptionD = OptionD,
                        CorrectOption = CorrectOption,
                        Mcqmarks = Mcqmarks <= 0 ? 1 : Mcqmarks
                    });
                }
                else
                {
                    var existing = await _context.EntranceMcqs.FindAsync(McqId);
                    if (existing == null)
                        return Json(new { success = false, message = "MCQ not found." });

                    existing.EntranceExamId = EntranceExamId;
                    existing.TopicId = TopicId;
                    existing.QuestionText = QuestionText;
                    existing.OptionA = OptionA;
                    existing.OptionB = OptionB;
                    existing.OptionC = OptionC;
                    existing.OptionD = OptionD;
                    existing.CorrectOption = CorrectOption;
                    existing.Mcqmarks = Mcqmarks <= 0 ? 1 : Mcqmarks;
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
                var mcq = await _context.EntranceMcqs.FindAsync(id);
                if (mcq == null)
                    return Json(new { success = false, message = "MCQ not found." });

                _context.EntranceMcqs.Remove(mcq);
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
