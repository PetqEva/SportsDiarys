using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data.Models;
using SportsDiarys.Data.Configurations;
using SportsDiarys.Models;

namespace SportsDiarys.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<TrainingDiary> TrainingDiaries { get; set; } = null!;
        public DbSet<TrainingEntry> TrainingEntries { get; set; } = null!;
        public DbSet<Exercise> Exercises { get; set; } = null!;
        public DbSet<TrainingEntryExercise> TrainingEntryExercises { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new TrainingDiaryConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingEntryConfiguration());

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
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}