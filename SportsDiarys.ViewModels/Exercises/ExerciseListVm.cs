using SportsDiarys.ViewModels.Exercises;

namespace SportsDiarys.ViewModels.Exercises
{
    public class ExerciseListVm
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public IEnumerable<ExerciseListItemVm> Items { get; set; } = new List<ExerciseListItemVm>();

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // Добавяме Query property
        public ExerciseQueryVm Query { get; set; } = new ExerciseQueryVm();
    }
}