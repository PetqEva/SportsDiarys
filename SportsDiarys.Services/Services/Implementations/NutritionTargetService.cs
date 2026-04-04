using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Interfaces;

namespace SportsDiarys.Services.Implementations
{
    public class NutritionTargetService : INutritionTargetService
    {
        private readonly AppDbContext _context;

        public NutritionTargetService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NutritionTarget?> GetByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User id is required.", nameof(userId));
            }

            return await _context.NutritionTargets
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task SaveAsync(string userId, double calories, double proteinGrams)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User id is required.", nameof(userId));
            }

            if (calories <= 0)
            {
                throw new ArgumentException("Calories must be greater than 0.", nameof(calories));
            }

            if (proteinGrams <= 0)
            {
                throw new ArgumentException("Protein grams must be greater than 0.", nameof(proteinGrams));
            }

            var roundedCalories = (int)Math.Round(calories);
            var roundedProtein = (int)Math.Round(proteinGrams);

            var existing = await _context.NutritionTargets
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (existing == null)
            {
                var target = new NutritionTarget
                {
                    UserId = userId,
                    Calories = roundedCalories,
                    ProteinGrams = roundedProtein,
                    CreatedOn = DateTime.UtcNow
                };

                _context.NutritionTargets.Add(target);
            }
            else
            {
                existing.Calories = roundedCalories;
                existing.ProteinGrams = roundedProtein;
                existing.CreatedOn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}