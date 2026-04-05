using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SportsDiarys.ViewModels.Exercises
{
    public class ExerciseFormVm
    {
        public int Id { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        public string MuscleGroup { get; set; } = string.Empty;

        [StringLength(400)]
        public string? Description { get; set; }

        public int Difficulty { get; set; }

        public int Type { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ImagePath { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}