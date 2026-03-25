using SportsDiarys.ViewModels.TrainingDiaries;

namespace SportsDiarys.Services.Interfaces
{
    public interface ITrainingDiaryService
    {
        Task<TrainingDiaryListVm> GetMyDiariesAsync(int userProfileId, string? search, int page, int pageSize);

        Task<TrainingDiaryDetailsViewModel?> GetByIdAsync(int diaryId, int userProfileId);

        Task<EditTrainingDiaryViewModel?> GetForEditAsync(int diaryId, int userProfileId);

        Task<bool> ExistsForDateAsync(int userProfileId, DateTime date, int? excludeDiaryId = null);

        Task<int> CreateAsync(CreateTrainingDiaryViewModel model, int userProfileId);

        Task<bool> UpdateAsync(UpdateTrainingDiaryViewModel model, int userProfileId);

        Task<bool> DeleteAsync(int diaryId, int userProfileId);
    }
}