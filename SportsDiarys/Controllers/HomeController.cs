using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsDiarys.Data.Models;
using SportsDiarys.Models;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.Home;
using System.Diagnostics;
using System.Security.Claims;
using SportsDiarys.Infrastructure; 

namespace SportsDiarys.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IHomeDashboardService _dashboardService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            IHomeDashboardService dashboardService,
            UserManager<ApplicationUser> userManager)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            HomeDashboardVm vm;

            if (string.IsNullOrWhiteSpace(userId))
            {
                vm = new HomeDashboardVm
                {
                    IsAuthenticated = false
                };
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null && !await _userManager.IsInRoleAsync(user, Roles.User))
                {
                    await _userManager.AddToRoleAsync(user, Roles.User);
                }

                vm = await _dashboardService.GetDashboardAsync(userId);
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StatusCodeError(int code)
        {
            ViewData["ErrorCode"] = code;
            return View();
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(model);
        }
    }
}