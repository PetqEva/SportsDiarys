using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SportsDiarys.ViewModels.TrainingDiaries
{
    public class UpdateTrainingDiaryViewModel
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        public int Calories { get; set; }

        public int DurationMinutes { get; set; }

        public double DistanceKm { get; set; }

        public double WaterLiters { get; set; }

        public string Place { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> PlaceOptions { get; set; } = new List<SelectListItem>();
    }
}