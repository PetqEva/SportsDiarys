namespace SportDiary.ViewModels.TrainingEntries
{
    public class EntryExerciseItemVm
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; } = string.Empty;

        public int Sets { get; set; }
        public int Reps { get; set; }

        public double? WeightKg { get; set; }
        public int? DurationSeconds { get; set; }
    }
}
