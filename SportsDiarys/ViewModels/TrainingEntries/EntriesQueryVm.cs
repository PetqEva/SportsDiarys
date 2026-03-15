namespace SportsDiarys.ViewModels.TrainingEntries
{
    public class EntriesQueryVm
    {
        public int? DiaryId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Sport { get; set; }
        public string Sort { get; set; } = "date_desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}