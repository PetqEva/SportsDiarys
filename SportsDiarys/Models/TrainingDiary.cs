using SportsDiarys.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace SportsDiarys.Models
{
    public class TrainingDiary
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Range(0, 600)]
        public int DurationMinutes { get; set; }

        [Required]
        public string Place { get; set; } = string.Empty; // Home/Gym/Outdoor/Other

        [Range(0, 20)]
        public double WaterLiters { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int UserProfileId { get; set; }

        public UserProfile UserProfile { get; set; } = null!;

        public ICollection<TrainingEntry> TrainingEntries { get; set; } = new List<TrainingEntry>();

    }
}
