using SportDiary.ViewModels.Home;

namespace SportDiary.Services.Interfaces
{
    public interface IHomeDashboardService
    {
        Task<HomeDashboardVm> BuildAsync(string userId);
    }
}