using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.ViewModels.Home;
using System.Security.Claims;

namespace SportsDiarys.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeDashboardVm();

            vm.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;

            if (!vm.IsAuthenticated)
                return View(vm);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.IdentityUserId == userId);

            if (profile == null)
            {
                // Опционално: обработка ако няма профил
                return RedirectToAction("Index", "Home");
            }

            int profileId = profile.Id;

            vm.DiariesCount = await _context.TrainingDiaries
                .Where(d => d.UserProfileId == profileId)
                .CountAsync();

            vm.EntriesCount = await _context.TrainingEntries
                .Where(e => e.TrainingDiary.UserProfileId == profileId)
                .CountAsync();

            vm.TotalDurationMinutes = await _context.TrainingDiaries
                .Where(d => d.UserProfileId == profileId)
                .SumAsync(d => (int?)d.DurationMinutes) ?? 0;

            vm.TotalWaterLiters = await _context.TrainingDiaries
                .Where(d => d.UserProfileId == profileId)
                .SumAsync(d => (double?)d.WaterLiters) ?? 0;

            vm.RecentDiaries = await _context.TrainingDiaries
                .Where(d => d.UserProfileId == profileId)
                .OrderByDescending(d => d.Date)
                .Take(5)
                .Select(d => new HomeDashboardVm.RecentDiaryVm
                {
                    Id = d.Id,
                    Date = d.Date,
                    Place = d.Place,
                    DurationMinutes = d.DurationMinutes,
                    WaterLiters = d.WaterLiters,
                    Notes = d.Notes
                })
                .ToListAsync();

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}