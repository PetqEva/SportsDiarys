using System.ComponentModel.DataAnnotations;
using SportsDiarys.Models.Enums;

namespace SportsDiarys.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        [Required, MaxLength(60)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(40)]
        public string MuscleGroup { get; set; } = null!; // e.g. Chest, Back, Legs

        [MaxLength(400)]
        public string? Description { get; set; }

        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;

        public bool IsActive { get; set; } = true;

    }
}
