using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data.Configurations;
using SportsDiarys.Models;

namespace SportsDiarys.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        /* public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!;*/

        public virtual DbSet<TrainingDiary> TrainingDiaries { get; set; } = null!;

        public virtual DbSet<TrainingEntry> TrainingEntries { get; set; } = null!;

        /*public virtual DbSet<NutritionTarget> NutritionTargets { get; set; } = null!;*/

        public virtual DbSet<Exercise> Exercises { get; set; } = null!;

        public virtual DbSet<TrainingEntryExercise> TrainingEntryExercises{ get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new TrainingDiaryConfiguration());

            modelBuilder.ApplyConfiguration(new TrainingEntryConfiguration());
            
        }
    }
}
