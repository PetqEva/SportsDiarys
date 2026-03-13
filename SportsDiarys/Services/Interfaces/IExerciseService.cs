using SportDiary.ViewModels.Exercises;

namespace SportDiary.Services.Interfaces
{
    public interface IExerciseService
    {
        Task<ExerciseListVm> GetPagedAsync(ExerciseQueryVm query);

        Task<ExerciseFormVm?> GetForEditAsync(int id);

        Task<int> CreateAsync(ExerciseFormVm model);

        Task<bool> UpdateAsync(ExerciseFormVm model);

        Task<bool> SetActiveAsync(int id, bool isActive);

        Task<bool> ExistsAsync(int id);
    }
}
