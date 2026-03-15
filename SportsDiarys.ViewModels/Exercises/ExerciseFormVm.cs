using System.ComponentModel.DataAnnotations;

namespace SporstDiarys.ViewModels.Exercises
{
    public class ExerciseFormVm
    {
        public int Id { get; set; }

        [Required, StringLength(60, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(40, MinimumLength = 2)]
        public string MuscleGroup { get; set; } = string.Empty;

        [StringLength(400)]
        public string? Description { get; set; }

        // enum as int (лесно за dropdown)
        [Range(0, 2)]
        public int Difficulty { get; set; } = 1;

        [Range(0, 2)]
        public int Type { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
