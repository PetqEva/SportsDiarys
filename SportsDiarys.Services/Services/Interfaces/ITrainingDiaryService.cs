using SportsDiarys.Models;
using SportsDiarys.ViewModels.TrainingDiaries;

namespace SportsDiarys.Services.Interfaces
{
    public interface ITrainingDiaryService
    {
        Task<List<TrainingDiary>> GetMyDiariesAsync(int userProfileId);
        Task<TrainingDiary?> GetMyDiaryDetailsAsync(int diaryId, int userProfileId);
        Task<TrainingDiaryDetailsViewModel?> GetMyDiaryDetailsVmAsync(int diaryId, int userProfileId);
        Task<TrainingDiary?> GetMyDiaryForEditAsync(int diaryId, int userProfileId);

        Task<bool> DiaryExistsForDateAsync(int userProfileId, DateTime date, int? excludeDiaryId = null);

        Task<int> CreateAsync(TrainingDiary diary, int userProfileId);

        
        Task<bool> UpdateAsync(
            int diaryId,
            int userProfileId,
            DateTime date,
            int durationMinutes,
            string place,
            double waterLiters,
            string? notes);

        Task<bool> DeleteAsync(int diaryId, int userProfileId);
    }
}
