namespace SportsDiarys.ViewModels.Home
{
    public class HomeDashboardVm
    {
        public bool IsAuthenticated { get; set; }

        public string Email { get; set; } = null!;
        public string ProfileName { get; set; } = null!;

        public int DiariesCount { get; set; }
        public int EntriesCount { get; set; }
        public int TotalDurationMinutes { get; set; }
        public double TotalWaterLiters { get; set; }

        public List<RecentDiaryVm> RecentDiaries { get; set; } = new();

        public class RecentDiaryVm
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Place { get; set; } = null!;
            public int DurationMinutes { get; set; }
            public double WaterLiters { get; set; }
            public string Notes { get; set; } = null!;
        }
    }
}
