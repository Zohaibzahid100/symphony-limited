using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers.StudentPortal
{
    public class DashboardController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public DashboardController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        // Every action here needs a logged-in student. Centralized check instead of
        // a full [Authorize] pipeline since this project doesn't use ASP.NET Identity.
        private async Task<Student?> CurrentStudentAsync()
        {
            var studentId = HttpContext.Session.GetStudentId();
            if (!studentId.HasValue) return null;

            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.Branch)
                .FirstOrDefaultAsync(s => s.StudentId == studentId.Value);
        }

        private void PushSidebarInfo(Student student)
        {
            ViewData["StudentName"] = student.User?.FullName ?? "Student";
            ViewData["StudentRoll"] = student.RollNumber ?? "—";
        }

        // A student only gets full dashboard access once they have a real, persisted
        // entrance-exam application row (created by EntranceExamController.Apply).
        private async Task<bool> HasAppliedForEntranceAsync(int studentId)
        {
            return await _context.EntranceExamApplications
                .AnyAsync(a => a.StudentId == studentId);
        }

        // Call at the top of every gated action. Returns the Locked view (rendered
        // inside the dashboard layout, same URL) if the student hasn't applied yet;
        // returns null if they're clear to proceed.
        private async Task<IActionResult?> RequireEntranceApplicationAsync(Student student)
        {
            var hasApplied = await HasAppliedForEntranceAsync(student.StudentId);
            if (!hasApplied)
            {
                PushSidebarInfo(student);
                ViewBag.LockMessage = "Please apply for the entrance exam first — entrance exam application/enrollment ke baghair dashboard ke pages available nahi hain.";
                return View("Locked");
            }
            return null;
        }

        public async Task<IActionResult> Dashboard()
        {
            var student = await CurrentStudentAsync();
            if (student == null) return RedirectToAction("login", "signin");
            var gate = await RequireEntranceApplicationAsync(student);
            if (gate != null) return gate;
            PushSidebarInfo(student);

            var enrollments = await _context.StudentEnrollments
                .Include(e => e.Course)
                .Include(e => e.Track)
                .Where(e => e.StudentId == student.StudentId)
                .ToListAsync();

            var entranceResults = await _context.EntranceResults
                .Include(r => r.EntranceExam)
                .Where(r => r.StudentId == student.StudentId)
                .ToListAsync();

            var finalResults = await _context.FinalResults
                .Include(r => r.FinalExam)
                .Where(r => r.StudentId == student.StudentId)
                .ToListAsync();

            var payments = await _context.Payments
                .Where(p => p.StudentId == student.StudentId)
                .ToListAsync();

            var applications = await _context.EntranceExamApplications
                .Include(a => a.EntranceExam).ThenInclude(e => e.Course)
                .Include(a => a.Branch)
                .Where(a => a.StudentId == student.StudentId)
                .OrderByDescending(a => a.ApplyDate)
                .ToListAsync();
            ViewBag.Applications = applications;

            decimal bestScore = 0;
            if (entranceResults.Any())
                bestScore = Math.Max(bestScore, entranceResults.Max(r => r.Percentage ?? 0));
            if (finalResults.Any())
                bestScore = Math.Max(bestScore, finalResults.Max(r => r.Percentage));

            ViewBag.Student = student;
            ViewBag.EnrollmentCount = enrollments.Count;
            ViewBag.ExamsTaken = entranceResults.Count + finalResults.Count;
            ViewBag.PaymentsMade = payments.Count;
            ViewBag.BestScore = Math.Round(bestScore);
            ViewBag.Enrollments = enrollments;
            ViewBag.Payments = payments.OrderByDescending(p => p.PaymentDate).Take(5).ToList();

            return View("Dashboard");
        }

        public async Task<IActionResult> Enrollment()
        {
            var student = await CurrentStudentAsync();
            if (student == null) return RedirectToAction("login", "signin");
            var gate = await RequireEntranceApplicationAsync(student);
            if (gate != null) return gate;
            PushSidebarInfo(student);

            ViewBag.Enrollments = await _context.StudentEnrollments
                .Include(e => e.Course)
                .Include(e => e.Track)
                .Where(e => e.StudentId == student.StudentId)
                .Select(e => new
                {
                    e.EnrollmentId,

                    CourseName = e.Course != null ? e.Course.CourseName : "",
                    TrackName = e.Track != null ? e.Track.TrackName : "",

                    e.EnrollmentDate,
                    DurationMonths = e.Track != null ? e.Track.DurationMonths : 0,
                    CourseFee = e.Track != null ? e.Track.CourseFee : 0
                })
                .ToListAsync();

            return View();
        }

        public async Task<IActionResult> Result()
        {
            var student = await CurrentStudentAsync();
            if (student == null) return RedirectToAction("login", "signin");
            var gate = await RequireEntranceApplicationAsync(student);
            if (gate != null) return gate;
            PushSidebarInfo(student);

            ViewBag.EntranceResults = await _context.EntranceResults
                .Include(r => r.EntranceExam).ThenInclude(e => e.Course)
                .Include(r => r.AssignedTrack)
                .Where(r => r.StudentId == student.StudentId)
                .ToListAsync();

            ViewBag.FinalResults = await _context.FinalResults
                .Include(r => r.FinalExam).ThenInclude(e => e.Course)
                .Where(r => r.StudentId == student.StudentId)
                .ToListAsync();

            var today = DateOnly.FromDateTime(DateTime.Now);
            ViewBag.Today = today;
            // result hai database me (auto-graded on submit), but the UI only reveals
            // marks once today >= the exam's ResultAnnouncementDate (set by admin per exam).
            // No date set => treated as announced immediately.

            ViewBag.EnrolledCourseIds = await _context.StudentEnrollments
                .Where(e => e.StudentId == student.StudentId)
                .Select(e => e.CourseId)
                .ToListAsync();

            ViewBag.Branches = await _context.Branch
                .Where(b => b.BranchStatus.ToLower() == "open")
                .ToListAsync();

            return View();
        }

        public async Task<IActionResult> Labsec()
        {
            var student = await CurrentStudentAsync();
            if (student == null) return RedirectToAction("login", "signin");
            var gate = await RequireEntranceApplicationAsync(student);
            if (gate != null) return gate;
            PushSidebarInfo(student);

            ViewBag.LabRegistrations = await _context.StudentLabRegistrations
                .Include(l => l.LabSession).ThenInclude(s => s.Course)
                .Where(l => l.StudentId == student.StudentId)
                .ToListAsync();

            // Lab sessions the student hasn't registered for yet (so they can register).
            var registeredSessionIds = await _context.StudentLabRegistrations
                .Where(l => l.StudentId == student.StudentId)
                .Select(l => l.LabSessionId)
                .ToListAsync();

            ViewBag.AvailableLabSessions = await _context.LabSessions
                .Include(s => s.Course)
                .Where(s => !registeredSessionIds.Contains(s.LabSessionId))
                .ToListAsync();

            ViewBag.Branches = await _context.Branch
                .Where(b => b.BranchStatus.ToLower() == "open")
                .ToListAsync();

            return View();
        }

        // Backs the lab-register modal's detail panel (fee/course) — same idea
        // as Course/Details and EntranceExam/EnrollInfo.
        [HttpGet]
        public async Task<IActionResult> LabSessionInfo(int labSessionId)
        {
            var session = await _context.LabSessions.Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.LabSessionId == labSessionId);
            if (session == null) return Json(new { success = false, message = "Session not found." });

            return Json(new
            {
                success = true,
                sessionName = session.SessionName,
                courseName = session.Course?.CourseName,
                fee = session.SessionFee ?? 0
            });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterLab(int labSessionId, int branchId, string paymentMethod)
        {
            var student = await CurrentStudentAsync();
            if (student == null) return RedirectToAction("login", "signin");

            var gate = await RequireEntranceApplicationAsync(student);
            if (gate != null) return gate;

            bool alreadyRegistered = await _context.StudentLabRegistrations
                .AnyAsync(l => l.StudentId == student.StudentId && l.LabSessionId == labSessionId);
            if (alreadyRegistered)
            {
                TempData["LabError"] = "Aap pehle hi is session ke liye register hain.";
                return RedirectToAction("Labsec");
            }

            var session = await _context.LabSessions.FirstOrDefaultAsync(s => s.LabSessionId == labSessionId);
            if (session == null)
            {
                TempData["LabError"] = "Session nahi mila. Dobara koshish karein.";
                return RedirectToAction("Labsec");
            }

            // Payment save karo — lab fee is student ke liye
            var payment = new Payment
            {
                StudentId = student.StudentId,
                Amount = session.SessionFee ?? 0,
                PaymentMethod = paymentMethod,
                PaymentType = "LabFee",
                PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                BranchId = branchId,
                PaymentStatus = "Paid"
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(); // PaymentId generate hoga

            // Registration create karo payment ke saath link karke
            _context.StudentLabRegistrations.Add(new StudentLabRegistration
            {
                StudentId = student.StudentId,
                LabSessionId = labSessionId,
                RegisterDate = DateOnly.FromDateTime(DateTime.Now),
                PaymentId = payment.PaymentId
            });
            await _context.SaveChangesAsync();

            TempData["LabSuccess"] = "Lab session ke liye registration aur payment kamyabi se ho gayi!";
            return RedirectToAction("Labsec");
        }

        public async Task<IActionResult> Payment()
        {
            var student = await CurrentStudentAsync();
            if (student == null) return RedirectToAction("login", "signin");
            var gate = await RequireEntranceApplicationAsync(student);
            if (gate != null) return gate;
            PushSidebarInfo(student);

            ViewBag.Payments = await _context.Payments
                .Where(p => p.StudentId == student.StudentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            ViewBag.TotalPaid = ((List<Payment>)ViewBag.Payments).Sum(p => p.Amount);
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var student = await CurrentStudentAsync();
            if (student == null) return RedirectToAction("login", "signin");
            var gate = await RequireEntranceApplicationAsync(student);
            if (gate != null) return gate;
            PushSidebarInfo(student);

            ViewBag.Branches = await _context.Branch.ToListAsync();
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string fullName, string phone, int? branchId)
        {
            var student = await CurrentStudentAsync();
            if (student == null) return RedirectToAction("login", "signin");
            var gate = await RequireEntranceApplicationAsync(student);
            if (gate != null) return gate;

            if (student.User != null)
            {
                student.User.FullName = fullName;
                student.User.Phone = phone;
            }
            student.BranchId = branchId;

            await _context.SaveChangesAsync();
            HttpContext.Session.SetString(SessionKeys.FullName, fullName);
            TempData["LoginSuccess"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }
    }
}
