using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;
using SportsDiarys.Data.Models.Enum;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.Exercises;

namespace SportsDiarys.Services.Implementations
{
    public class ExerciseService : IExerciseService
    {
        private readonly AppDbContext _context;

        public ExerciseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ExerciseListVm> GetPagedAsync(ExerciseQueryVm query)
        {
            var q = _context.Exercises.AsNoTracking();

            if (query.OnlyActive)
                q = q.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim().ToLower();
                q = q.Where(x =>
                    x.Name.ToLower().Contains(s) ||
                    x.MuscleGroup.ToLower().Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(query.MuscleGroup))
            {
                var mg = query.MuscleGroup.Trim().ToLower();
                q = q.Where(x => x.MuscleGroup.ToLower().Contains(mg));
            }

            if (query.Type.HasValue)
            {
                var t = (ExerciseType)query.Type.Value;
                q = q.Where(x => x.Type == t);
            }

            if (query.Difficulty.HasValue)
            {
                var d = (DifficultyLevel)query.Difficulty.Value;
                q = q.Where(x => x.Difficulty == d);
            }

            q = q.OrderBy(x => x.MuscleGroup).ThenBy(x => x.Name);

            var page = query.Page < 1 ? 1 : query.Page;
            var pageSize = query.PageSize < 5 ? 10 : query.PageSize;

            var total = await q.CountAsync();

            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ExerciseListItemVm
                {
                    Id = x.Id,
                    Name = x.Name,
                    MuscleGroup = x.MuscleGroup,
                    Type = x.Type.ToString(),
                    Difficulty = x.Difficulty.ToString(),
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return new ExerciseListVm
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = total,
                Items = items
            };
        }

        public async Task<ExerciseFormVm?> GetForEditAsync(int id)
        {
            return await _context.Exercises
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new ExerciseFormVm
                {
                    Id = x.Id,
                    Name = x.Name,
                    MuscleGroup = x.MuscleGroup,
                    Description = x.Description,
                    Difficulty = (int)x.Difficulty,
                    Type = (int)x.Type,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(ExerciseFormVm model)
        {
            // нормализация (проста, но полезна)
            var name = model.Name.Trim();
            var mg = model.MuscleGroup.Trim();

            var entity = new Exercise
            {
                Name = name,
                MuscleGroup = mg,
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                Difficulty = (DifficultyLevel)model.Difficulty,
                Type = (ExerciseType)model.Type,
                IsActive = model.IsActive
            };

            _context.Exercises.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(ExerciseFormVm model)
        {
            var entity = await _context.Exercises.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null) return false;

            entity.Name = model.Name.Trim();
            entity.MuscleGroup = model.MuscleGroup.Trim();
            entity.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
            entity.Difficulty = (DifficultyLevel)model.Difficulty;
            entity.Type = (ExerciseType)model.Type;
            entity.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var entity = await _context.Exercises.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            entity.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<bool> ExistsAsync(int id)
            => _context.Exercises.AsNoTracking().AnyAsync(x => x.Id == id);
    }
}
