using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;
using SportsDiarys.Infrastructure;
using SportsDiarys.ViewModels.Admin;
using System.Security.Claims;

namespace SportsDiarys.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Administrator)]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardVm
            {
                UsersCount = await _userManager.Users.CountAsync(),
                DiariesCount = await _context.TrainingDiaries.CountAsync(),
                EntriesCount = await _context.TrainingEntries.CountAsync()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();

            var model = new List<UserAdminVm>();

            foreach (var user in users)
            {
                model.Add(new UserAdminVm
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    IsAdmin = await _userManager.IsInRoleAsync(user, Roles.Administrator)
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

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
        public async Task<IActionResult> RemoveAdmin(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // не позволявай да си махнеш Administrator на себе си
            if (userId == currentUserId)
            {
                TempData["AdminError"] = "Не можеш да махнеш Administrator роля на собствения си акаунт.";
                return RedirectToAction(nameof(Users));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

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

