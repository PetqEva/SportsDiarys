namespace SportDiary.ViewModels.TrainingEntries
{
    public class TrainingEntryAllViewModel
    {
        public int Id { get; set; }

        public string SportName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int Calories { get; set; }
        public double? DistanceKm { get; set; }

        public int TrainingDiaryId { get; set; }
        public string DiaryLabel { get; set; } = string.Empty; // "Geri - 2026-02-02"
    }
}

