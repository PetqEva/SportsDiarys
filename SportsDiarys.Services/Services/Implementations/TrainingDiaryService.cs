using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Models;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.TrainingDiaries;

namespace SportDiary.Services.Implementations
{
    public class TrainingDiaryService : ITrainingDiaryService
    {
        private readonly AppDbContext _context;

        // Централно allowed values (същите като в контролера)
        private static readonly HashSet<string> AllowedPlaces =
            new(StringComparer.OrdinalIgnoreCase) { "Home", "Gym", "Outdoor", "Other" };

        public TrainingDiaryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TrainingDiary>> GetMyDiariesAsync(int userProfileId)
        {
            return await _context.TrainingDiaries
                .Where(d => d.UserProfileId == userProfileId)
                .Include(d => d.TrainingEntries)
                .AsNoTracking()
                .OrderByDescending(d => d.Date)
                .ToListAsync();
        }


        public async Task<TrainingDiary?> GetMyDiaryDetailsAsync(int diaryId, int userProfileId)
        {
            return await _context.TrainingDiaries
                .AsNoTracking()
                .Include(d => d.TrainingEntries)
                .FirstOrDefaultAsync(d => d.Id == diaryId && d.UserProfileId == userProfileId);
        }

        public async Task<TrainingDiaryDetailsViewModel?> GetMyDiaryDetailsVmAsync(int diaryId, int userProfileId)
        {
            var diary = await _context.TrainingDiaries
                .AsNoTracking()
                .Include(d => d.UserProfile)
                .Include(d => d.TrainingEntries)
                .FirstOrDefaultAsync(d => d.Id == diaryId && d.UserProfileId == userProfileId);

            if (diary == null) return null;

            var entries = diary.TrainingEntries
                .OrderByDescending(e => e.Id)
                .Select(e => new TrainingDiaryDetailsViewModel.EntryItem
                {
                    Id = e.Id,
                    SportName = e.SportName,
                    DurationMinutes = e.DurationMinutes,
                    Calories = e.Calories,
                    DistanceKm = e.DistanceKm
                })
                .ToList();

            return new TrainingDiaryDetailsViewModel
            {
                Id = diary.Id,
                Date = diary.Date,
                Notes = diary.Notes,

                UserProfileId = diary.UserProfileId,
                UserName = diary.UserProfile?.Name ?? string.Empty,

                Entries = entries,

                TotalEntries = entries.Count,
                TotalDurationMinutes = entries.Sum(x => x.DurationMinutes),
                TotalCalories = entries.Sum(x => x.Calories),
                TotalDistanceKm = Math.Round(entries.Sum(x => x.DistanceKm ?? 0), 2)
            };
        }

        public async Task<TrainingDiary?> GetMyDiaryForEditAsync(int diaryId, int userProfileId)
        {
            return await _context.TrainingDiaries
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == diaryId && d.UserProfileId == userProfileId);
        }

        public async Task<bool> DiaryExistsForDateAsync(int userProfileId, DateTime date, int? excludeDiaryId = null)
        {
            var targetDate = date.Date;

            return await _context.TrainingDiaries
                .AsNoTracking()
                .AnyAsync(d =>
                    d.UserProfileId == userProfileId &&
                    d.Date.Date == targetDate &&
                    (!excludeDiaryId.HasValue || d.Id != excludeDiaryId.Value));
        }

        // ✅ Ownership enforced here (do NOT trust incoming diary.UserProfileId)
        public async Task<int> CreateAsync(TrainingDiary diary, int userProfileId)
        {
            if (diary == null) throw new ArgumentNullException(nameof(diary));

            // enforce owner
            diary.UserProfileId = userProfileId;

            // optional hardening: validate Place
            if (!AllowedPlaces.Contains(diary.Place))
                throw new ArgumentException("Invalid Place value.", nameof(diary.Place));

            _context.TrainingDiaries.Add(diary);
            await _context.SaveChangesAsync();

            return diary.Id;
        }

        public async Task<bool> UpdateAsync(
            int diaryId,
            int userProfileId,
            DateTime date,
            int durationMinutes,
            string place,
            double waterLiters,
            string? notes)
        {
            var diary = await _context.TrainingDiaries
                .FirstOrDefaultAsync(d => d.Id == diaryId && d.UserProfileId == userProfileId);

            if (diary == null) return false;

            // optional hardening: validate Place
            if (!AllowedPlaces.Contains(place))
                return false;

            diary.Date = date;
            diary.DurationMinutes = durationMinutes;
            diary.Place = place;
            diary.WaterLiters = waterLiters;
            diary.Notes = notes;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int diaryId, int userProfileId)
        {
            var diary = await _context.TrainingDiaries
                .Include(d => d.TrainingEntries)
                .FirstOrDefaultAsync(d => d.Id == diaryId && d.UserProfileId == userProfileId);

            if (diary == null) return false;

            _context.TrainingDiaries.Remove(diary);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

