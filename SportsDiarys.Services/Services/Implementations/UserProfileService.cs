using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Interfaces;

namespace SportsDiarys.Services.Implementations
{
    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _context;

        // Ако искаш “allow list” за activity level (както в UI)
        private static readonly HashSet<string> AllowedActivityLevels =
            new(StringComparer.OrdinalIgnoreCase) { "Low", "Medium", "High" };

        public UserProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetMyProfileAsync(string userId)
        {
            return await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdentityUserId == userId);
        }

        public async Task<UserProfile?> GetMyProfileWithDiariesAsync(string userId)
        {
            return await _context.UserProfiles
                .AsNoTracking()
                .Include(p => p.TrainingDiaries)
                    .ThenInclude(d => d.TrainingEntries)
                .FirstOrDefaultAsync(p => p.IdentityUserId == userId);
        }

        public async Task<bool> CreateMyProfileAsync(
            string userId,
            string name,
            int age,
            string gender,
            double startWeightKg,
            double currentWeightKg,
            int heightCm,
            string activityLevel)
        {
            // Guard: един профил на user
            var exists = await _context.UserProfiles
                .AsNoTracking()
                .AnyAsync(p => p.IdentityUserId == userId);

            if (exists) return false;

            // Optional hardening: activity allow-list
            if (!string.IsNullOrWhiteSpace(activityLevel) && !AllowedActivityLevels.Contains(activityLevel))
                return false;

            var profile = new UserProfile
            {
                IdentityUserId = userId,
                Name = (name ?? string.Empty).Trim(),
                Age = age,
                Gender = gender ?? string.Empty,
                StartWeightKg = startWeightKg,
                CurrentWeightKg = currentWeightKg,
                HeightCm = heightCm,
                ActivityLevel = activityLevel ?? string.Empty
            };

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateMyProfileAsync(
            string userId,
            string name,
            int age,
            string gender,
            double startWeightKg,
            double currentWeightKg,
            int heightCm,
            string activityLevel)
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.IdentityUserId == userId);

            if (profile == null) return false;

            // Optional hardening
            if (!string.IsNullOrWhiteSpace(activityLevel) && !AllowedActivityLevels.Contains(activityLevel))
                return false;

            profile.Name = (name ?? string.Empty).Trim();
            profile.Age = age;
            profile.Gender = gender ?? string.Empty;
            profile.StartWeightKg = startWeightKg;
            profile.CurrentWeightKg = currentWeightKg;
            profile.HeightCm = heightCm;
            profile.ActivityLevel = activityLevel ?? string.Empty;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMyProfileAsync(string userId)
        {
            var profile = await _context.UserProfiles
                .Include(p => p.TrainingDiaries)
                    .ThenInclude(d => d.TrainingEntries)
                .FirstOrDefaultAsync(p => p.IdentityUserId == userId);

            if (profile == null) return false;

            // Предвидимо изтриване (без да разчиташ на cascade)
            foreach (var diary in profile.TrainingDiaries.ToList())
            {
                _context.TrainingEntries.RemoveRange(diary.TrainingEntries);
            }

            _context.TrainingDiaries.RemoveRange(profile.TrainingDiaries);
            _context.UserProfiles.Remove(profile);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
