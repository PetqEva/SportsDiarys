using SportsDiarys.ViewModels.Home;
using System.Threading.Tasks;

namespace SportsDiarys.Services.Interfaces
{
    public interface IHomeDashboardService
    {
        Task<HomeDashboardVm> GetDashboardAsync(string userId);
    }
}