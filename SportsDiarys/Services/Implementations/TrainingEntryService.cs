using Microsoft.EntityFrameworkCore;
using SportsDiarys.ViewModels.TrainingEntries;
using SportsDiarys.Data;
using SportsDiarys.Models;
using SportDiary.ViewModels.TrainingEntries;
using SportDiary.ViewModels.Shared;
using SportDiary.Services.Interfaces;


namespace SportDiary.Services.Implementations
{
    public class TrainingEntryService : ITrainingEntryService
    {
        private readonly AppDbContext _context;

        public TrainingEntryService(AppDbContext context)
        {
            _context = context;
        }

        // READ: всички мои записи
        public async Task<List<TrainingEntry>> GetMyEntriesAsync(int userProfileId)
        {
            return await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .Where(e => e.TrainingDiary != null && e.TrainingDiary.UserProfileId == userProfileId)
                .OrderByDescending(e => e.TrainingDiary!.Date)
                .ThenByDescending(e => e.Id)
                .ToListAsync();
        }

        // READ: paged + filtering
        public async Task<PagedResultVm<TrainingEntryAllViewModel>> GetMyEntriesPagedAsync(int userProfileId, EntriesQueryVm query)
        {
            query ??= new EntriesQueryVm();
            if (query.Page < 1) query.Page = 1;
            if (query.PageSize < 5) query.PageSize = 5;
            if (query.PageSize > 50) query.PageSize = 50;

            var q = _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .Where(e => e.TrainingDiary != null && e.TrainingDiary.UserProfileId == userProfileId);

            if (query.DiaryId.HasValue)
                q = q.Where(e => e.TrainingDiaryId == query.DiaryId.Value);

            if (!string.IsNullOrWhiteSpace(query.Sport))
            {
                var sport = query.Sport.Trim();
                q = q.Where(e => e.SportName.Contains(sport));
            }

            if (query.From.HasValue)
            {
                var fromDate = query.From.Value.Date;
                q = q.Where(e => e.TrainingDiary!.Date.Date >= fromDate);
            }

            if (query.To.HasValue)
            {
                var toDate = query.To.Value.Date;
                q = q.Where(e => e.TrainingDiary!.Date.Date <= toDate);
            }

            q = query.Sort switch
            {
                "calories_desc" => q.OrderByDescending(e => e.Calories).ThenByDescending(e => e.Id),
                "duration_desc" => q.OrderByDescending(e => e.DurationMinutes).ThenByDescending(e => e.Id),
                "distance_desc" => q.OrderByDescending(e => e.DistanceKm ?? 0).ThenByDescending(e => e.Id),
                "date_asc" => q.OrderBy(e => e.TrainingDiary!.Date).ThenBy(e => e.Id),
                _ => q.OrderByDescending(e => e.TrainingDiary!.Date).ThenByDescending(e => e.Id)
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
                    DiaryLabel = e.TrainingDiary != null ? e.TrainingDiary.Date.ToString("yyyy-MM-dd") : string.Empty
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

        // Business validation (не DB)
        public IEnumerable<(string field, string message)> ValidateBusinessRules(TrainingEntryFormVm vm)
        {
            if (vm.DurationMinutes <= 0)
                yield return (nameof(vm.DurationMinutes), "Продължителността трябва да е положителна.");

            if (vm.DurationMinutes > 600)
                yield return (nameof(vm.DurationMinutes), "Продължителността е нереалистично голяма.");

            if (vm.Calories > vm.DurationMinutes * 40)
                yield return (nameof(vm.Calories), "Калориите са нереалистично високи спрямо продължителността.");

            if (vm.Calories < 0)
                yield return (nameof(vm.Calories), "Калориите не могат да са отрицателни.");

            if (vm.DistanceKm.HasValue)
            {
                if (vm.DistanceKm < 0)
                    yield return (nameof(vm.DistanceKm), "Разстоянието не може да е отрицателно.");

                if (vm.DistanceKm > 200)
                    yield return (nameof(vm.DistanceKm), "Разстоянието е нереалистично голямо.");
            }

            if (string.IsNullOrWhiteSpace(vm.SportName))
                yield return (nameof(vm.SportName), "Името на спорта е задължително.");

            if (!string.IsNullOrWhiteSpace(vm.SportName) && vm.SportName.Length < 3)
                yield return (nameof(vm.SportName), "Името трябва да съдържа поне 3 символа.");
        }

        // READ: Details => ViewModel (НЕ entity)
        public async Task<TrainingEntryDetailsViewModel?> GetMyEntryDetailsAsync(int entryId, int userProfileId)
        {
            return await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                    .ThenInclude(d => d.UserProfile) // ако имаш навигация
                .Where(e =>
                    e.Id == entryId &&
                    e.TrainingDiary != null &&
                    e.TrainingDiary.UserProfileId == userProfileId)
                .Select(e => new TrainingEntryDetailsViewModel
                {
                    Id = e.Id,
                    SportName = e.SportName,
                    DurationMinutes = e.DurationMinutes,
                    Calories = e.Calories,
                    DistanceKm = e.DistanceKm,

                    TrainingDiaryId = e.TrainingDiaryId,
                    DiaryLabel = e.TrainingDiary != null
                        ? e.TrainingDiary.Date.ToString("yyyy-MM-dd")
                        : string.Empty,

                    UserProfileId = e.TrainingDiary!.UserProfileId,
                    UserName = e.TrainingDiary.UserProfile != null ? e.TrainingDiary.UserProfile.Name : string.Empty,

                    DiaryDate = e.TrainingDiary!.Date
                })
                .FirstOrDefaultAsync();
        }

        // READ: за Edit (entity)
        public async Task<TrainingEntry?> GetMyEntryForEditAsync(int entryId, int userProfileId)
        {
            return await _context.TrainingEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.Id == entryId &&
                    _context.TrainingDiaries.Any(d =>
                        d.Id == e.TrainingDiaryId &&
                        d.UserProfileId == userProfileId));
        }

        // Security helper
        public async Task<bool> DiaryBelongsToMeAsync(int diaryId, int userProfileId)
        {
            return await _context.TrainingDiaries
                .AsNoTracking()
                .AnyAsync(d => d.Id == diaryId && d.UserProfileId == userProfileId);
        }

        // CREATE
        public async Task<int> CreateAsync(TrainingEntry entry, int userProfileId)
        {
            var diaryIsMine = await DiaryBelongsToMeAsync(entry.TrainingDiaryId, userProfileId);
            if (!diaryIsMine)
                throw new UnauthorizedAccessException("Diary does not belong to the current user.");

            entry.SportName = (entry.SportName ?? string.Empty).Trim();

            _context.TrainingEntries.Add(entry);
            await _context.SaveChangesAsync();
            return entry.Id;
        }

        // UPDATE
        public async Task<bool> UpdateAsync(TrainingEntry form, int userProfileId)
        {
            var entry = await _context.TrainingEntries
                .FirstOrDefaultAsync(e =>
                    e.Id == form.Id &&
                    _context.TrainingDiaries.Any(d =>
                        d.Id == e.TrainingDiaryId &&
                        d.UserProfileId == userProfileId));

            if (entry == null) return false;

            if (entry.TrainingDiaryId != form.TrainingDiaryId)
            {
                var diaryIsMine = await DiaryBelongsToMeAsync(form.TrainingDiaryId, userProfileId);
                if (!diaryIsMine) return false;

                entry.TrainingDiaryId = form.TrainingDiaryId;
            }

            entry.SportName = (form.SportName ?? string.Empty).Trim();
            entry.DurationMinutes = form.DurationMinutes;
            entry.Calories = form.Calories;
            entry.DistanceKm = form.DistanceKm;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(int entryId, int userProfileId)
        {
            var entry = await _context.TrainingEntries
                .FirstOrDefaultAsync(e =>
                    e.Id == entryId &&
                    _context.TrainingDiaries.Any(d =>
                        d.Id == e.TrainingDiaryId &&
                        d.UserProfileId == userProfileId));

            if (entry == null) return false;

            _context.TrainingEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return true;
        }

        // Dropdown: моите дневници
        public async Task<List<(int Id, string Text)>> GetMyDiarySelectItemsAsync(int userProfileId)
        {
            var diaries = await _context.TrainingDiaries
                .AsNoTracking()
                .Where(d => d.UserProfileId == userProfileId)
                .OrderByDescending(d => d.Date)
                .Select(d => new { d.Id, d.Date })
                .ToListAsync();

            return diaries
                .Select(d => (d.Id, d.Date.ToString("dd.MM.yyyy")))
                .ToList();
        }

        // -------- EXERCISES --------

        public async Task<IEnumerable<(int Id, string Name)>> GetActiveExercisesAsync()
        {
            return await _context.Exercises
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new ValueTuple<int, string>(x.Id, x.Name))
                .ToListAsync();
        }

        public async Task<IEnumerable<EntryExerciseItemVm>> GetEntryExercisesAsync(int entryId, int userProfileId)
        {
            var isMine = await _context.TrainingEntries
                .AsNoTracking()
                .AnyAsync(e =>
                    e.Id == entryId &&
                    e.TrainingDiary != null &&
                    e.TrainingDiary.UserProfileId == userProfileId);

            if (!isMine)
                return Enumerable.Empty<EntryExerciseItemVm>();

            return await _context.TrainingEntryExercises
                .AsNoTracking()
                .Where(x => x.TrainingEntryId == entryId)
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
            var entryIsMine = await _context.TrainingEntries
                .AnyAsync(e =>
                    e.Id == model.TrainingEntryId &&
                    e.TrainingDiary != null &&
                    e.TrainingDiary.UserProfileId == userProfileId);

            if (model.ExerciseId is null)
                return false;

            int exerciseId = model.ExerciseId.Value;

            var already = await _context.TrainingEntryExercises
                .AnyAsync(x => x.TrainingEntryId == model.TrainingEntryId && x.ExerciseId == exerciseId);

            if (already) return false;

            _context.TrainingEntryExercises.Add(new TrainingEntryExercise
            {
                TrainingEntryId = model.TrainingEntryId,
                ExerciseId = exerciseId,
                Sets = model.Sets,
                Reps = model.Reps,
                WeightKg = model.WeightKg,
                DurationSeconds = model.DurationSeconds
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveExerciseAsync(int trainingEntryId, int exerciseId, int userProfileId)
        {
            var entryIsMine = await _context.TrainingEntries
                .AnyAsync(e =>
                    e.Id == trainingEntryId &&
                    e.TrainingDiary != null &&
                    e.TrainingDiary.UserProfileId == userProfileId);

            if (!entryIsMine) return false;

            var entity = await _context.TrainingEntryExercises
                .FirstOrDefaultAsync(x => x.TrainingEntryId == trainingEntryId && x.ExerciseId == exerciseId);

            if (entity == null) return false;

            _context.TrainingEntryExercises.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
