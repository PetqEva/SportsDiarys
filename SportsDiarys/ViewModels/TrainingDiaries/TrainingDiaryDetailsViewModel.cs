namespace SportDiary.ViewModels.TrainingDiaries
{
    public class TrainingDiaryDetailsViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public string? Notes { get; set; }

        public int UserProfileId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public List<EntryItem> Entries { get; set; } = new();

        // 📊 Summary
        public int TotalEntries { get; set; }
        public int TotalDurationMinutes { get; set; }
        public int TotalCalories { get; set; }
        public double TotalDistanceKm { get; set; }

        public class EntryItem
        {
            public int Id { get; set; }
            public string SportName { get; set; } = string.Empty;
            public int DurationMinutes { get; set; }
            public int Calories { get; set; }
            public double? DistanceKm { get; set; }
        }
    }
}
