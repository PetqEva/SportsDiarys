using SporstDiarys.ViewModels.TrainingEntries;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Implementations;
using SportsDiarys.ViewModels.Shared;
using SportsDiarys.ViewModels.TrainingEntries;

namespace SportsDiarys.Services.Interfaces
{
    public interface ITrainingEntryService
    {
        // ===================== ENTRIES =====================
        Task<List<TrainingEntry>> GetMyEntriesAsync(int userProfileId);
        Task<PagedResultVm<TrainingEntryAllViewModel>> GetMyEntriesPagedAsync(int userProfileId, EntriesQueryVm query);

        // ===================== VALIDATION =====================
        IEnumerable<TrainingEntryService.ValidationError> ValidateBusinessRules(TrainingEntryFormVm vm);

        // ===================== SINGLE ENTRY =====================
        Task<TrainingEntryDetailsViewModel?> GetMyEntryDetailsAsync(int entryId, int userProfileId);
        Task<TrainingEntry?> GetMyEntryForEditAsync(int entryId, int userProfileId);

        // ===================== DIARY CHECK =====================
        Task<bool> DiaryBelongsToMeAsync(int diaryId, int userProfileId);

        // ===================== CRUD =====================
        Task<int> CreateAsync(TrainingEntry entry, int userProfileId);
        Task<bool> UpdateAsync(TrainingEntry form, int userProfileId);
        Task<bool> DeleteAsync(int entryId, int userProfileId);

        // ===================== DIARY DROPDOWN =====================
        Task<List<(int Id, string Text)>> GetMyDiarySelectItemsAsync(int userProfileId);

        // ===================== EXERCISES =====================
        Task<IEnumerable<(int Id, string Name)>> GetActiveExercisesAsync();
        Task<IEnumerable<EntryExerciseItemVm>> GetEntryExercisesAsync(int entryId, int userProfileId);
        Task<bool> AddExerciseAsync(AddExerciseToEntryVm model, int userProfileId);
        Task<bool> RemoveExerciseAsync(int trainingEntryId, int exerciseId, int userProfileId);
    }
}