using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsDiarys.Models;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.Home;
using System.Diagnostics;
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