using FluentAssertions;
using SportsDiarys.Models;
using SportsDiarys.Services.Implementations;
using SportsDiarys.Tests.Helpers;
using SportsDiarys.ViewModels.TrainingDiaries;
using Xunit;

namespace SportsDiarys.Tests.Services
{
    public class TrainingDiaryServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldCreateDiary_WhenDataIsValid()
        {
            // Arrange
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            var model = new CreateTrainingDiaryViewModel
            {
                Date = new DateTime(2026, 3, 25),
                Notes = "Test notes",
                Calories = 300,
                DurationMinutes = 45,
                DistanceKm = 2.5,
                WaterLiters = 1.5,
                Place = "Gym"
            };

            int userProfileId = 1;

            // Act
            var id = await service.CreateAsync(model, userProfileId);

            // Assert
            var diary = await context.TrainingDiaries.FindAsync(id);

            diary.Should().NotBeNull();
            diary!.UserProfileId.Should().Be(userProfileId);
            diary.Place.Should().Be("Gym");
            diary.Notes.Should().Be("Test notes");
            diary.DurationMinutes.Should().Be(45);
        }

        [Fact]
        public async Task ExistsForDateAsync_ShouldReturnTrue_WhenDiaryExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Date = new DateTime(2026, 3, 25),
                Name = "Diary",
                Notes = "Test",
                Place = "Gym"
            });

            await context.SaveChangesAsync();

            var result = await service.ExistsForDateAsync(1, new DateTime(2026, 3, 25));

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsForDateAsync_ShouldReturnFalse_WhenNoDiaryExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            var result = await service.ExistsForDateAsync(1, new DateTime(2026, 3, 25));

            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDiary_WhenExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Date = new DateTime(2026, 3, 20),
                Name = "Old",
                Notes = "Old",
                Place = "Home"
            });

            await context.SaveChangesAsync();

            var model = new UpdateTrainingDiaryViewModel
            {
                Id = 1,
                Date = new DateTime(2026, 3, 25),
                Notes = "Updated",
                Calories = 200,
                DurationMinutes = 60,
                DistanceKm = 3,
                WaterLiters = 2,
                Place = "Gym"
            };

            var result = await service.UpdateAsync(model, 1);

            result.Should().BeTrue();

            var diary = await context.TrainingDiaries.FindAsync(1);
            diary!.Place.Should().Be("Gym");
            diary.Notes.Should().Be("Updated");
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenDiaryDoesNotExist()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            var model = new UpdateTrainingDiaryViewModel
            {
                Id = 999,
                Date = DateTime.Now,
                Notes = "Test",
                Place = "Gym"
            };

            var result = await service.UpdateAsync(model, 1);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteDiary_WhenExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Date = DateTime.Now,
                Name = "Diary",
                Place = "Gym"
            });

            await context.SaveChangesAsync();

            var result = await service.DeleteAsync(1, 1);

            result.Should().BeTrue();

            var diary = await context.TrainingDiaries.FindAsync(1);
            diary.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenDiaryDoesNotExist()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            var result = await service.DeleteAsync(999, 1);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetMyDiariesAsync_ShouldReturnOnlyUserDiaries()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            context.TrainingDiaries.AddRange(
                new TrainingDiary { Id = 1, UserProfileId = 1, Date = DateTime.Now, Name = "A" },
                new TrainingDiary { Id = 2, UserProfileId = 2, Date = DateTime.Now, Name = "B" }
            );

            await context.SaveChangesAsync();

            var result = await service.GetMyDiariesAsync(1, null, 1, 10);

            result.Items.Should().HaveCount(1);
            result.Items.First().Id.Should().Be(1);
        }
    }
}