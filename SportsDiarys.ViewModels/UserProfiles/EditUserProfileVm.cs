using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SportsDiarys.ViewModels.UserProfiles
{
    public class EditUserProfileVm
    {
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

        // dropdown options
        public List<SelectListItem> ActivityOptions { get; set; } = new();
    }
}
