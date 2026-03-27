using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.Home;

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

            // ===================== PROFILE =====================
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.IdentityUserId == userId);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    IdentityUserId = userId,
                    Name = "New User"
                };

                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            int profileId = profile.Id;
            vm.ProfileName = profile.Name ?? "";

            // ===================== STATS (FIXED) =====================
            var diaries = await _context.TrainingDiaries
                .Where(d => d.UserProfileId == profileId)
                .AsNoTracking()
                .ToListAsync();

            vm.DiariesCount = diaries.Count;

            vm.TotalDurationMinutes = diaries.Sum(d => d.DurationMinutes);

            vm.TotalWaterLiters = diaries.Sum(d => d.WaterLiters);

            // ===================== ENTRIES COUNT =====================
            vm.EntriesCount = await _context.TrainingEntries
                .CountAsync(e => e.TrainingDiary.UserProfileId == profileId);

            // ===================== RECENT DIARIES =====================
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
                    Notes = d.Notes ?? ""
                })
                .AsNoTracking()
                .ToListAsync();

            return vm;
        }
    }
}