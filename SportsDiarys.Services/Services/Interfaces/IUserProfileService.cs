using SportsDiarys.Data.Models;

namespace SportsDiarys.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfile?> GetMyProfileAsync(string userId);
        Task<UserProfile?> GetMyProfileWithDiariesAsync(string userId);

        Task<bool> CreateMyProfileAsync(
            string userId,
            string name,
            int age,
            string gender,
            double startWeightKg,
            double currentWeightKg,
            int heightCm,
            string activityLevel);

        Task<bool> UpdateMyProfileAsync(
            string userId,
            string name,
            int age,
            string gender,
            double startWeightKg,
            double currentWeightKg,
            int heightCm,
            string activityLevel);

        Task<bool> DeleteMyProfileAsync(string userId);
    }
}


