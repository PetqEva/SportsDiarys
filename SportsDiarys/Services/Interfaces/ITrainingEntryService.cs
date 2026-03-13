using SportDiary.Data.Models;
using SportDiary.ViewModels.Shared;
using SportDiary.ViewModels.TrainingEntries;
using SportsDiarys.ViewModels.TrainingEntries;

namespace SportDiary.Services.Interfaces
{
    public interface ITrainingEntryService
    {
        Task<List<TrainingEntry>> GetMyEntriesAsync(int userProfileId);
        Task<PagedResultVm<TrainingEntryAllViewModel>> GetMyEntriesPagedAsync(int userProfileId, EntriesQueryVm query);

        IEnumerable<(string field, string message)> ValidateBusinessRules(TrainingEntryFormVm vm);

        // Details трябва да връща ViewModel (не entity)
        Task<TrainingEntryDetailsViewModel?> GetMyEntryDetailsAsync(int entryId, int userProfileId);

        Task<TrainingEntry?> GetMyEntryForEditAsync(int entryId, int userProfileId);

        Task<bool> DiaryBelongsToMeAsync(int diaryId, int userProfileId);
        Task<int> CreateAsync(TrainingEntry entry, int userProfileId);
        Task<bool> UpdateAsync(TrainingEntry form, int userProfileId);
        Task<bool> DeleteAsync(int entryId, int userProfileId);

        Task<List<(int Id, string Text)>> GetMyDiarySelectItemsAsync(int userProfileId);

        // ---- Exercises ----
        Task<IEnumerable<(int Id, string Name)>> GetActiveExercisesAsync();
        Task<IEnumerable<EntryExerciseItemVm>> GetEntryExercisesAsync(int entryId, int userProfileId);

        // само сигурните версии
        Task<bool> AddExerciseAsync(AddExerciseToEntryVm model, int userProfileId);
        Task<bool> RemoveExerciseAsync(int trainingEntryId, int exerciseId, int userProfileId);
    }
}
