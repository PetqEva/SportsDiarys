using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.Progress;

namespace SportsDiarys.Services.Implementations
{
    public class ProgressService : IProgressService
    {
        private readonly AppDbContext _context;

        public ProgressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProgressVm> GetMyProgressAsync(int userProfileId)
        {
            var today = DateTime.Today;
            var last7DaysFrom = today.AddDays(-6);
            var last30DaysFrom = today.AddDays(-29);

            var myDiaries = _context.TrainingDiaries
                .AsNoTracking()
                .Where(d => d.UserProfileId == userProfileId);

            var myEntries = _context.TrainingEntries
                .AsNoTracking()
                .Where(e => e.TrainingDiary.UserProfileId == userProfileId);

            var vm = new ProgressVm
            {
                TotalDiaries = await myDiaries.CountAsync(),
                TotalEntries = await myEntries.CountAsync()
            };

            vm.Last7DaysDurationMinutes = await myDiaries
                .Where(d => d.Date >= last7DaysFrom && d.Date <= today)
                .SumAsync(d => (int?)d.DurationMinutes) ?? 0;

            vm.Last30DaysDurationMinutes = await myDiaries
                .Where(d => d.Date >= last30DaysFrom && d.Date <= today)
                .SumAsync(d => (int?)d.DurationMinutes) ?? 0;

            vm.Last7DaysCalories = await myDiaries
                .Where(d => d.Date >= last7DaysFrom && d.Date <= today)
                .SumAsync(d => (int?)d.Calories) ?? 0;

            vm.Last30DaysCalories = await myDiaries
                .Where(d => d.Date >= last30DaysFrom && d.Date <= today)
                .SumAsync(d => (int?)d.Calories) ?? 0;

            vm.Last7DaysDistanceKm = await myDiaries
                .Where(d => d.Date >= last7DaysFrom && d.Date <= today)
                .SumAsync(d => (double?)d.DistanceKm) ?? 0;

            vm.Last30DaysDistanceKm = await myDiaries
                .Where(d => d.Date >= last30DaysFrom && d.Date <= today)
                .SumAsync(d => (double?)d.DistanceKm) ?? 0;

            vm.SportBreakdown = await myEntries
                .GroupBy(e => e.SportName)
                .Select(g => new SportBreakdownVm
                {
                    SportName = g.Key,
                    EntriesCount = g.Count(),
                    TotalDurationMinutes = g.Sum(x => x.DurationMinutes),
                    TotalCalories = g.Sum(x => x.Calories),
                    TotalDistanceKm = g.Sum(x => x.DistanceKm ?? 0)
                })
                .OrderByDescending(x => x.EntriesCount)
                .ThenBy(x => x.SportName)
                .ToListAsync();

            vm.MostActiveSport = vm.SportBreakdown.FirstOrDefault()?.SportName;

            var dailyData = await myDiaries
                .Where(d => d.Date >= last7DaysFrom && d.Date <= today)
                .OrderBy(d => d.Date)
                .Select(d => new
                {
                    Date = d.Date,
                    d.DurationMinutes,
                    d.Calories,
                    d.DistanceKm
                })
                .ToListAsync();

            vm.DailyProgress = Enumerable.Range(0, 7)
                .Select(offset => last7DaysFrom.AddDays(offset))
                .Select(date =>
                {
                    var dayItems = dailyData.Where(x => x.Date.Date == date.Date).ToList();

                    return new DailyProgressVm
                    {
                        DateLabel = date.ToString("dd.MM"),
                        DurationMinutes = dayItems.Sum(x => x.DurationMinutes),
                        Calories = dayItems.Sum(x => x.Calories),
                        DistanceKm = dayItems.Sum(x => x.DistanceKm)
                    };
                })
                .ToList();

            return vm;
        }
    }
}
