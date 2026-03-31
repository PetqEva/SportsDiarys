namespace SportsDiarys.ViewModels.Progress
{
    public class ProgressVm
    {
        public int TotalDiaries { get; set; }
        public int TotalEntries { get; set; }

        public int Last7DaysDurationMinutes { get; set; }
        public int Last30DaysDurationMinutes { get; set; }

        public int Last7DaysCalories { get; set; }
        public int Last30DaysCalories { get; set; }

        public double Last7DaysDistanceKm { get; set; }
        public double Last30DaysDistanceKm { get; set; }

        public string? MostActiveSport { get; set; }

        public List<SportBreakdownVm> SportBreakdown { get; set; } = new();
        public List<DailyProgressVm> DailyProgress { get; set; } = new();
    }

    public class SportBreakdownVm
    {
        public string SportName { get; set; } = string.Empty;
        public int EntriesCount { get; set; }
        public int TotalDurationMinutes { get; set; }
        public int TotalCalories { get; set; }
        public double TotalDistanceKm { get; set; }
    }

    public class DailyProgressVm
    {
        public string DateLabel { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int Calories { get; set; }
        public double DistanceKm { get; set; }
    }
}