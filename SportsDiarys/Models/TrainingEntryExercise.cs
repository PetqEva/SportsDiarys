using System.ComponentModel.DataAnnotations;

namespace SportsDiarys.Data.Models
{
    public class TrainingEntryExercise
    {
        public int TrainingEntryId { get; set; }
        public TrainingEntry TrainingEntry { get; set; } = null!;

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;

        [Range(1, 50)]
        public int Sets { get; set; } = 3;

        [Range(1, 300)]
        public int Reps { get; set; } = 10;

        [Range(0, 500)]
        public double? WeightKg { get; set; } // null ако е bodyweight

        [Range(0, 600)]
        public int? DurationSeconds { get; set; } // за cardio/издръжливост
    }
}