using System.ComponentModel.DataAnnotations;

namespace SportsDiarys.Data.Models
{
    public class NutritionTarget
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        public int Calories { get; set; }
        public int ProteinGrams { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
