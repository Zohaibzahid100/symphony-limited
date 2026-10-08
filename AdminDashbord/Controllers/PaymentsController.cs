using AdminDashbord.Models;
using AdminDashbord.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class PaymentsController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public PaymentsController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var payments = await _context.Payments
                .Include(p => p.Student).ThenInclude(s => s.User)
                .OrderByDescending(p => p.PaymentId)
                .ToListAsync();

            ViewBag.Students = await _context.Students.Include(s => s.User).OrderBy(s => s.RollNumber).ToListAsync();

            ViewBag.TotalCollected = payments.Sum(p => p.Amount);
            ViewBag.CourseFees = payments.Where(p => p.PaymentType == "CourseFee").Sum(p => p.Amount);
            ViewBag.EntranceFees = payments.Where(p => p.PaymentType == "EntranceFee").Sum(p => p.Amount);
            ViewBag.LabFees = payments.Where(p => p.PaymentType == "LabFee").Sum(p => p.Amount);

            return View(payments);
        }

        [HttpPost]
        public async Task<IActionResult> Save(Payment model)
        {
            try
            {
                if (model.StudentId == 0 || model.Amount <= 0)
                    return Json(new { success = false, message = "Student and a valid amount are required." });

                if (model.PaymentId == 0)
                {
                    model.PaymentDate ??= DateOnly.FromDateTime(DateTime.Now);
                    _context.Payments.Add(model);
                }
                else
                {
                    var existing = await _context.Payments.FindAsync(model.PaymentId);
                    if (existing == null)
                        return Json(new { success = false, message = "Payment not found." });

                    existing.StudentId = model.StudentId;
                    existing.Amount = model.Amount;
                    existing.PaymentMethod = model.PaymentMethod;
                    existing.PaymentType = model.PaymentType;
                    existing.ReferenceNumber = model.ReferenceNumber;
                    existing.PaymentDate = model.PaymentDate;
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
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                    return Json(new { success = false, message = "Payment not found." });

                _context.Payments.Remove(payment);
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
