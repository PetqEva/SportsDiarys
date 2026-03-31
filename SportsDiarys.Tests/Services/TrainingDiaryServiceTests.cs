using FluentAssertions;
using SportsDiarys.Data.Models;
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
            diary.Calories.Should().Be(300);
            diary.DistanceKm.Should().Be(2.5);
            diary.WaterLiters.Should().Be(1.5);
            diary.Name.Should().NotBeNullOrWhiteSpace();
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
        public async Task ExistsForDateAsync_ShouldReturnFalse_WhenMatchingDiaryIsExcluded()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 5,
                UserProfileId = 1,
                Date = new DateTime(2026, 3, 25),
                Name = "Diary",
                Place = "Gym"
            });

            await context.SaveChangesAsync();

            var result = await service.ExistsForDateAsync(1, new DateTime(2026, 3, 25), 5);

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
            diary.DistanceKm.Should().Be(3);
            diary.WaterLiters.Should().Be(2);
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
        public async Task UpdateAsync_ShouldReturnFalse_WhenDiaryBelongsToAnotherUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 1,
                UserProfileId = 2,
                Date = new DateTime(2026, 3, 20),
                Name = "Other user diary",
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
        public async Task DeleteAsync_ShouldReturnFalse_WhenDiaryBelongsToAnotherUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 1,
                UserProfileId = 2,
                Date = DateTime.Now,
                Name = "Diary",
                Place = "Gym"
            });

            await context.SaveChangesAsync();

            var result = await service.DeleteAsync(1, 1);

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
            result.TotalCount.Should().Be(1);
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
            result.Items.First().Id.Should().Be(1);
        }

        [Fact]
        public async Task GetMyDiariesAsync_ShouldNormalizeInvalidPageAndPageSize()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            for (int i = 1; i <= 12; i++)
            {
                context.TrainingDiaries.Add(new TrainingDiary
                {
                    Id = i,
                    UserProfileId = 1,
                    Name = $"Diary {i}",
                    Date = new DateTime(2026, 3, i),
                    Place = "Gym"
                });
            }

            await context.SaveChangesAsync();

            var result = await service.GetMyDiariesAsync(1, null, 0, 0);

            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.Items.Should().HaveCount(10);
            result.TotalCount.Should().Be(12);
        }

        [Fact]
        public async Task GetForEditAsync_ShouldReturnDiary_WhenOwnedByUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Date = new DateTime(2026, 3, 25),
                Name = "Diary",
                Notes = "Notes",
                Calories = 150,
                DurationMinutes = 35,
                DistanceKm = 4,
                WaterLiters = 2,
                Place = "Gym"
            });

            await context.SaveChangesAsync();

            var result = await service.GetForEditAsync(1, 1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Place.Should().Be("Gym");
            result.DurationMinutes.Should().Be(35);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDiaryDetails_WhenOwnedByUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            var user = new ApplicationUser
            {
                Id = "user-1",
                UserName = "petq@abv.bg",
                Email = "petq@abv.bg"
            };

            var profile = new UserProfile
            {
                Id = 1,
                IdentityUserId = "user-1",
                Name = "Petq",
                Age = 30,
                Gender = "Male",
                StartWeightKg = 80,
                CurrentWeightKg = 78,
                HeightCm = 180,
                ActivityLevel = "Medium"
            };

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                UserProfile = profile,
                Name = "Diary",
                Date = new DateTime(2026, 3, 25),
                Notes = "My notes",
                Place = "Gym",
                TrainingEntries = new List<TrainingEntry>
        {
            new TrainingEntry
            {
                Id = 11,
                SportName = "Running",
                DurationMinutes = 30,
                Calories = 200,
                DistanceKm = 4,
                TrainingDiaryId = 1
            },
            new TrainingEntry
            {
                Id = 12,
                SportName = "Cycling",
                DurationMinutes = 45,
                Calories = 300,
                DistanceKm = 8,
                TrainingDiaryId = 1
            }
        }
            };

            context.Users.Add(user);
            context.UserProfiles.Add(profile);
            context.TrainingDiaries.Add(diary);
            await context.SaveChangesAsync();

            var result = await service.GetByIdAsync(1, 1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.UserName.Should().Be("Petq");
            result.Place.Should().Be("Gym");
            result.TotalEntries.Should().Be(2);
            result.TotalDurationMinutes.Should().Be(75);
            result.TotalCalories.Should().Be(500);
            result.TotalDistanceKm.Should().Be(12);
        }

        [Fact]
        public async Task ExistsForDateAsync_ShouldIgnoreCurrentDiary_WhenExcludeIdIsPassed()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingDiaryService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Date = DateTime.Today,
                Name = "Test",
                Place = "Gym"
            });

            await context.SaveChangesAsync();

            var result = await service.ExistsForDateAsync(1, DateTime.Today, 1);

            result.Should().BeFalse();
        }
    }
}