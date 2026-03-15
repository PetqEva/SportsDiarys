using System.ComponentModel.DataAnnotations;
using static SportsDiarys.Common.ValidationConstants;

namespace SportsDiarys.ViewModels.TrainingDiaries
{
    public class TrainingDiaryFormViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(DiaryNotesMaxLength)]
        public string? Notes { get; set; }

        [Required]
        public int UserProfileId { get; set; }
    }
}


