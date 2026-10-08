using AdminDashbord.Helpers;
using AdminDashbord.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Controllers
{
    [AuthFilter]
    public class BranchesController : Controller
    {
        private readonly SymphonyLimitedContext _context;
        private readonly IWebHostEnvironment _env;
        private const string ImageFolder = "branches";

        public BranchesController(SymphonyLimitedContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Branches
        public async Task<IActionResult> Index()
        {
            var branches = await _context.Branches
                .OrderByDescending(b => b.BranchId)
                .ToListAsync();

            return View(branches);
        }

        // POST: /Branches/Save
        [HttpPost]
        public async Task<IActionResult> Save(
            int BranchId,
            string BranchName,
            string BranchCity,
            string BranchEmail,
            string? Address,
            string? BranchContact,
            string BranchStatus,
            IFormFile? BranchImageFile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BranchName) || string.IsNullOrWhiteSpace(BranchCity) || string.IsNullOrWhiteSpace(BranchEmail))
                    return Json(new { success = false, message = "Branch name, city and email are required." });

                if (BranchId == 0)
                {
                    if (BranchImageFile == null || BranchImageFile.Length == 0)
                        return Json(new { success = false, message = "Please choose a branch image." });

                    var savedName = await FileUploadHelper.SaveImageAsync(BranchImageFile, _env, ImageFolder);

                    var branch = new Branch
                    {
                        BranchName = BranchName,
                        BranchCity = BranchCity,
                        BranchEmail = BranchEmail,
                        Address = Address,
                        BranchContact = BranchContact,
                        BranchStatus = string.IsNullOrWhiteSpace(BranchStatus) ? "open" : BranchStatus,
                        BranchImg = savedName!
                    };

                    _context.Branches.Add(branch);
                }
                else
                {
                    var existing = await _context.Branches.FindAsync(BranchId);
                    if (existing == null)
                        return Json(new { success = false, message = "Branch not found." });

                    existing.BranchName = BranchName;
                    existing.BranchCity = BranchCity;
                    existing.BranchEmail = BranchEmail;
                    existing.Address = Address;
                    existing.BranchContact = BranchContact;
                    existing.BranchStatus = string.IsNullOrWhiteSpace(BranchStatus) ? "open" : BranchStatus;

                    if (BranchImageFile != null && BranchImageFile.Length > 0)
                    {
                        var savedName = await FileUploadHelper.SaveImageAsync(BranchImageFile, _env, ImageFolder);
                        FileUploadHelper.DeleteImage(existing.BranchImg, _env, ImageFolder);
                        existing.BranchImg = savedName!;
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "Branch email must be unique. Please use a different email." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Branches/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var branch = await _context.Branches.FindAsync(id);
                if (branch == null)
                    return Json(new { success = false, message = "Branch not found." });

                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();

                FileUploadHelper.DeleteImage(branch.BranchImg, _env, ImageFolder);

                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "This branch has students linked to it and cannot be deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
