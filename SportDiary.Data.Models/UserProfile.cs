using SportsDiarys.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDiarys.Data.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        public string IdentityUserId { get; set; } = null!;

        [ForeignKey(nameof(IdentityUserId))]
        public ApplicationUser IdentityUser { get; set; } = null!;

        public ICollection<TrainingDiary> TrainingDiaries { get; set; } = new List<TrainingDiary>();

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(10, 100)]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Range(30, 300)]
        public double StartWeightKg { get; set; }

        [Range(30, 300)]
        public double CurrentWeightKg { get; set; }

        [Range(100, 250)]
        public int HeightCm { get; set; }

        [Required]
        public string ActivityLevel { get; set; } = string.Empty;
    }
}
