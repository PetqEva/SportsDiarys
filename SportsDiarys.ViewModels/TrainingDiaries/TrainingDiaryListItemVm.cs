namespace SportsDiarys.ViewModels.TrainingDiaries
{
    public class TrainingDiaryListItemVm
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Place { get; set; }
        public int DurationMinutes { get; set; }
        public double WaterLiters { get; set; }
        public string? Notes { get; set; }
    }
}