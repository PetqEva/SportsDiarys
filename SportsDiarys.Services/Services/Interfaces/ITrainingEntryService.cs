using SportsDiarys.Data.Models;
using SportsDiarys.ViewModels.Common;
using SportsDiarys.ViewModels.Shared;
using SportsDiarys.ViewModels.TrainingEntries;

namespace SportsDiarys.Services.Interfaces
{
    public interface ITrainingEntryService
    {
        IEnumerable<ValidationError> ValidateBusinessRules(TrainingEntryFormVm vm);

        Task<int> CreateAsync(TrainingEntry entry, int userProfileId);

        Task<bool> UpdateAsync(TrainingEntry entry, int userProfileId);

        Task<bool> DeleteAsync(int entryId, int userProfileId);

        Task<List<TrainingEntry>> GetMyEntriesAsync(int userProfileId);

        Task<PagedResultVm<TrainingEntryAllViewModel>> GetMyEntriesPagedAsync(
            int userProfileId,
            EntriesQueryVm query);

        Task<TrainingEntryDetailsViewModel?> GetMyEntryDetailsAsync(
            int entryId,
            int userProfileId);

        Task<TrainingEntry?> GetMyEntryForEditAsync(int entryId, int userProfileId);

        Task<bool> DiaryBelongsToMeAsync(int diaryId, int userProfileId);

        Task<List<(int Id, string Text)>> GetMyDiarySelectItemsAsync(int userProfileId);

        Task<IEnumerable<(int Id, string Name)>> GetActiveExercisesAsync();

        Task<IEnumerable<EntryExerciseItemVm>> GetEntryExercisesAsync(int entryId, int userProfileId);

        Task<bool> AddExerciseAsync(AddExerciseToEntryVm model, int userProfileId);

        Task<bool> RemoveExerciseAsync(int entryId, int exerciseId, int userProfileId);
    }
}
