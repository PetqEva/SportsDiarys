using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data.Models;
using SportsDiarys.Models;
using SportsDiarys.Models.Enums;

namespace SportsDiarys.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ========================
        // DbSets
        // ========================
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<TrainingDiary> TrainingDiaries { get; set; } = null!;
        public DbSet<TrainingEntry> TrainingEntries { get; set; } = null!;
        public DbSet<Exercise> Exercises { get; set; } = null!;
        public DbSet<TrainingEntryExercise> TrainingEntryExercises { get; set; } = null!;

        // ========================
        // Model Configuration
        // ========================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureTrainingEntryExercise(modelBuilder);
            ConfigureSeedData(modelBuilder);
        }

        // ========================
        // Many-to-Many Configuration
        // ========================
        private void ConfigureTrainingEntryExercise(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrainingEntryExercise>(entity =>
            {
                entity.HasKey(te => new { te.TrainingEntryId, te.ExerciseId });

                entity.HasOne(te => te.TrainingEntry)
                      .WithMany(t => t.TrainingEntryExercises)
                      .HasForeignKey(te => te.TrainingEntryId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(te => te.Exercise)
                      .WithMany(e => e.TrainingEntryExercises)
                      .HasForeignKey(te => te.ExerciseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        // ========================
        // Seed Data
        // ========================
        private void ConfigureSeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise
                {
                    Id = 1,
                    Name = "Push-Up",
                    MuscleGroup = "Chest",
                    Type = ExerciseType.Strength,
                    Difficulty = DifficultyLevel.Easy,
                    IsActive = true
                },
                new Exercise
                {
                    Id = 2,
                    Name = "Squat",
                    MuscleGroup = "Legs",
                    Type = ExerciseType.Strength,
                    Difficulty = DifficultyLevel.Medium,
                    IsActive = true
                },
                new Exercise
                {
                    Id = 3,
                    Name = "Plank",
                    MuscleGroup = "Core",
                    Type = ExerciseType.Strength,
                    Difficulty = DifficultyLevel.Medium,
                    IsActive = true
                }
            );
        }
    }
}