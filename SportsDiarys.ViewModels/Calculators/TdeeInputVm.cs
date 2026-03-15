using System.ComponentModel.DataAnnotations;

namespace SportsDiarys.ViewModels.Calculators
{
    public enum Gender
    {
        [Display(Name = "Мъж")]
        Male = 1,

        [Display(Name = "Жена")]
        Female = 2
    }
    public class TdeeInputVm
    {
        [Required]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Въведи възраст")]
        [Range(10, 100)]
        public int Age { get; set; }

        [Required(ErrorMessage = "Въведи тегло в килограми")]
        [Range(30, 300)]
        public double WeightKg { get; set; }

        [Required(ErrorMessage = "Въведи височина в сантиметри")]
        [Range(120, 230)]
        public double HeightCm { get; set; }

        // Body fat % (optional)
        [Range(3, 60)]
        public double? BodyFatPercent { get; set; }

        // 1.2 / 1.375 / 1.55 / 1.725 / 1.9
        [Range(1.2, 1.9)]
        public double ActivityMultiplier { get; set; } = 1.2;
    }
}
