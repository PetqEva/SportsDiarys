using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.Home;
using System.Threading.Tasks;

namespace SportsDiarys.Services.Implementations
{
    public class HomeDashboardService : IHomeDashboardService
    {
        private readonly AppDbContext _context;

        public HomeDashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HomeDashboardVm> GetDashboardAsync(string userId)
        {
            var vm = new HomeDashboardVm { IsAuthenticated = true };

            // Вземаме профила или го създаваме, ако не съществува
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.IdentityUserId == userId);

            if (profile == null)
            {
                profile = new UserProfile { IdentityUserId = userId };
                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            int profileId = profile.Id;
            vm.ProfileName = profile.Name;

            // Агрегати
            var diaryStats = await _context.TrainingDiaries
                .Where(d => d.UserProfileId == profileId)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    DiariesCount = g.Count(),
                    TotalDurationMinutes = g.Sum(x => (int?)x.DurationMinutes) ?? 0,
                    TotalWaterLiters = g.Sum(x => (double?)x.WaterLiters) ?? 0
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            vm.DiariesCount = diaryStats?.DiariesCount ?? 0;
            vm.TotalDurationMinutes = diaryStats?.TotalDurationMinutes ?? 0;
            vm.TotalWaterLiters = diaryStats?.TotalWaterLiters ?? 0;

            // EntriesCount
            vm.EntriesCount = await _context.TrainingEntries
                .CountAsync(e => e.TrainingDiary.UserProfileId == profileId);

            // Последни 5 дневника
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
                .AsNoTracking()
                .ToListAsync();

            return vm;
        }
    }
}