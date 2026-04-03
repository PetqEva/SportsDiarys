using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;
using SportsDiarys.Data.Models.Enum;

namespace SportsDiarys.Infrastructure
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<AppDbContext>();

            await SeedExercisesAsync(dbContext);
            await SeedAdminDiaryDataAsync(dbContext);
        }

        private static async Task SeedExercisesAsync(AppDbContext dbContext)
        {
            if (await dbContext.Exercises.AnyAsync())
            {
                return;
            }

            var exercises = new List<Exercise>
            {
                new()
                {
                    Name = "Push-Up",
                    MuscleGroup = "Chest",
                    Difficulty = DifficultyLevel.Easy,
                    Type = ExerciseType.Strength,
                    Description = "Bodyweight pushing exercise for chest, shoulders and triceps.",
                    IsActive = true
                },
                new()
                {
                    Name = "Squat",
                    MuscleGroup = "Legs",
                    Difficulty = DifficultyLevel.Medium,
                    Type = ExerciseType.Strength,
                    Description = "Compound lower-body exercise.",
                    IsActive = true
                },
                new()
                {
                    Name = "Plank",
                    MuscleGroup = "Core",
                    Difficulty = DifficultyLevel.Medium,
                    Type = ExerciseType.Strength,
                    Description = "Static core exercise.",
                    IsActive = true
                },
                new()
                {
                    Name = "Burpee",
                    MuscleGroup = "Full Body",
                    Difficulty = DifficultyLevel.Hard,
                    Type = ExerciseType.Cardio,
                    Description = "High intensity full-body movement.",
                    IsActive = true
                },
                new()
                {
                    Name = "Jump Rope",
                    MuscleGroup = "Cardio",
                    Difficulty = DifficultyLevel.Easy,
                    Type = ExerciseType.Cardio,
                    Description = "Cardio endurance exercise.",
                    IsActive = true
                }
            };

            await dbContext.Exercises.AddRangeAsync(exercises);
            await dbContext.SaveChangesAsync();
        }

        private static async Task SeedAdminDiaryDataAsync(AppDbContext dbContext)
        {
            var adminProfile = await dbContext.UserProfiles
                .FirstOrDefaultAsync(x => x.Name == "Administrator");

            if (adminProfile == null)
            {
                return;
            }

            var hasDiary = await dbContext.TrainingDiaries
                .AnyAsync(x => x.UserProfileId == adminProfile.Id);

            if (hasDiary)
            {
                return;
            }

            var diary = new TrainingDiary
            {
                Name = "Начален дневник",
                Date = new DateTime(2026, 3, 20),
                DurationMinutes = 60,
                Place = "Gym",
                WaterLiters = 2,
                Notes = "Seed diary",
                UserProfileId = adminProfile.Id,
                Calories = 500,
                DistanceKm = 3
            };

            await dbContext.TrainingDiaries.AddAsync(diary);
            await dbContext.SaveChangesAsync();

            var entry = new TrainingEntry
            {
                SportName = "Full Body Workout",
                DurationMinutes = 45,
                Calories = 350,
                DistanceKm = 2.5,
                TrainingDiaryId = diary.Id
            };

            await dbContext.TrainingEntries.AddAsync(entry);
            await dbContext.SaveChangesAsync();

            var pushUp = await dbContext.Exercises.FirstAsync(x => x.Name == "Push-Up");
            var squat = await dbContext.Exercises.FirstAsync(x => x.Name == "Squat");

            await dbContext.TrainingEntryExercises.AddRangeAsync(
                new TrainingEntryExercise
                {
                    TrainingEntryId = entry.Id,
                    ExerciseId = pushUp.Id,
                    Sets = 3,
                    Reps = 15
                },
                new TrainingEntryExercise
                {
                    TrainingEntryId = entry.Id,
                    ExerciseId = squat.Id,
                    Sets = 4,
                    Reps = 12
                });

            await dbContext.SaveChangesAsync();
        }
    }
}