using SportsDiarys.ViewModels.TrainingEntries;

namespace SportsDiarys.ViewModels.TrainingEntries
{
    public class TrainingEntryDetailsPageVm
    {
        public TrainingEntryDetailsViewModel Entry { get; set; } = null!;

        public IEnumerable<EntryExerciseItemVm> Exercises { get; set; }
            = new List<EntryExerciseItemVm>();

        public AddExerciseToEntryVm AddExercise { get; set; } = new();

        public string? ReturnUrl { get; set; }
    }
}