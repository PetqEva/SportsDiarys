using System.ComponentModel.DataAnnotations;
using static SportsDiarys.Common.ValidationConstants;

namespace SportsDiarys.ViewModels.UserProfiles
{
    public class UserProfileFormViewModel
    {
        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; } = null!;

        [Range(AgeMin, AgeMax)]
        public int Age { get; set; }
    }
}
