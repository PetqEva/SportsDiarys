using FluentAssertions;
using SportsDiarys.Data.Models;
using SportsDiarys.Models.Enums;
using SportsDiarys.Services.Implementations;
using SportsDiarys.Tests.Helpers;
using SportsDiarys.ViewModels.Exercises;
using Xunit;

namespace SportsDiarys.Tests.Services
{
    public class ExerciseServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldCreateExercise_WhenDataIsValid()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ExerciseService(context);

            var model = new ExerciseFormVm
            {
                Name = "Push-Up",
                MuscleGroup = "Chest",
                Description = "Bodyweight exercise",
                Difficulty = 0,
                Type = 0,
                IsActive = true
            };

            var id = await service.CreateAsync(model);

            var entity = await context.Exercises.FindAsync(id);

            entity.Should().NotBeNull();
            entity!.Name.Should().Be("Push-Up");
            entity.MuscleGroup.Should().Be("Chest");
            entity.Description.Should().Be("Bodyweight exercise");
            entity.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenExerciseExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ExerciseService(context);

            context.Exercises.Add(new Exercise
            {
                Id = 1,
                Name = "Old Exercise",
                MuscleGroup = "Legs",
                Description = "Old description",
                Difficulty = DifficultyLevel.Easy,
                Type = ExerciseType.Strength,
                IsActive = true
            });

            await context.SaveChangesAsync();

            var model = new ExerciseFormVm
            {
                Id = 1,
                Name = "Updated Exercise",
                MuscleGroup = "Back",
                Description = "Updated description",
                Difficulty = 2,
                Type = 1,
                IsActive = true
            };

            var result = await service.UpdateAsync(model);

            result.Should().BeTrue();

            var entity = await context.Exercises.FindAsync(1);
            entity.Should().NotBeNull();
            entity!.Name.Should().Be("Updated Exercise");
            entity.MuscleGroup.Should().Be("Back");
            entity.Description.Should().Be("Updated description");
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenExerciseDoesNotExist()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ExerciseService(context);

            var model = new ExerciseFormVm
            {
                Id = 999,
                Name = "Missing",
                MuscleGroup = "Chest",
                Difficulty = 1,
                Type = 0
            };

            var result = await service.UpdateAsync(model);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task SetActiveAsync_ShouldDeactivateExercise_WhenExerciseExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ExerciseService(context);

            context.Exercises.Add(new Exercise
            {
                Id = 1,
                Name = "Push-Up",
                MuscleGroup = "Chest",
                Difficulty = DifficultyLevel.Easy,
                Type = ExerciseType.Strength,
                IsActive = true
            });

            await context.SaveChangesAsync();

            var result = await service.SetActiveAsync(1, false);

            result.Should().BeTrue();

            var entity = await context.Exercises.FindAsync(1);
            entity.Should().NotBeNull();
            entity!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task SetActiveAsync_ShouldReturnFalse_WhenExerciseDoesNotExist()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ExerciseService(context);

            var result = await service.SetActiveAsync(999, false);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenExerciseExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ExerciseService(context);

            context.Exercises.Add(new Exercise
            {
                Id = 1,
                Name = "Squat",
                MuscleGroup = "Legs",
                Difficulty = DifficultyLevel.Medium,
                Type = ExerciseType.Strength,
                IsActive = true
            });

            await context.SaveChangesAsync();

            var result = await service.ExistsAsync(1);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenExerciseDoesNotExist()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ExerciseService(context);

            var result = await service.ExistsAsync(999);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnOnlyMatchingExercises_WhenSearchIsApplied()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ExerciseService(context);

            context.Exercises.AddRange(
                new Exercise
                {
                    Id = 1,
                    Name = "Push-Up",
                    MuscleGroup = "Chest",
                    Difficulty = DifficultyLevel.Easy,
                    Type = ExerciseType.Strength,
                    IsActive = true
                },
                new Exercise
                {
                    Id = 2,
                    Name = "Squat",
                    MuscleGroup = "Legs",
                    Difficulty = DifficultyLevel.Medium,
                    Type = ExerciseType.Strength,
                    IsActive = true
                }
            );

            await context.SaveChangesAsync();

            var query = new ExerciseQueryVm
            {
                Search = "Push",
                Page = 1,
                PageSize = 10
            };

            var result = await service.GetPagedAsync(query);

            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Items.First().Name.Should().Be("Push-Up");
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnCorrectPage()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ExerciseService(context);

            for (int i = 1; i <= 5; i++)
            {
                context.Exercises.Add(new Exercise
                {
                    Id = i,
                    Name = $"Exercise {i}",
                    MuscleGroup = "General",
                    Difficulty = DifficultyLevel.Easy,
                    Type = ExerciseType.Strength,
                    IsActive = true
                });
            }

            await context.SaveChangesAsync();

            var query = new ExerciseQueryVm
            {
                Page = 2,
                PageSize = 2
            };

            var result = await service.GetPagedAsync(query);

            result.Should().NotBeNull();
            result.CurrentPage.Should().Be(2);
            result.Items.Should().HaveCount(2);
        }
    }
}
