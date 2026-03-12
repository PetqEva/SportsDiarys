namespace SportDiary.ViewModels.TrainingEntries
{
    public class TrainingEntryDetailsViewModel
    {
        public int Id { get; set; }

        public string SportName { get; set; } = null!;
        public int DurationMinutes { get; set; }
        public int Calories { get; set; }
        public double? DistanceKm { get; set; }

        public int TrainingDiaryId { get; set; }
        public string DiaryLabel { get; set; } = null!;

        public DateTime DiaryDate { get; set; }

        public int UserProfileId { get; set; }
        public string UserName { get; set; } = null!;
    }
}
