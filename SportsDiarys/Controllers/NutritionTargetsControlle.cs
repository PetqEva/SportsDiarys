using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;

namespace SportsDiarys.Controllers
{
    [Authorize]
    public class NutritionTargetsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NutritionTargetsController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(double calories, double proteinGrams)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var existing = await _context.NutritionTargets
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (existing == null)
            {
                var target = new NutritionTarget
                {
                    UserId = user.Id,
                    Calories = (int)Math.Round(calories),
                    ProteinGrams = (int)Math.Round(proteinGrams)
                };

                _context.NutritionTargets.Add(target);
            }
            else
            {
                existing.Calories = (int)Math.Round(calories);
                existing.ProteinGrams = (int)Math.Round(proteinGrams);
                existing.CreatedOn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Целта е запазена успешно!";
            return RedirectToAction("Tdee", "Calculators");
        }
    }
}