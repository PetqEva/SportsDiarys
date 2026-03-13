using Microsoft.EntityFrameworkCore;
using SportDiary.Services.Interfaces;
using SportDiary.ViewModels.Home;
using SportsDiarys.Data;
using SportsDiarys.Services.Interfaces;


namespace SportsDiarys.Services.Implementations
{
    public class HomeDashboardService : IHomeDashboardService
    {
        private readonly AppDbContext _context;
        private readonly IUserProfileService _profileService;

        public HomeDashboardService(AppDbContext context, IUserProfileService profileService)
        {
            _context = context;
            _profileService = profileService;
        }

        public async Task<HomeDashboardVm> BuildAsync(string userId)
        {
            var vm = new HomeDashboardVm
            {
                IsAuthenticated = true
            };

            var profile = await _profileService.GetMyProfileAsync(userId);

            if (profile == null)
                return vm;

            vm.ProfileName = profile.Name;


            // 1) Агрегати - 1 заявка
            var agg = await _context.TrainingDiaries
                .Where(d => d.UserProfileId == profile.Id)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    DiariesCount = g.Count(),
                    TotalDurationMinutes = g.Sum(x => (int?)x.DurationMinutes) ?? 0,
                    TotalWaterLiters = g.Sum(x => (double?)x.WaterLiters) ?? 0
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            vm.DiariesCount = agg?.DiariesCount ?? 0;
            vm.TotalDurationMinutes = agg?.TotalDurationMinutes ?? 0;
            vm.TotalWaterLiters = agg?.TotalWaterLiters ?? 0;

            // EntriesCount (ако искаш и това в 1 заявка - може, но така е ясно)
            vm.EntriesCount = await _context.TrainingEntries
                .Where(e => e.TrainingDiary.UserProfileId == profile.Id)
                .CountAsync();

            // 2) Recent - 1 заявка
            vm.RecentDiaries = await _context.TrainingDiaries
                .Where(d => d.UserProfileId == profile.Id)
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
                .AsNoTracking()
                .ToListAsync();

            return vm;
        }
    }
}
