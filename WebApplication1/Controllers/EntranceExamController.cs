using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class EntranceExamController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public EntranceExamController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> enExam()
        {
            var exams = await _context.EntranceExams
                .Include(e => e.Course)
                .OrderBy(e => e.ExamDate)
                .ToListAsync();

            ViewBag.Exams = exams;

            ViewBag.Branches = await _context.Branch
              .Where(b => b.BranchStatus.ToLower() == "open")
              .ToListAsync();
            ViewBag.IsRegistered = HttpContext.Session.IsLoggedIn();

            // Which exams has this student already applied to (used to fill step 1 & 2 + disable Apply button per exam)
            var appliedIds = HttpContext.Session.GetAppliedExamIds();
            var studentId = HttpContext.Session.GetStudentId();
            if (studentId.HasValue)
            {
                var dbApplied = await _context.EntranceExamApplications
                    .Where(a => a.StudentId == studentId.Value)
                    .Select(a => a.EntranceExamId)
                    .ToListAsync();
                appliedIds = appliedIds.Union(dbApplied).ToList();
            }
            ViewBag.AppliedExamIds = appliedIds;
            ViewBag.HasApplied = appliedIds.Any();

            return View();
        }

        // Persists a "student applied for this entrance exam" marker.
        // Real fee collection happens at the branch (per the school's actual process),
        // so we don't create a paid EntranceResult row here — just flag the application
        // in session so the UI (steps, buttons) reacts correctly.
        [HttpPost]
        public async Task<IActionResult> Apply(int examId, int branchId, string paymentMethod)
        {
            if (!HttpContext.Session.IsLoggedIn())
            {
                return Json(new { success = false, message = "Please login first." });
            }

            var studentId = HttpContext.Session.GetStudentId();

            if (studentId == null)
            {
                return Json(new { success = false, message = "Student not found." });
            }

            // check duplicate application — using the dedicated applications table,
            // not EntranceResults (results only exist after the exam is graded).
            var alreadyApplied = await _context.EntranceExamApplications
                .AnyAsync(a => a.StudentId == studentId && a.EntranceExamId == examId);

            if (alreadyApplied)
            {
                return Json(new { success = false, message = "Already applied for this exam." });
            }

            // exam fetch (fee ke liye)
            var exam = await _context.EntranceExams
                .FirstOrDefaultAsync(e => e.EntranceExamId == examId);

            if (exam == null)
            {
                return Json(new { success = false, message = "Exam not found." });
            }

            // ✅ PAYMENT SAVE
            var payment = new Payment
            {
                StudentId = studentId.Value,
                Amount = exam.ExamFee,
                PaymentMethod = paymentMethod,
                PaymentType = "EntranceFee",
                PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                BranchId = branchId
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(); // need payment.PaymentId before linking the application

            // ✅ APPLICATION RECORD — this is what unlocks the student's dashboard
            var application = new EntranceExamApplication
            {
                StudentId = studentId.Value,
                EntranceExamId = examId,
                BranchId = branchId,
                PaymentId = payment.PaymentId,
                ApplyDate = DateOnly.FromDateTime(DateTime.Now),
                Status = "Applied"
            };
            _context.EntranceExamApplications.Add(application);

            await _context.SaveChangesAsync();

            HttpContext.Session.AddAppliedExamId(examId);

            return Json(new { success = true });
        }
        // Student opens this once they've applied (and paid at branch) — shows the
        // actual MCQs from EntranceMCQs for this exam. Blocks re-entry if already
        // has a result (exam already taken/graded).
        public async Task<IActionResult> TakeExam(int examId)
        {
            var studentId = HttpContext.Session.GetStudentId();
            if (studentId == null)
            {
                TempData["LoginError"] = "Please login first.";
                return RedirectToAction("login", "signin");
            }

            var hasApplied = await _context.EntranceExamApplications
                .AnyAsync(a => a.StudentId == studentId && a.EntranceExamId == examId);
            if (!hasApplied)
            {
                TempData["LoginSuccess"] = "Please apply for this exam first.";
                return RedirectToAction("enExam");
            }

            var alreadyTaken = await _context.EntranceResults
                .AnyAsync(r => r.StudentId == studentId && r.EntranceExamId == examId);
            if (alreadyTaken)
            {
                TempData["LoginSuccess"] = "You have already taken this exam.";
                return RedirectToAction("enExam");
            }

            var exam = await _context.EntranceExams.Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EntranceExamId == examId);
            if (exam == null) return RedirectToAction("enExam");

            var mcqs = await _context.EntranceMcqs
                .Where(m => m.EntranceExamId == examId)
                .ToListAsync();

            ViewBag.Exam = exam;
            ViewBag.Mcqs = mcqs;
            return View();
        }

        // Grades the submitted answers, persists StudentMCQAnswers + EntranceResult,
        // assigns a track via TrackAssignmentRules, and — this is what turns an
        // applicant into an enrolled student — creates the StudentEnrollment row.
        public class SubmitExamRequest
        {
            public int ExamId { get; set; }
            public Dictionary<int, string> Answers { get; set; } = new();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitExam([FromBody] SubmitExamRequest request)
        {
            var examId = request.ExamId;
            var answers = request.Answers;
            var studentId = HttpContext.Session.GetStudentId();
            if (studentId == null)
            {
                return Json(new { success = false, message = "Please login first." });
            }

            var alreadyTaken = await _context.EntranceResults
                .AnyAsync(r => r.StudentId == studentId && r.EntranceExamId == examId);
            if (alreadyTaken)
            {
                return Json(new { success = false, message = "Exam already submitted." });
            }

            var exam = await _context.EntranceExams.FirstOrDefaultAsync(e => e.EntranceExamId == examId);
            if (exam == null) return Json(new { success = false, message = "Exam not found." });

            var mcqs = await _context.EntranceMcqs.Where(m => m.EntranceExamId == examId).ToListAsync();

            int marksObtained = 0;
            int totalMarks = 0;
            foreach (var mcq in mcqs)
            {
                totalMarks += mcq.Mcqmarks;
                var selected = answers != null && answers.TryGetValue(mcq.McqId, out var opt) ? opt : "";
                bool correct = !string.IsNullOrEmpty(selected) &&
                               selected.Trim().Equals(mcq.CorrectOption.Trim(), StringComparison.OrdinalIgnoreCase);
                int marksForThis = correct ? mcq.Mcqmarks : 0;

                _context.StudentMcqanswers.Add(new StudentMcqanswer
                {
                    StudentId = studentId.Value,
                    EntranceExamId = examId,
                    McqId = mcq.McqId,
                    SelectedOption = string.IsNullOrEmpty(selected) ? "-" : selected.Trim().ToUpper(),
                    Marks = marksForThis,
                    SubmittedAt = DateTime.Now
                });

                marksObtained += marksForThis;
            }

            if (totalMarks == 0) totalMarks = exam.TotalMarks; // fallback if no MCQs were seeded
            decimal percentage = totalMarks > 0 ? Math.Round((decimal)marksObtained / totalMarks * 100, 2) : 0;

            // Track assignment — based on this course's rules (falls back to "no track" if none defined).
            var rule = await _context.TrackAssignmentRules
                .Include(r => r.AssignedTrack)
                .Where(r => r.CourseId == exam.CourseId && percentage >= r.MinPercentage && percentage <= r.MaxPercentage)
                .FirstOrDefaultAsync();

            var result = new EntranceResult
            {
                StudentId = studentId.Value,
                EntranceExamId = examId,
                MarksObtained = marksObtained,
                TotalMarks = totalMarks,
                Percentage = percentage,
                ResultStatus = "pass", // entrance exam has no fail — it only routes the track (see enExam FAQ)
                AssignedTrackId = rule?.AssignedTrackId,
                ResultDate = DateOnly.FromDateTime(DateTime.Now)
            };
            _context.EntranceResults.Add(result);

            // NOTE: enrollment is no longer automatic here. The student sees their
            // result on the Results tab once announced, and explicitly enrolls via
            // the "Enroll Now" form there (EntranceExam/EnrollNow) — same pattern as
            // the entrance-exam Apply flow.

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                announcementDate = exam.ResultAnnouncementDate?.ToString("dd MMM yyyy")
            });
        }

        // Backs the Enroll Now modal — course + assigned-track details (duration, fee),
        // same idea as Course/Details for the entrance-exam Apply flow.
        [HttpGet]
        public async Task<IActionResult> EnrollInfo(int entranceExamId)
        {
            var studentId = HttpContext.Session.GetStudentId();
            if (studentId == null) return Json(new { success = false, message = "Please login first." });

            var exam = await _context.EntranceExams.Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EntranceExamId == entranceExamId);
            if (exam == null) return Json(new { success = false, message = "Exam not found." });

            var result = await _context.EntranceResults.Include(r => r.AssignedTrack)
                .FirstOrDefaultAsync(r => r.StudentId == studentId && r.EntranceExamId == entranceExamId);
            if (result == null || result.AssignedTrack == null)
            {
                return Json(new { success = false, message = "No track assigned for this result yet." });
            }

            return Json(new
            {
                success = true,
                courseName = exam.Course?.CourseName,
                trackName = result.AssignedTrack.TrackName,
                durationMonths = result.AssignedTrack.DurationMonths,
                fee = result.AssignedTrack.CourseFee
            });
        }

        [HttpPost]
        public async Task<IActionResult> EnrollNow(int entranceExamId, int branchId, string paymentMethod)
        {
            var studentId = HttpContext.Session.GetStudentId();
            if (studentId == null) return Json(new { success = false, message = "Please login first." });

            var exam = await _context.EntranceExams.FirstOrDefaultAsync(e => e.EntranceExamId == entranceExamId);
            if (exam == null) return Json(new { success = false, message = "Exam not found." });

            var result = await _context.EntranceResults.Include(r => r.AssignedTrack)
                .FirstOrDefaultAsync(r => r.StudentId == studentId && r.EntranceExamId == entranceExamId);
            if (result == null || result.AssignedTrackId == null || result.AssignedTrack == null)
            {
                return Json(new { success = false, message = "No track assigned for this result yet." });
            }

            // respect the announcement date — no enrolling on a result the student
            // can't actually see yet.
            var today = DateOnly.FromDateTime(DateTime.Now);
            if (exam.ResultAnnouncementDate != null && today < exam.ResultAnnouncementDate)
            {
                return Json(new { success = false, message = "Result not announced yet." });
            }

            var alreadyEnrolled = await _context.StudentEnrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == exam.CourseId);
            if (alreadyEnrolled)
            {
                return Json(new { success = false, message = "Already enrolled in this course." });
            }

            // ✅ PAYMENT SAVE — course fee, recorded against this student/branch.
            var payment = new Payment
            {
                StudentId = studentId.Value,
                Amount = result.AssignedTrack.CourseFee,
                PaymentMethod = paymentMethod,
                PaymentType = "CourseFee",
                PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                BranchId = branchId,
                PaymentStatus = "Paid" // collected at the moment of confirming enrollment
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(); // need payment.PaymentId

            // Enrollment only happens once the payment is actually Paid.
            if (payment.PaymentStatus != "Paid")
            {
                return Json(new { success = false, message = "Payment not completed — enrollment not created." });
            }

            _context.StudentEnrollments.Add(new StudentEnrollment
            {
                StudentId = studentId.Value,
                CourseId = exam.CourseId,
                TrackId = result.AssignedTrackId.Value,
                EnrollmentDate = DateOnly.FromDateTime(DateTime.Now),
                PaymentId = payment.PaymentId
            });
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

    }
}
