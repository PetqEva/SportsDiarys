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

        public IEnumerable<ValidationError> ValidateBusinessRules(TrainingEntryFormVm vm)
        {
            if (string.IsNullOrWhiteSpace(vm.SportName))
            {
                yield return new ValidationError(
                    nameof(vm.SportName),
                    "Името на спорта е задължително.");
            }

            if (vm.DurationMinutes <= 0)
            {
                yield return new ValidationError(
                    nameof(vm.DurationMinutes),
                    "Продължителността трябва да е положителна.");
            }

            if (vm.Calories < 0)
            {
                yield return new ValidationError(
                    nameof(vm.Calories),
                    "Калориите не могат да са отрицателни.");
            }

            if (vm.DistanceKm.HasValue && vm.DistanceKm.Value < 0)
            {
                yield return new ValidationError(
                    nameof(vm.DistanceKm),
                    "Дистанцията не може да е отрицателна.");
            }

            if (vm.TrainingDiaryId <= 0)
            {
                yield return new ValidationError(
                    nameof(vm.TrainingDiaryId),
                    "Избери дневник.");
            }
        }

        public async Task<int> CreateAsync(TrainingEntry entry, int userProfileId)
        {
            ArgumentNullException.ThrowIfNull(entry);

            bool ownsDiary = await _context.TrainingDiaries
                .AnyAsync(d => d.Id == entry.TrainingDiaryId &&
                               d.UserProfileId == userProfileId);

            if (!ownsDiary)
            {
                throw new UnauthorizedAccessException("Нямаш достъп до този дневник.");
            }

            entry.SportName = entry.SportName.Trim();

            await _context.TrainingEntries.AddAsync(entry);
            await _context.SaveChangesAsync();

            return entry.Id;
        }

        public async Task<bool> UpdateAsync(TrainingEntry entry, int userProfileId)
        {
            ArgumentNullException.ThrowIfNull(entry);

            var existingEntry = await _context.TrainingEntries
                .Include(e => e.TrainingDiary)
                .FirstOrDefaultAsync(e =>
                    e.Id == entry.Id &&
                    e.TrainingDiary.UserProfileId == userProfileId);

            if (existingEntry == null)
            {
                return false;
            }

            bool targetDiaryIsMine = await _context.TrainingDiaries
                .AnyAsync(d => d.Id == entry.TrainingDiaryId &&
                               d.UserProfileId == userProfileId);

            if (!targetDiaryIsMine)
            {
                return false;
            }

            existingEntry.SportName = entry.SportName.Trim();
            existingEntry.DurationMinutes = entry.DurationMinutes;
            existingEntry.Calories = entry.Calories;
            existingEntry.DistanceKm = entry.DistanceKm;
            existingEntry.TrainingDiaryId = entry.TrainingDiaryId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int entryId, int userProfileId)
        {
            var entry = await _context.TrainingEntries
                .Include(e => e.TrainingDiary)
                .Include(e => e.TrainingEntryExercises)
                .FirstOrDefaultAsync(e =>
                    e.Id == entryId &&
                    e.TrainingDiary.UserProfileId == userProfileId);

            if (entry == null)
            {
                return false;
            }

            if (entry.TrainingEntryExercises.Any())
            {
                _context.TrainingEntryExercises.RemoveRange(entry.TrainingEntryExercises);
            }

            _context.TrainingEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<TrainingEntry>> GetMyEntriesAsync(int userProfileId)
        {
            return await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .Include(e => e.TrainingEntryExercises)
                    .ThenInclude(te => te.Exercise)
                .Where(e => e.TrainingDiary.UserProfileId == userProfileId)
                .OrderByDescending(e => e.TrainingDiary.Date)
                .ThenByDescending(e => e.Id)
                .ToListAsync();
        }

        public async Task<PagedResultVm<TrainingEntryAllViewModel>> GetMyEntriesPagedAsync(
            int userProfileId,
            EntriesQueryVm query)
        {
            query ??= new EntriesQueryVm();

            query.Page = Math.Max(1, query.Page);
            query.PageSize = Math.Clamp(query.PageSize, 5, 50);

            IQueryable<TrainingEntry> entriesQuery = _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .Include(e => e.TrainingEntryExercises)
                    .ThenInclude(te => te.Exercise)
                .Where(e => e.TrainingDiary.UserProfileId == userProfileId);

            if (query.DiaryId.HasValue)
            {
                entriesQuery = entriesQuery.Where(e => e.TrainingDiaryId == query.DiaryId.Value);
            }

            if (query.From.HasValue)
            {
                DateTime fromDate = query.From.Value.Date;
                entriesQuery = entriesQuery.Where(e => e.TrainingDiary.Date >= fromDate);
            }

            if (query.To.HasValue)
            {
                DateTime toExclusive = query.To.Value.Date.AddDays(1);
                entriesQuery = entriesQuery.Where(e => e.TrainingDiary.Date < toExclusive);
            }

            if (!string.IsNullOrWhiteSpace(query.Sport))
            {
                string sport = query.Sport.Trim();
                entriesQuery = entriesQuery.Where(e => e.SportName.Contains(sport));
            }

            entriesQuery = query.Sort switch
            {
                "date_asc" => entriesQuery
                    .OrderBy(e => e.TrainingDiary.Date)
                    .ThenBy(e => e.Id),

                "calories_desc" => entriesQuery
                    .OrderByDescending(e => e.Calories)
                    .ThenByDescending(e => e.TrainingDiary.Date)
                    .ThenByDescending(e => e.Id),

                "duration_desc" => entriesQuery
                    .OrderByDescending(e => e.DurationMinutes)
                    .ThenByDescending(e => e.TrainingDiary.Date)
                    .ThenByDescending(e => e.Id),

                "distance_desc" => entriesQuery
                    .OrderByDescending(e => e.DistanceKm ?? 0)
                    .ThenByDescending(e => e.TrainingDiary.Date)
                    .ThenByDescending(e => e.Id),

                _ => entriesQuery
                    .OrderByDescending(e => e.TrainingDiary.Date)
                    .ThenByDescending(e => e.Id)
            };

            int totalCount = await entriesQuery.CountAsync();

            var dbItems = await entriesQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var items = dbItems
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
                        .Where(x => x.Exercise != null)
                        .Select(x => x.Exercise.Name)
                        .OrderBy(x => x)
                        .ToList()
                })
                .ToList();

            return new PagedResultVm<TrainingEntryAllViewModel>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<TrainingEntryDetailsViewModel?> GetMyEntryDetailsAsync(int entryId, int userProfileId)
        {
            var entry = await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                    .ThenInclude(d => d.UserProfile)
                .Include(e => e.TrainingEntryExercises)
                    .ThenInclude(te => te.Exercise)
                .FirstOrDefaultAsync(e =>
                    e.Id == entryId &&
                    e.TrainingDiary.UserProfileId == userProfileId);

            if (entry == null)
            {
                return null;
            }

            return new TrainingEntryDetailsViewModel
            {
                Id = entry.Id,
                SportName = entry.SportName,
                DurationMinutes = entry.DurationMinutes,
                Calories = entry.Calories,
                DistanceKm = entry.DistanceKm,
                TrainingDiaryId = entry.TrainingDiaryId,
                DiaryLabel = entry.TrainingDiary.Date.ToString("yyyy-MM-dd"),
                DiaryDate = entry.TrainingDiary.Date,
                UserProfileId = entry.TrainingDiary.UserProfileId,
                UserName = entry.TrainingDiary.UserProfile?.Name ?? string.Empty,
                ExerciseNames = entry.TrainingEntryExercises
                    .Where(x => x.Exercise != null)
                    .Select(x => x.Exercise.Name)
                    .OrderBy(x => x)
                    .ToList()
            };
        }

        public async Task<TrainingEntry?> GetMyEntryForEditAsync(int entryId, int userProfileId)
        {
            return await _context.TrainingEntries
                .Include(e => e.TrainingDiary)
                .Include(e => e.TrainingEntryExercises)
                .ThenInclude(te => te.Exercise)
                .FirstOrDefaultAsync(e =>
                    e.Id == entryId &&
                    e.TrainingDiary.UserProfileId == userProfileId);
        }

        public async Task<bool> DiaryBelongsToMeAsync(int diaryId, int userProfileId)
        {
            return await _context.TrainingDiaries
                .AnyAsync(d => d.Id == diaryId &&
                               d.UserProfileId == userProfileId);
        }

        public async Task<List<(int Id, string Text)>> GetMyDiarySelectItemsAsync(int userProfileId)
        {
            var diaries = await _context.TrainingDiaries
                .AsNoTracking()
                .Where(d => d.UserProfileId == userProfileId)
                .OrderByDescending(d => d.Date)
                .ThenByDescending(d => d.Id)
                .ToListAsync();

            return diaries
                .Select(d => (d.Id, d.Date.ToString("dd.MM.yyyy")))
                .ToList();
        }

        public async Task<IEnumerable<(int Id, string Name)>> GetActiveExercisesAsync()
        {
            return await _context.Exercises
                .AsNoTracking()
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .Select(e => new ValueTuple<int, string>(e.Id, e.Name))
                .ToListAsync();
        }

        public async Task<IEnumerable<EntryExerciseItemVm>> GetEntryExercisesAsync(int entryId, int userProfileId)
        {
            bool isMine = await _context.TrainingEntries
                .AnyAsync(e =>
                    e.Id == entryId &&
                    e.TrainingDiary.UserProfileId == userProfileId);

            if (!isMine)
            {
                return Enumerable.Empty<EntryExerciseItemVm>();
            }

            return await _context.TrainingEntryExercises
                .AsNoTracking()
                .Where(x => x.TrainingEntryId == entryId)
                .Include(x => x.Exercise)
                .OrderBy(x => x.Exercise.Name)
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
            ArgumentNullException.ThrowIfNull(model);

            if (!model.ExerciseId.HasValue)
            {
                return false;
            }

            bool entryExistsAndIsMine = await _context.TrainingEntries
                .AnyAsync(e =>
                    e.Id == model.TrainingEntryId &&
                    e.TrainingDiary.UserProfileId == userProfileId);

            if (!entryExistsAndIsMine)
            {
                return false;
            }

            bool exerciseExistsAndIsActive = await _context.Exercises
                .AnyAsync(e =>
                    e.Id == model.ExerciseId.Value &&
                    e.IsActive);

            if (!exerciseExistsAndIsActive)
            {
                return false;
            }

            bool alreadyAdded = await _context.TrainingEntryExercises
                .AnyAsync(x =>
                    x.TrainingEntryId == model.TrainingEntryId &&
                    x.ExerciseId == model.ExerciseId.Value);

            if (alreadyAdded)
            {
                return false;
            }

            var relation = new TrainingEntryExercise
            {
                TrainingEntryId = model.TrainingEntryId,
                ExerciseId = model.ExerciseId.Value,
                Sets = model.Sets,
                Reps = model.Reps,
                WeightKg = model.WeightKg,
                DurationSeconds = model.DurationSeconds
            };

            await _context.TrainingEntryExercises.AddAsync(relation);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveExerciseAsync(int entryId, int exerciseId, int userProfileId)
        {
            var relation = await _context.TrainingEntryExercises
                .Include(x => x.TrainingEntry)
                    .ThenInclude(e => e.TrainingDiary)
                .FirstOrDefaultAsync(x =>
                    x.TrainingEntryId == entryId &&
                    x.ExerciseId == exerciseId &&
                    x.TrainingEntry.TrainingDiary.UserProfileId == userProfileId);

            if (relation == null)
            {
                return false;
            }

            _context.TrainingEntryExercises.Remove(relation);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}