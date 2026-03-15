namespace SportsDiarys.ViewModels.Exercises
{
    public class ExerciseListItemVm
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string MuscleGroup { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Difficulty { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
