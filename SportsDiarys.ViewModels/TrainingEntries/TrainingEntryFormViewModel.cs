using System.ComponentModel.DataAnnotations;
using static SportsDiarys.Common.ValidationConstants;

namespace SportsDiarys.ViewModels.TrainingEntries
{
    public class TrainingEntryFormViewModel
    {
        [Required]
        [StringLength(SportNameMaxLength, MinimumLength = SportNameMinLength)]
        public string SportName { get; set; } = string.Empty;

        [Range(DurationMin, DurationMax)]
        public int DurationMinutes { get; set; }

        [Range(CaloriesMin, CaloriesMax)]
        public int Calories { get; set; }

        [Range(0, 100)]
        public double? DistanceKm { get; set; }

        [Required]
        public int TrainingDiaryId { get; set; }
    }
}

