
using SportsDiarys.Data.Models;
using SportsDiarys.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportDiary.Data.Models
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

        public ExerciseType ExerciseType { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public ICollection<TrainingEntryExercise> TrainingEntries { get; set; } = new List<TrainingEntryExercise>();
    }
}

