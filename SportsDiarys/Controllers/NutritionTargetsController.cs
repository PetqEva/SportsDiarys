using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Interfaces;

namespace SportsDiarys.Controllers
{
    [Authorize]
    public class NutritionTargetsController : Controller
    {
        private readonly INutritionTargetService _nutritionTargetService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NutritionTargetsController(
            INutritionTargetService nutritionTargetService,
            UserManager<ApplicationUser> userManager)
        {
            _nutritionTargetService = nutritionTargetService;
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

            try
            {
                await _nutritionTargetService.SaveAsync(user.Id, calories, proteinGrams);
                TempData["SuccessMessage"] = "Целта е запазена успешно!";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Tdee", "Calculators");
        }
    }
}