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
                model.Add(new UserAdminVm
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    IsAdministrator = await _userManager.IsInRoleAsync(user, Roles.Administrator)
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promote(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!await _userManager.IsInRoleAsync(user, Roles.Administrator))
            {
                var result = await _userManager.AddToRoleAsync(user, Roles.Administrator);
                if (!result.Succeeded)
                {
                    TempData["AdminError"] = string.Join("; ", result.Errors.Select(e => e.Description));
                }
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Demote(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(user, Roles.Administrator))
            {
                var result = await _userManager.RemoveFromRoleAsync(user, Roles.Administrator);
                if (!result.Succeeded)
                {
                    TempData["AdminError"] = string.Join("; ", result.Errors.Select(e => e.Description));
                }
            }

            return RedirectToAction(nameof(Users));
        }
    }
}