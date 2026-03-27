using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.TrainingDiaries;

namespace SportsDiarys.Services.Implementations
{
    public class TrainingDiaryService : ITrainingDiaryService
    {
        private readonly AppDbContext _context;

        public TrainingDiaryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TrainingDiaryListVm> GetMyDiariesAsync(int userProfileId, string? search, int page, int pageSize)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            IQueryable<TrainingDiary> query = _context.TrainingDiaries
                .AsNoTracking()
                .Where(d => d.UserProfileId == userProfileId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string normalizedSearch = search.Trim();

                query = query.Where(d =>
                    (d.Name != null && d.Name.Contains(normalizedSearch)) ||
                    (d.Notes != null && d.Notes.Contains(normalizedSearch)) ||
                    (d.Place != null && d.Place.Contains(normalizedSearch)));
            }

            int totalCount = await query.CountAsync();

            List<TrainingDiaryListItemVm> diaries = await query
                .OrderByDescending(d => d.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new TrainingDiaryListItemVm
                {
                    Id = d.Id,
                    Date = d.Date,
                    Place = d.Place,
                    DurationMinutes = d.DurationMinutes,
                    WaterLiters = d.WaterLiters,
                    Notes = d.Notes
                })
                .ToListAsync();

            return new TrainingDiaryListVm
            {
                Items = diaries,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Search = search
            };
        }

        public async Task<TrainingDiaryDetailsViewModel?> GetByIdAsync(int diaryId, int userProfileId)
        {
            TrainingDiary? diary = await _context.TrainingDiaries
                .Include(d => d.UserProfile)
                .Include(d => d.TrainingEntries)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == diaryId && d.UserProfileId == userProfileId);

            if (diary == null)
            {
                return null;
            }

            return MapToDetailsViewModel(diary);
        }

        public async Task<EditTrainingDiaryViewModel?> GetForEditAsync(int diaryId, int userProfileId)
        {
            TrainingDiary? diary = await _context.TrainingDiaries
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == diaryId && d.UserProfileId == userProfileId);

            if (diary == null)
            {
                return null;
            }

            return new EditTrainingDiaryViewModel
            {
                Id = diary.Id,
                Date = diary.Date,
                Notes = diary.Notes ?? string.Empty,
                Calories = diary.Calories,
                DurationMinutes = diary.DurationMinutes,
                DistanceKm = diary.DistanceKm,
                WaterLiters = diary.WaterLiters,
                Place = diary.Place ?? string.Empty
            };
        }

        public async Task<bool> ExistsForDateAsync(int userProfileId, DateTime date, int? excludeDiaryId = null)
        {
            IQueryable<TrainingDiary> query = _context.TrainingDiaries
                .AsNoTracking()
                .Where(d => d.UserProfileId == userProfileId && d.Date.Date == date.Date);

            if (excludeDiaryId.HasValue)
            {
                query = query.Where(d => d.Id != excludeDiaryId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<int> CreateAsync(CreateTrainingDiaryViewModel model, int userProfileId)
        {
            TrainingDiary diary = new TrainingDiary
            {
                Name = $"Дневник {model.Date:dd.MM.yyyy}",
                Date = model.Date,
                Notes = model.Notes ?? string.Empty,
                UserProfileId = userProfileId,
                Calories = model.Calories,
                DurationMinutes = model.DurationMinutes,
                DistanceKm = model.DistanceKm,
                WaterLiters = model.WaterLiters,
                Place = model.Place ?? string.Empty
            };

            await _context.TrainingDiaries.AddAsync(diary);
            await _context.SaveChangesAsync();

            return diary.Id;
        }

        public async Task<bool> UpdateAsync(UpdateTrainingDiaryViewModel model, int userProfileId)
        {
            TrainingDiary? diary = await _context.TrainingDiaries
                .FirstOrDefaultAsync(d => d.Id == model.Id && d.UserProfileId == userProfileId);

            if (diary == null)
            {
                return false;
            }

            diary.Name = $"Дневник {model.Date:dd.MM.yyyy}";
            diary.Date = model.Date;
            diary.Notes = model.Notes ?? string.Empty;
            diary.Calories = model.Calories;
            diary.DurationMinutes = model.DurationMinutes;
            diary.DistanceKm = model.DistanceKm;
            diary.WaterLiters = model.WaterLiters;
            diary.Place = model.Place ?? string.Empty;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int diaryId, int userProfileId)
        {
            TrainingDiary? diary = await _context.TrainingDiaries
                .FirstOrDefaultAsync(d => d.Id == diaryId && d.UserProfileId == userProfileId);

            if (diary == null)
            {
                return false;
            }

            _context.TrainingDiaries.Remove(diary);
            await _context.SaveChangesAsync();

            return true;
        }

        private static TrainingDiaryDetailsViewModel MapToDetailsViewModel(TrainingDiary diary)
        {
            List<TrainingDiaryDetailsViewModel.EntryItem> entries = diary.TrainingEntries
                .Select(e => new TrainingDiaryDetailsViewModel.EntryItem
                {
                    Id = e.Id,
                    SportName = e.SportName ?? string.Empty,
                    DurationMinutes = e.DurationMinutes,
                    Calories = e.Calories,
                    DistanceKm = e.DistanceKm ?? 0.0
                })
                .ToList();

            return new TrainingDiaryDetailsViewModel
            {
                Id = diary.Id,
                Date = diary.Date,
                Notes = diary.Notes ?? string.Empty,
                UserProfileId = diary.UserProfileId,
                UserName = diary.UserProfile?.Name ?? string.Empty,
                Entries = entries,
                TotalEntries = entries.Count,
                TotalCalories = entries.Sum(x => x.Calories),
                TotalDurationMinutes = entries.Sum(x => x.DurationMinutes),
                TotalDistanceKm = entries.Sum(x => x.DistanceKm)
            };
        }
    }
}