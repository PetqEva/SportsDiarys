using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using static SportsDiarys.Common.ValidationConstants;

namespace SportsDiarys.ViewModels.TrainingEntries
{
    public class TrainingEntryFormVm
    {
        public int Id { get; set; }

        [Required]
        [StringLength(SportNameMaxLength, MinimumLength = SportNameMinLength)]
        public string SportName { get; set; } = string.Empty;

        [Range(DurationMin, DurationMax)]
        public int DurationMinutes { get; set; }

        [Range(CaloriesMin, CaloriesMax)]
        public int Calories { get; set; }

        [Range(0, 50)]
        public double? DistanceKm { get; set; }

        [Required]
        public int TrainingDiaryId { get; set; }

        // dropdown за Create/Edit
        public List<SelectListItem> Diaries { get; set; } = new();
        public string? ReturnUrl { get; set; }


    }
}




