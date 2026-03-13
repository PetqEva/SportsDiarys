namespace SportDiary.ViewModels.Exercises
{
    public class ExerciseListVm
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public IEnumerable<ExerciseListItemVm> Items { get; set; } = new List<ExerciseListItemVm>();

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
