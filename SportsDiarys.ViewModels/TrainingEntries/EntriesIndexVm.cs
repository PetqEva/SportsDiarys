using SporstDiarys.ViewModels.TrainingEntries;
using SportsDiarys.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SportsDiarys.ViewModels.TrainingEntries
{
    public class EntriesIndexVm
    {
        public EntriesQueryVm Query { get; set; } = new();
        public List<SelectListItem> Diaries { get; set; } = new();

        public PagedResultVm<TrainingEntryAllViewModel> Result { get; set; } = new();
    }
}
