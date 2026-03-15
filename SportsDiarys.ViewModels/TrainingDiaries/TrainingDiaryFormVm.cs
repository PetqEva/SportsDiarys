using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SportsDiarys.ViewModels.TrainingDiaries
{
    public class TrainingDiaryFormVm
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Range(0, 600)]
        public int DurationMinutes { get; set; }

        [Required]
        public string Place { get; set; } = string.Empty;

        [Range(0, 20)]
        public double WaterLiters { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public List<SelectListItem> PlaceOptions { get; set; } = new();

    }
}
