using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;
using SportsDiarys.Infrastructure;
using SportsDiarys.ViewModels.Admin;

namespace SportsDiarys.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Administrator)]
    public class AdminController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            AppDbContext dbContext,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardVm
            {
                UsersCount = await _dbContext.Users.CountAsync(),
                DiariesCount = await _dbContext.TrainingDiaries.CountAsync(),
                EntriesCount = await _dbContext.TrainingEntries.CountAsync()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _dbContext.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var model = new List<UserAdminVm>();

            foreach (var user in users)
            {
                bool isAdministrator = await _userManager.IsInRoleAsync(user, Roles.Administrator);

                model.Add(new UserAdminVm
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    IsAdministrator = isAdministrator
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promote(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["AdminError"] = "Невалиден потребител.";
                return RedirectToAction(nameof(Users));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["AdminError"] = "Потребителят не е намерен.";
                return RedirectToAction(nameof(Users));
            }

            bool isAdministrator = await _userManager.IsInRoleAsync(user, Roles.Administrator);
            if (!isAdministrator)
            {
                var result = await _userManager.AddToRoleAsync(user, Roles.Administrator);

                if (!result.Succeeded)
                {
                    TempData["AdminError"] = string.Join("; ", result.Errors.Select(e => e.Description));
                }
                else
                {
                    TempData["AdminSuccess"] = "Потребителят беше направен администратор.";
                }
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Demote(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["AdminError"] = "Невалиден потребител.";
                return RedirectToAction(nameof(Users));
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id == id)
            {
                TempData["AdminError"] = "Не можеш да премахнеш собствената си администраторска роля.";
                return RedirectToAction(nameof(Users));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["AdminError"] = "Потребителят не е намерен.";
                return RedirectToAction(nameof(Users));
            }

            bool isAdministrator = await _userManager.IsInRoleAsync(user, Roles.Administrator);
            if (isAdministrator)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, Roles.Administrator);

                if (!result.Succeeded)
                {
                    TempData["AdminError"] = string.Join("; ", result.Errors.Select(e => e.Description));
                }
                else
                {
                    TempData["AdminSuccess"] = "Администраторските права бяха премахнати.";
                }
            }

            return RedirectToAction(nameof(Users));
        }
    }
}