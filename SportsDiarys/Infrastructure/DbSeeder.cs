using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;
using SportsDiarys.Models;
using SportsDiarys.Models.Enums;

namespace SportsDiarys.Infrastructure
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 1️⃣ Seed Exercises
            if (!dbContext.Exercises.Any())
            {
                var exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        Name = "Push-Up",
                        MuscleGroup = "Chest",
                        Difficulty = DifficultyLevel.Easy,
                        Type = ExerciseType.Strength,
                        IsActive = true
                    },
                    new Exercise
                    {
                        Name = "Squat",
                        MuscleGroup = "Legs",
                        Difficulty = DifficultyLevel.Medium,
                        Type = ExerciseType.Strength,
                        IsActive = true
                    },
                    new Exercise
                    {
                        Name = "Plank",
                        MuscleGroup = "Core",
                        Difficulty = DifficultyLevel.Medium,
                        Type = ExerciseType.Strength,
                        IsActive = true
                    }
                };

                dbContext.Exercises.AddRange(exercises);
                await dbContext.SaveChangesAsync();
            }

            // 2️⃣ Seed TrainingDiary & TrainingEntry for Admin user
            var adminProfile = await dbContext.UserProfiles.FirstOrDefaultAsync(up => up.Name == "Administrator");
            if (adminProfile != null && !dbContext.TrainingDiaries.Any(td => td.UserProfileId == adminProfile.Id))
            {
                var diary = new TrainingDiary
                {
                    Name = "Моят дневник",
                    Date = DateTime.Now.Date,
                    DurationMinutes = 60,
                    Place = "Home",
                    WaterLiters = 1.5,
                    UserProfileId = adminProfile.Id
                };

                dbContext.TrainingDiaries.Add(diary);
                await dbContext.SaveChangesAsync();

                var entry = new TrainingEntry
                {
                    SportName = "Morning Workout",
                    DurationMinutes = 30,
                    Calories = 200,
                    TrainingDiaryId = diary.Id
                };

                dbContext.TrainingEntries.Add(entry);
                await dbContext.SaveChangesAsync();

                // 3️⃣ Link Exercises to TrainingEntry
                var pushUp = await dbContext.Exercises.FirstAsync(e => e.Name == "Push-Up");
                var squat = await dbContext.Exercises.FirstAsync(e => e.Name == "Squat");

                var trainingExercises = new List<TrainingEntryExercise>
                {
                    new TrainingEntryExercise
                    {
                        TrainingEntryId = entry.Id,
                        ExerciseId = pushUp.Id,
                        Sets = 3,
                        Reps = 12
                    },
                    new TrainingEntryExercise
                    {
                        TrainingEntryId = entry.Id,
                        ExerciseId = squat.Id,
                        Sets = 3,
                        Reps = 15
                    }
                };

                dbContext.TrainingEntryExercises.AddRange(trainingExercises);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}

