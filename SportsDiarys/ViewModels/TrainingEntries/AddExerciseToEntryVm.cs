using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SportDiary.ViewModels.TrainingEntries
{
    public class AddExerciseToEntryVm
    {
        public int TrainingEntryId { get; set; }

        [Required(ErrorMessage = "Избери упражнение.")]
        public int? ExerciseId { get; set; }

        [Range(1, 50)]
        public int Sets { get; set; } = 3;

        [Range(1, 300)]
        public int Reps { get; set; } = 10;

        [Range(0, 500)]
        public double? WeightKg { get; set; }

        [Range(0, 600)]
        public int? DurationSeconds { get; set; }

        public IEnumerable<SelectListItem> AvailableExercises { get; set; }
            = new List<SelectListItem>();
    }
}
