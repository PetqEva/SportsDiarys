using SporstDiarys.ViewModels.TrainingEntries;
using SportsDiarys.Data.Models;
using SportsDiarys.ViewModels.Shared;
using SportsDiarys.ViewModels.TrainingEntries;

namespace SportsDiarys.Services.Interfaces
{
    public class ValidationError
    {
        public string Field { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

    public interface ITrainingEntryService
    {
        Task<List<TrainingEntry>> GetMyEntriesAsync(int userProfileId);
        Task<PagedResultVm<TrainingEntryAllViewModel>> GetMyEntriesPagedAsync(int userProfileId, EntriesQueryVm query);

        IEnumerable<ValidationError> ValidateBusinessRules(TrainingEntryFormVm vm);

        Task<TrainingEntryDetailsViewModel?> GetMyEntryDetailsAsync(int entryId, int userProfileId);
        Task<TrainingEntry?> GetMyEntryForEditAsync(int entryId, int userProfileId);

        Task<bool> DiaryBelongsToMeAsync(int diaryId, int userProfileId);

        Task<int> CreateAsync(TrainingEntry entry, int userProfileId);
        Task<bool> UpdateAsync(TrainingEntry entry, int userProfileId);
        Task<bool> DeleteAsync(int entryId, int userProfileId);

        Task<List<(int Id, string Text)>> GetMyDiarySelectItemsAsync(int userProfileId);

        Task<IEnumerable<(int Id, string Name)>> GetActiveExercisesAsync();
        Task<IEnumerable<EntryExerciseItemVm>> GetEntryExercisesAsync(int entryId, int userProfileId);

        Task<bool> AddExerciseAsync(AddExerciseToEntryVm model, int userProfileId);
        Task<bool> RemoveExerciseAsync(int trainingEntryId, int exerciseId, int userProfileId);
    }
}