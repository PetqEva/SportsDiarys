using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.Home;
using System.Security.Claims;

namespace SportsDiarys.Controllers
{
 	[AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IHomeDashboardService _dashboardService;

        public HomeController(IHomeDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            HomeDashboardVm vm;

            if (string.IsNullOrEmpty(userId))
            {
                vm = new HomeDashboardVm { IsAuthenticated = false };
            }
            else
            {
                vm = await _dashboardService.GetDashboardAsync(userId);
            }

            return View(vm);
        }

        public IActionResult StatusCodeError(int code)
        {
            ViewData["ErrorCode"] = code;
            return View();
        }

        public IActionResult Privacy() => View();
    }
}