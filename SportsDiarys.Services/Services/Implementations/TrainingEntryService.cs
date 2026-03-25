using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.Common;
using SportsDiarys.ViewModels.Shared;
using SportsDiarys.ViewModels.TrainingEntries;

namespace SportsDiarys.Services.Implementations
{
    public class TrainingEntryService : ITrainingEntryService
    {
        private readonly AppDbContext _context;

        public TrainingEntryService(AppDbContext context)
        {
            _context = context;
        }

        // ===================== VALIDATION =====================
        public IEnumerable<ValidationError> ValidateBusinessRules(TrainingEntryFormVm vm)
        {
            if (vm.DurationMinutes <= 0)
                yield return new ValidationError(nameof(vm.DurationMinutes), "Продължителността трябва да е положителна.");

            if (vm.Calories < 0)
                yield return new ValidationError(nameof(vm.Calories), "Калориите не могат да са отрицателни.");

            if (string.IsNullOrWhiteSpace(vm.SportName))
                yield return new ValidationError(nameof(vm.SportName), "Името е задължително.");
        }

        // ===================== CREATE =====================
        public async Task<int> CreateAsync(TrainingEntry entry, int userProfileId)
        {
            bool ownsDiary = await _context.TrainingDiaries
                .AnyAsync(d => d.Id == entry.TrainingDiaryId &&
                               d.UserProfileId == userProfileId);

            if (!ownsDiary)
                throw new UnauthorizedAccessException();

            entry.SportName = entry.SportName.Trim();

            await _context.TrainingEntries.AddAsync(entry);
            await _context.SaveChangesAsync();

            return entry.Id;
        }

        // ===================== UPDATE =====================
        public async Task<bool> UpdateAsync(TrainingEntry entry, int userProfileId)
        {
            var existing = await _context.TrainingEntries
                .Include(e => e.TrainingDiary)
                .FirstOrDefaultAsync(e => e.Id == entry.Id &&
                                          e.TrainingDiary.UserProfileId == userProfileId);

            if (existing == null)
                return false;

            existing.SportName = entry.SportName.Trim();
            existing.DurationMinutes = entry.DurationMinutes;
            existing.Calories = entry.Calories;
            existing.DistanceKm = entry.DistanceKm;

            await _context.SaveChangesAsync();
            return true;
        }

        // ===================== DELETE =====================
        public async Task<bool> DeleteAsync(int entryId, int userProfileId)
        {
            var entry = await _context.TrainingEntries
                .Include(e => e.TrainingDiary)
                .FirstOrDefaultAsync(e => e.Id == entryId &&
                                          e.TrainingDiary.UserProfileId == userProfileId);

            if (entry == null)
                return false;

            _context.TrainingEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return true;
        }

        // ===================== GET ALL =====================
        public async Task<List<TrainingEntry>> GetMyEntriesAsync(int userProfileId)
        {
            return await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .Include(e => e.TrainingEntryExercises)
                    .ThenInclude(te => te.Exercise)
                .Where(e => e.TrainingDiary.UserProfileId == userProfileId)
                .OrderByDescending(e => e.TrainingDiary.Date)
                .ToListAsync();
        }

        // ===================== PAGINATION =====================
        public async Task<PagedResultVm<TrainingEntryAllViewModel>> GetMyEntriesPagedAsync(
    int userProfileId,
    EntriesQueryVm query)
        {
            query.Page = Math.Max(1, query.Page);
            query.PageSize = Math.Clamp(query.PageSize, 5, 50);

            IQueryable<TrainingEntry> q = _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .Include(e => e.TrainingEntryExercises)
                    .ThenInclude(te => te.Exercise)
                .Where(e => e.TrainingDiary.UserProfileId == userProfileId);

            if (query.DiaryId.HasValue)
            {
                q = q.Where(e => e.TrainingDiaryId == query.DiaryId.Value);
            }

            if (query.From.HasValue)
            {
                var fromDate = query.From.Value.Date;
                q = q.Where(e => e.TrainingDiary.Date >= fromDate);
            }

            if (query.To.HasValue)
            {
                var toDateInclusive = query.To.Value.Date.AddDays(1);
                q = q.Where(e => e.TrainingDiary.Date < toDateInclusive);
            }

            if (!string.IsNullOrWhiteSpace(query.Sport))
            {
                var sport = query.Sport.Trim();
                q = q.Where(e => e.SportName.Contains(sport));
            }

            q = query.Sort switch
            {
                "date_asc" => q.OrderBy(e => e.TrainingDiary.Date),
                "calories_desc" => q.OrderByDescending(e => e.Calories).ThenByDescending(e => e.TrainingDiary.Date),
                "duration_desc" => q.OrderByDescending(e => e.DurationMinutes).ThenByDescending(e => e.TrainingDiary.Date),
                "distance_desc" => q.OrderByDescending(e => e.DistanceKm ?? 0).ThenByDescending(e => e.TrainingDiary.Date),
                _ => q.OrderByDescending(e => e.TrainingDiary.Date)
            };

            var total = await q.CountAsync();

            var items = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(e => new TrainingEntryAllViewModel
                {
                    Id = e.Id,
                    SportName = e.SportName,
                    DurationMinutes = e.DurationMinutes,
                    Calories = e.Calories,
                    DistanceKm = e.DistanceKm,
                    TrainingDiaryId = e.TrainingDiaryId,
                    DiaryLabel = e.TrainingDiary.Date.ToString("yyyy-MM-dd"),
                    ExerciseNames = e.TrainingEntryExercises
                        .Select(x => x.Exercise.Name)
                        .ToList()
                })
                .ToListAsync();

            return new PagedResultVm<TrainingEntryAllViewModel>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = total
            };
        }

        // ===================== DETAILS =====================
        public async Task<TrainingEntryDetailsViewModel?> GetMyEntryDetailsAsync(int entryId, int userProfileId)
        {
            return await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                    .ThenInclude(d => d.UserProfile)
                .Include(e => e.TrainingEntryExercises)
                    .ThenInclude(te => te.Exercise)
                .Where(e => e.Id == entryId &&
                            e.TrainingDiary.UserProfileId == userProfileId)
                .Select(e => new TrainingEntryDetailsViewModel
                {
                    Id = e.Id,
                    SportName = e.SportName,
                    DurationMinutes = e.DurationMinutes,
                    Calories = e.Calories,
                    DistanceKm = e.DistanceKm,
                    TrainingDiaryId = e.TrainingDiaryId,
                    DiaryLabel = e.TrainingDiary.Date.ToString("yyyy-MM-dd"),
                    DiaryDate = e.TrainingDiary.Date,
                    UserProfileId = e.TrainingDiary.UserProfileId,
                    UserName = e.TrainingDiary.UserProfile.Name,
                    ExerciseNames = e.TrainingEntryExercises
                        .Select(x => x.Exercise.Name)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        // ===================== EDIT =====================
        public async Task<TrainingEntry?> GetMyEntryForEditAsync(int entryId, int userProfileId)
        {
            return await _context.TrainingEntries
                .Include(e => e.TrainingDiary)
                .Include(e => e.TrainingEntryExercises)
                .FirstOrDefaultAsync(e => e.Id == entryId &&
                                          e.TrainingDiary.UserProfileId == userProfileId);
        }

        // ===================== DIARY =====================
        public async Task<bool> DiaryBelongsToMeAsync(int diaryId, int userProfileId)
        {
            return await _context.TrainingDiaries
                .AnyAsync(d => d.Id == diaryId &&
                               d.UserProfileId == userProfileId);
        }

        public async Task<List<(int Id, string Text)>> GetMyDiarySelectItemsAsync(int userProfileId)
        {
            return await _context.TrainingDiaries
                .Where(d => d.UserProfileId == userProfileId)
                .OrderByDescending(d => d.Date)
                .Select(d => new ValueTuple<int, string>(d.Id, d.Date.ToString("dd.MM.yyyy")))
                .ToListAsync();
        }

        // ===================== EXERCISES =====================
        public async Task<IEnumerable<(int Id, string Name)>> GetActiveExercisesAsync()
        {
            return await _context.Exercises
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .Select(e => new ValueTuple<int, string>(e.Id, e.Name))
                .ToListAsync();
        }

        public async Task<IEnumerable<EntryExerciseItemVm>> GetEntryExercisesAsync(int entryId, int userProfileId)
        {
            var isMine = await _context.TrainingEntries
                .AnyAsync(e => e.Id == entryId &&
                               e.TrainingDiary.UserProfileId == userProfileId);

            if (!isMine)
                return Enumerable.Empty<EntryExerciseItemVm>();

            return await _context.TrainingEntryExercises
                .Where(x => x.TrainingEntryId == entryId)
                .Select(x => new EntryExerciseItemVm
                {
                    ExerciseId = x.ExerciseId,
                    Name = x.Exercise.Name,
                    Sets = x.Sets,
                    Reps = x.Reps,
                    WeightKg = x.WeightKg,
                    DurationSeconds = x.DurationSeconds
                })
                .ToListAsync();
        }

        public async Task<bool> AddExerciseAsync(AddExerciseToEntryVm model, int userProfileId)
        {
            var exists = await _context.TrainingEntries
                .AnyAsync(e => e.Id == model.TrainingEntryId &&
                               e.TrainingDiary.UserProfileId == userProfileId);

            if (!exists)
                return false;

            if (!model.ExerciseId.HasValue)
                return false;

            var already = await _context.TrainingEntryExercises
                .AnyAsync(x => x.TrainingEntryId == model.TrainingEntryId &&
                               x.ExerciseId == model.ExerciseId.Value);

            if (already)
                return false;

            _context.TrainingEntryExercises.Add(new TrainingEntryExercise
            {
                TrainingEntryId = model.TrainingEntryId,
                ExerciseId = model.ExerciseId.Value,
                Sets = model.Sets,
                Reps = model.Reps,
                WeightKg = model.WeightKg,
                DurationSeconds = model.DurationSeconds
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveExerciseAsync(int entryId, int exerciseId, int userProfileId)
        {
            var entity = await _context.TrainingEntryExercises
                .FirstOrDefaultAsync(x => x.TrainingEntryId == entryId &&
                                         x.ExerciseId == exerciseId);

            if (entity == null)
                return false;

            _context.TrainingEntryExercises.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}