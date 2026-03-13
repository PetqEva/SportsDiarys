namespace SportDiary.ViewModels.TrainingDiaries
{
    public class TrainingDiaryAllViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public string? Notes { get; set; }

        public int UserProfileId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
