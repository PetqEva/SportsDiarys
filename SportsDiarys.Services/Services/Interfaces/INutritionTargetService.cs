using SportsDiarys.Data.Models;

namespace SportsDiarys.Services.Interfaces
{
    public interface INutritionTargetService
    {
        Task<NutritionTarget?> GetByUserIdAsync(string userId);

        Task SaveAsync(string userId, double calories, double proteinGrams);
    }
}