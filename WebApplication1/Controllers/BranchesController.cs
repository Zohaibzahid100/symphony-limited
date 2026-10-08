using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BranchesController : Controller
    {
        private readonly SymphonyLimitedContext _context;

        public BranchesController(SymphonyLimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> branches()
        {
            var branches = await _context.Branch
                .Include(b => b.Students)
                .ToListAsync();

            ViewBag.Branches = branches;
            ViewBag.BranchCount = branches.Count(b => b.BranchStatus == "open");
            return View();
        }
    }
}
