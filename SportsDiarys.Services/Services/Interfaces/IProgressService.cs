using SportsDiarys.ViewModels.Progress;

namespace SportsDiarys.Services.Interfaces
{
    public interface IProgressService
    {
        Task<ProgressVm> GetMyProgressAsync(int userProfileId);
    }
}