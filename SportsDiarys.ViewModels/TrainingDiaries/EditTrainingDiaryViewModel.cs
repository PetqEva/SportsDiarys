using System;
using System.ComponentModel.DataAnnotations;

namespace SportsDiarys.ViewModels.TrainingDiaries
{
    public class EditTrainingDiaryViewModel
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
    }
}