using SportsDiarys.Data.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SportsDiarys.Data.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        [Required, MaxLength(60)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(40)]
        public string MuscleGroup { get; set; } = string.Empty;

        [MaxLength(400)]
        public string? Description { get; set; }

        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;

        public ExerciseType Type { get; set; } = ExerciseType.Strength;

        public bool IsActive { get; set; } = true;

        // ✅ NEW
        [MaxLength(255)]
        public string? ImagePath { get; set; }

        public ICollection<TrainingEntryExercise> TrainingEntryExercises { get; set; } = new List<TrainingEntryExercise>();
    }
}