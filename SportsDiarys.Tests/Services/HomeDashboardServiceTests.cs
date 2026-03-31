using FluentAssertions;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Implementations;
using SportsDiarys.Tests.Helpers;
using Xunit;

namespace SportsDiarys.Tests.Services
{
    public class HomeDashboardServiceTests
    {
        [Fact]
        public async Task GetDashboardAsync_ShouldReturnDashboard_WhenProfileExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new HomeDashboardService(context);

            var profile = new UserProfile
            {
                Id = 1,
                IdentityUserId = "user-1",
                Name = "Petq",
                Age = 25,
                Gender = "Female",
                StartWeightKg = 65,
                CurrentWeightKg = 63,
                HeightCm = 170,
                ActivityLevel = "Medium"
            };

            context.UserProfiles.Add(profile);
            await context.SaveChangesAsync();

            var result = await service.GetDashboardAsync("user-1");

            result.Should().NotBeNull();
            result.IsAuthenticated.Should().BeTrue();
            result.ProfileName.Should().Be("Petq");
            context.UserProfiles.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetDashboardAsync_ShouldCreateProfile_WhenProfileDoesNotExist()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new HomeDashboardService(context);

            var result = await service.GetDashboardAsync("new-user");

            result.Should().NotBeNull();
            result.IsAuthenticated.Should().BeTrue();
            result.ProfileName.Should().Be("New User");

            context.UserProfiles.Should().HaveCount(1);

            var createdProfile = context.UserProfiles.First();
            createdProfile.IdentityUserId.Should().Be("new-user");
            createdProfile.Name.Should().Be("New User");
        }

        [Fact]
        public async Task GetDashboardAsync_ShouldReturnCorrectStats()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new HomeDashboardService(context);

            var profile = new UserProfile
            {
                Id = 1,
                IdentityUserId = "user-1",
                Name = "Petq",
                Age = 25,
                Gender = "Female",
                StartWeightKg = 65,
                CurrentWeightKg = 63,
                HeightCm = 170,
                ActivityLevel = "Medium"
            };

            var diary1 = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                UserProfile = profile,
                Name = "Diary 1",
                Date = new DateTime(2026, 3, 20),
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 3
            };

            var diary2 = new TrainingDiary
            {
                Id = 2,
                UserProfileId = 1,
                UserProfile = profile,
                Name = "Diary 2",
                Date = new DateTime(2026, 3, 21),
                Place = "Park",
                DurationMinutes = 40,
                WaterLiters = 1.5,
                Calories = 250,
                DistanceKm = 5
            };

            context.UserProfiles.Add(profile);
            context.TrainingDiaries.AddRange(diary1, diary2);

            context.TrainingEntries.AddRange(
                new TrainingEntry
                {
                    Id = 1,
                    SportName = "Running",
                    DurationMinutes = 30,
                    Calories = 200,
                    TrainingDiaryId = 1,
                    TrainingDiary = diary1
                },
                new TrainingEntry
                {
                    Id = 2,
                    SportName = "Cycling",
                    DurationMinutes = 45,
                    Calories = 300,
                    TrainingDiaryId = 2,
                    TrainingDiary = diary2
                },
                new TrainingEntry
                {
                    Id = 3,
                    SportName = "Fitness",
                    DurationMinutes = 20,
                    Calories = 150,
                    TrainingDiaryId = 2,
                    TrainingDiary = diary2
                });

            await context.SaveChangesAsync();

            var result = await service.GetDashboardAsync("user-1");

            result.Should().NotBeNull();
            result.ProfileName.Should().Be("Petq");
            result.DiariesCount.Should().Be(2);
            result.EntriesCount.Should().Be(3);
            result.TotalDurationMinutes.Should().Be(100);
            result.TotalWaterLiters.Should().Be(3.5);
        }

        [Fact]
        public async Task GetDashboardAsync_ShouldReturnLatestFiveDiariesOrderedByDateDescending()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new HomeDashboardService(context);

            var profile = new UserProfile
            {
                Id = 1,
                IdentityUserId = "user-1",
                Name = "Petq",
                Age = 25,
                Gender = "Female",
                StartWeightKg = 65,
                CurrentWeightKg = 63,
                HeightCm = 170,
                ActivityLevel = "Medium"
            };

            context.UserProfiles.Add(profile);

            for (int i = 1; i <= 6; i++)
            {
                context.TrainingDiaries.Add(new TrainingDiary
                {
                    Id = i,
                    UserProfileId = 1,
                    UserProfile = profile,
                    Name = $"Diary {i}",
                    Date = new DateTime(2026, 3, i),
                    Place = $"Place {i}",
                    DurationMinutes = 10 * i,
                    WaterLiters = i,
                    Calories = 100 + i,
                    DistanceKm = i
                });
            }

            await context.SaveChangesAsync();

            var result = await service.GetDashboardAsync("user-1");

            result.Should().NotBeNull();
            result.RecentDiaries.Should().HaveCount(5);

            result.RecentDiaries.Select(d => d.Id)
                .Should()
                .ContainInOrder(6, 5, 4, 3, 2);
        }
    }
}