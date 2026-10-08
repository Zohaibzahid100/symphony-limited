using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ContactController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public ContactController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> contact()
        {
            ViewBag.Branches = await _context.Branch.ToListAsync();
            ViewBag.HeadOffice = ViewBag.Branches.Count > 0 ? ((List<Branch>)ViewBag.Branches)[0] : null;
            return View();
        }

        // Contact form submit — there's no ContactMessage table in the DB, so we
        // just acknowledge the message for now (no email/DB persistence yet).
        [HttpPost]
        public IActionResult Send(string name, string email, string subject, string message)
        {
            TempData["LoginSuccess"] = $"Thanks {name}, your message has been received. We'll get back to you shortly.";
            return RedirectToAction("contact");
        }
    }
}
