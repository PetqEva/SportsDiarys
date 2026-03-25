using System.Collections.Generic;

namespace SportsDiarys.ViewModels.TrainingDiaries
{
    public class TrainingDiaryListVm
    {
        public List<TrainingDiaryListItemVm> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string? Search { get; set; }
    }
}