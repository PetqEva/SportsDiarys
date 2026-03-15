using SportsDiarys.ViewModels.Home;

namespace SportsDiarys.Services.Interfaces
{
    public interface IHomeDashboardService
    {
        Task<HomeDashboardVm> BuildAsync(string userId);
    }
}