using SportsDiarys.Data.Models;
using SportsDiarys.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportsDiarys.Data.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        [Required, MaxLength(60)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(40)]
        public string MuscleGroup { get; set; } = string.Empty; // e.g. Chest, Back, Legs

        [MaxLength(400)]
        public string? Description { get; set; }

        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;

        public ExerciseType Type { get; set; } = ExerciseType.Strength;

        public bool IsActive { get; set; } = true;

        // Навигационно свойство към TrainingEntryExercise (много към много)
        public ICollection<TrainingEntryExercise> TrainingEntryExercises { get; set; } = new List<TrainingEntryExercise>();
    }
}