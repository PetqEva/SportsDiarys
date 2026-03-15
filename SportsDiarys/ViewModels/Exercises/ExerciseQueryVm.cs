namespace SportsDiarys.ViewModels.Exercises
{
    public class ExerciseQueryVm
    {
        public string? Search { get; set; }             // name contains
        public string? MuscleGroup { get; set; }        // exact/contains
        public int? Type { get; set; }                  // enum int
        public int? Difficulty { get; set; }            // enum int
        public bool OnlyActive { get; set; } = true;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
