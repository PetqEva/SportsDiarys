using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Interfaces;
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
                yield return new ValidationError { Field = nameof(vm.DurationMinutes), Message = "Продължителността трябва да е положителна." };

            if (vm.DurationMinutes > 600)
                yield return new ValidationError { Field = nameof(vm.DurationMinutes), Message = "Продължителността е нереалистично голяма." };

            if (vm.Calories < 0)
                yield return new ValidationError { Field = nameof(vm.Calories), Message = "Калориите не могат да са отрицателни." };

            if (vm.Calories > vm.DurationMinutes * 40)
                yield return new ValidationError { Field = nameof(vm.Calories), Message = "Калориите са нереалистично високи спрямо продължителността." };

            if (vm.DistanceKm.HasValue)
            {
                if (vm.DistanceKm < 0)
                    yield return new ValidationError { Field = nameof(vm.DistanceKm), Message = "Разстоянието не може да е отрицателно." };

                if (vm.DistanceKm > 200)
                    yield return new ValidationError { Field = nameof(vm.DistanceKm), Message = "Разстоянието е нереалистично голямо." };
            }

            if (string.IsNullOrWhiteSpace(vm.SportName))
                yield return new ValidationError { Field = nameof(vm.SportName), Message = "Името на спорта е задължително." };

            if (!string.IsNullOrWhiteSpace(vm.SportName) && vm.SportName.Length < 3)
                yield return new ValidationError { Field = nameof(vm.SportName), Message = "Името трябва да съдържа поне 3 символа." };
        }

        // ===================== CRUD =====================
        public async Task<int> CreateAsync(TrainingEntry entry, int userProfileId)
        {
            bool ownsDiary = await _context.TrainingDiaries
                .AsNoTracking()
                .AnyAsync(d => d.Id == entry.TrainingDiaryId && d.UserProfileId == userProfileId);

            if (!ownsDiary)
                throw new UnauthorizedAccessException("Diary does not belong to the current user.");

            entry.SportName = (entry.SportName ?? string.Empty).Trim();
            _context.TrainingEntries.Add(entry);
            await _context.SaveChangesAsync();
            return entry.Id;
        }

        public async Task<bool> UpdateAsync(TrainingEntry entry, int userProfileId)
        {
            var existing = await _context.TrainingEntries
                .Include(e => e.TrainingDiary)
                .FirstOrDefaultAsync(e => e.Id == entry.Id && e.TrainingDiary.UserProfileId == userProfileId);

            if (existing == null)
                return false;

            existing.SportName = (entry.SportName ?? string.Empty).Trim();
            existing.DurationMinutes = entry.DurationMinutes;
            existing.Calories = entry.Calories;
            existing.DistanceKm = entry.DistanceKm;

            if (existing.TrainingDiaryId != entry.TrainingDiaryId)
            {
                bool ownsDiary = await _context.TrainingDiaries
                    .AnyAsync(d => d.Id == entry.TrainingDiaryId && d.UserProfileId == userProfileId);

                if (!ownsDiary) return false;

                existing.TrainingDiaryId = entry.TrainingDiaryId;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int entryId, int userProfileId)
        {
            var entry = await _context.TrainingEntries
                .Include(e => e.TrainingDiary)
                .FirstOrDefaultAsync(e => e.Id == entryId && e.TrainingDiary.UserProfileId == userProfileId);

            if (entry == null) return false;

            _context.TrainingEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TrainingEntry>> GetMyEntriesAsync(int userProfileId)
        {
            return await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .Where(e => e.TrainingDiary.UserProfileId == userProfileId)
                .OrderByDescending(e => e.TrainingDiary.Date)
                .ThenByDescending(e => e.Id)
                .ToListAsync();
        }

        public async Task<PagedResultVm<TrainingEntryAllViewModel>> GetMyEntriesPagedAsync(int userProfileId, EntriesQueryVm query)
        {
            query ??= new EntriesQueryVm();
            query.Page = Math.Max(1, query.Page);
            query.PageSize = Math.Clamp(query.PageSize, 5, 50);

            var q = _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .Where(e => e.TrainingDiary.UserProfileId == userProfileId);

            if (query.DiaryId.HasValue)
                q = q.Where(e => e.TrainingDiaryId == query.DiaryId.Value);

            if (!string.IsNullOrWhiteSpace(query.Sport))
                q = q.Where(e => e.SportName.Contains(query.Sport.Trim()));

            if (query.From.HasValue)
                q = q.Where(e => e.TrainingDiary.Date.Date >= query.From.Value.Date);

            if (query.To.HasValue)
                q = q.Where(e => e.TrainingDiary.Date.Date <= query.To.Value.Date);

            q = query.Sort switch
            {
                "calories_desc" => q.OrderByDescending(e => e.Calories).ThenByDescending(e => e.Id),
                "duration_desc" => q.OrderByDescending(e => e.DurationMinutes).ThenByDescending(e => e.Id),
                "distance_desc" => q.OrderByDescending(e => e.DistanceKm ?? 0).ThenByDescending(e => e.Id),
                "date_asc" => q.OrderBy(e => e.TrainingDiary.Date).ThenBy(e => e.Id),
                _ => q.OrderByDescending(e => e.TrainingDiary.Date).ThenByDescending(e => e.Id)
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
                    DiaryLabel = e.TrainingDiary.Date.ToString("yyyy-MM-dd")
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

        public async Task<TrainingEntryDetailsViewModel?> GetMyEntryDetailsAsync(int entryId, int userProfileId)
        {
            return await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                    .ThenInclude(d => d.UserProfile)
                .Where(e => e.Id == entryId && e.TrainingDiary.UserProfileId == userProfileId)
                .Select(e => new TrainingEntryDetailsViewModel
                {
                    Id = e.Id,
                    SportName = e.SportName,
                    DurationMinutes = e.DurationMinutes,
                    Calories = e.Calories,
                    DistanceKm = e.DistanceKm,
                    TrainingDiaryId = e.TrainingDiaryId,
                    DiaryLabel = e.TrainingDiary.Date.ToString("yyyy-MM-dd"),
                    UserProfileId = e.TrainingDiary.UserProfileId,
                    UserName = e.TrainingDiary.UserProfile.Name,
                    DiaryDate = e.TrainingDiary.Date
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TrainingEntry?> GetMyEntryForEditAsync(int entryId, int userProfileId)
        {
            return await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .FirstOrDefaultAsync(e => e.Id == entryId && e.TrainingDiary.UserProfileId == userProfileId);
        }

        public async Task<bool> DiaryBelongsToMeAsync(int diaryId, int userProfileId)
        {
            return await _context.TrainingDiaries
                .AsNoTracking()
                .AnyAsync(d => d.Id == diaryId && d.UserProfileId == userProfileId);
        }

        // ===================== DIARY DROPDOWN =====================
        public async Task<List<(int Id, string Text)>> GetMyDiarySelectItemsAsync(int userProfileId)
        {
            var diaries = await _context.TrainingDiaries
                .AsNoTracking()
                .Where(d => d.UserProfileId == userProfileId)
                .OrderByDescending(d => d.Date)
                .Select(d => new { d.Id, d.Date })
                .ToListAsync();

            return diaries.Select(d => (d.Id, d.Date.ToString("dd.MM.yyyy"))).ToList();
        }

        // ===================== EXERCISES =====================
        public async Task<IEnumerable<(int Id, string Name)>> GetActiveExercisesAsync()
        {
            var exercises = await _context.Exercises
                .AsNoTracking()
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .Select(e => new { e.Id, e.Name })
                .ToListAsync();

            return exercises.Select(e => (e.Id, e.Name));
        }

        public async Task<IEnumerable<EntryExerciseItemVm>> GetEntryExercisesAsync(int entryId, int userProfileId)
        {
            var isMine = await _context.TrainingEntries
                .AsNoTracking()
                .AnyAsync(e => e.Id == entryId && e.TrainingDiary.UserProfileId == userProfileId);

            if (!isMine) return Enumerable.Empty<EntryExerciseItemVm>();

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
            if (model.ExerciseId is null) return false;

            var entryIsMine = await _context.TrainingEntries
                .AnyAsync(e => e.Id == model.TrainingEntryId && e.TrainingDiary.UserProfileId == userProfileId);

            if (!entryIsMine) return false;

            var already = await _context.TrainingEntryExercises
                .AnyAsync(x => x.TrainingEntryId == model.TrainingEntryId && x.ExerciseId == model.ExerciseId.Value);

            if (already) return false;

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

        public async Task<bool> RemoveExerciseAsync(int trainingEntryId, int exerciseId, int userProfileId)
        {
            var entryIsMine = await _context.TrainingEntries
                .AnyAsync(e => e.Id == trainingEntryId && e.TrainingDiary.UserProfileId == userProfileId);

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