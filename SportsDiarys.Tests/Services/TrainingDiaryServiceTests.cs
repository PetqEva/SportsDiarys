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

            var id = await service.CreateAsync(model, userProfileId);

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
            diary.Should().NotBeNull();
            diary!.Place.Should().Be("Gym");
            diary.Notes.Should().Be("Updated");
            diary.DurationMinutes.Should().Be(60);
            diary.Calories.Should().Be(200);
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
                new TrainingDiary
                {
                    Id = 1,
                    UserProfileId = 1,
                    Name = "Diary A",
                    Date = new DateTime(2026, 3, 25),
                    Notes = "Test A",
                    Place = "Gym",
                    DurationMinutes = 45,
                    Calories = 300,
                    DistanceKm = 2.5,
                    WaterLiters = 1.5
                },
                new TrainingDiary
                {
                    Id = 2,
                    UserProfileId = 2,
                    Name = "Diary B",
                    Date = new DateTime(2026, 3, 25),
                    Notes = "Test B",
                    Place = "Park",
                    DurationMinutes = 30,
                    Calories = 200,
                    DistanceKm = 4,
                    WaterLiters = 1
                });

            await context.SaveChangesAsync();

            var result = await service.GetMyDiariesAsync(1, null, 1, 10);

            result.Items.Should().HaveCount(1);
            result.Items.First().Id.Should().Be(1);
        }

        [Fact]
        public async Task GetMyDiariesAsync_ShouldFilterBySearch()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            context.TrainingDiaries.AddRange(
                new TrainingDiary
                {
                    Id = 1,
                    UserProfileId = 1,
                    Name = "Gym Day",
                    Date = DateTime.Today,
                    Notes = "Strength training",
                    Place = "Fitness",
                    DurationMinutes = 60,
                    Calories = 350,
                    DistanceKm = 2,
                    WaterLiters = 2
                },
                new TrainingDiary
                {
                    Id = 2,
                    UserProfileId = 1,
                    Name = "Running Day",
                    Date = DateTime.Today,
                    Notes = "Cardio session",
                    Place = "Park",
                    DurationMinutes = 30,
                    Calories = 250,
                    DistanceKm = 5,
                    WaterLiters = 1
                });

            await context.SaveChangesAsync();

            var result = await service.GetMyDiariesAsync(1, "Gym", 1, 10);

            result.Items.Should().HaveCount(1);
            result.Items.First().Place.Should().Be("Fitness");
        }
    }
}