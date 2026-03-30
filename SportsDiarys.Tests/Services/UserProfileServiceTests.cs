using FluentAssertions;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Implementations;
using SportsDiarys.Tests.Helpers;
using Xunit;

namespace SportsDiarys.Tests.Services
{
    public class UserProfileServiceTests
    {
        [Fact]
        public async Task GetMyProfileAsync_ShouldReturnProfile_WhenExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new UserProfileService(context);

            context.UserProfiles.Add(new UserProfile
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
            });

            await context.SaveChangesAsync();

            var result = await service.GetMyProfileAsync("user-1");

            result.Should().NotBeNull();
            result!.IdentityUserId.Should().Be("user-1");
            result.Name.Should().Be("Petq");
        }

        [Fact]
        public async Task GetMyProfileAsync_ShouldReturnNull_WhenProfileDoesNotExist()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new UserProfileService(context);

            var result = await service.GetMyProfileAsync("missing-user");

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetMyProfileWithDiariesAsync_ShouldReturnProfileWithDiaries_WhenExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new UserProfileService(context);

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
                ActivityLevel = "Medium",
                TrainingDiaries = new List<TrainingDiary>
                {
                    new TrainingDiary
                    {
                        Id = 10,
                        Name = "Diary 1",
                        Date = new DateTime(2026, 3, 25),
                        Place = "Gym",
                        TrainingEntries = new List<TrainingEntry>
                        {
                            new TrainingEntry
                            {
                                Id = 100,
                                SportName = "Running",
                                DurationMinutes = 30,
                                Calories = 200,
                                TrainingDiaryId = 10
                            }
                        }
                    }
                }
            };

            context.UserProfiles.Add(profile);
            await context.SaveChangesAsync();

            var result = await service.GetMyProfileWithDiariesAsync("user-1");

            result.Should().NotBeNull();
            result!.TrainingDiaries.Should().HaveCount(1);
            result.TrainingDiaries.First().TrainingEntries.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetMyProfileWithDiariesAsync_ShouldReturnNull_WhenProfileDoesNotExist()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new UserProfileService(context);

            var result = await service.GetMyProfileWithDiariesAsync("missing-user");

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateMyProfileAsync_ShouldCreateProfile_WhenDataIsValid()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new UserProfileService(context);

            var result = await service.CreateMyProfileAsync(
                "user-1",
                "  Petq  ",
                30,
                "Male",
                80,
                78,
                180,
                "Medium");

            result.Should().BeTrue();

            var profile = context.UserProfiles.SingleOrDefault(p => p.IdentityUserId == "user-1");
            profile.Should().NotBeNull();
            profile!.Name.Should().Be("Petq");
            profile.Age.Should().Be(30);
            profile.Gender.Should().Be("Male");
            profile.StartWeightKg.Should().Be(80);
            profile.CurrentWeightKg.Should().Be(78);
            profile.HeightCm.Should().Be(180);
            profile.ActivityLevel.Should().Be("Medium");
        }

        [Fact]
        public async Task CreateMyProfileAsync_ShouldReturnFalse_WhenProfileAlreadyExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new UserProfileService(context);

            context.UserProfiles.Add(new UserProfile
            {
                Id = 1,
                IdentityUserId = "user-1",
                Name = "Existing",
                Age = 25,
                Gender = "Female",
                StartWeightKg = 60,
                CurrentWeightKg = 59,
                HeightCm = 165,
                ActivityLevel = "Low"
            });

            await context.SaveChangesAsync();

            var result = await service.CreateMyProfileAsync(
                "user-1",
                "New Name",
                30,
                "Male",
                80,
                78,
                180,
                "Medium");

            result.Should().BeFalse();
            context.UserProfiles.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateMyProfileAsync_ShouldReturnFalse_WhenActivityLevelIsInvalid()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new UserProfileService(context);

            var result = await service.CreateMyProfileAsync(
                "user-1",
                "Petq",
                30,
                "Male",
                80,
                78,
                180,
                "Extreme");

            result.Should().BeFalse();
            context.UserProfiles.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateMyProfileAsync_ShouldUpdateProfile_WhenDataIsValid()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new UserProfileService(context);

            context.UserProfiles.Add(new UserProfile
            {
                Id = 1,
                IdentityUserId = "user-1",
                Name = "Old Name",
                Age = 25,
                Gender = "Female",
                StartWeightKg = 60,
                CurrentWeightKg = 59,
                HeightCm = 165,
                ActivityLevel = "Low"
            });

            await context.SaveChangesAsync();

            var result = await service.UpdateMyProfileAsync(
                "user-1",
                "  Updated Name  ",
                30,
                "Male",
                80,
                78,
                180,
                "Medium");

            result.Should().BeTrue();

            var profile = context.UserProfiles.Single(p => p.IdentityUserId == "user-1");
            profile.Name.Should().Be("Updated Name");
            profile.Age.Should().Be(30);
            profile.Gender.Should().Be("Male");
            profile.StartWeightKg.Should().Be(80);
            profile.CurrentWeightKg.Should().Be(78);
            profile.HeightCm.Should().Be(180);
            profile.ActivityLevel.Should().Be("Medium");
        }

        [Fact]
        public async Task UpdateMyProfileAsync_ShouldReturnFalse_WhenProfileDoesNotExist()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new UserProfileService(context);

            var result = await service.UpdateMyProfileAsync(
                "missing-user",
                "Name",
                30,
                "Male",
                80,
                78,
                180,
                "Medium");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateMyProfileAsync_ShouldReturnFalse_WhenActivityLevelIsInvalid()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new UserProfileService(context);

            context.UserProfiles.Add(new UserProfile
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
            });

            await context.SaveChangesAsync();

            var result = await service.UpdateMyProfileAsync(
                "user-1",
                "Petq",
                31,
                "Male",
                81,
                79,
                180,
                "VeryHigh");

            result.Should().BeFalse();

            var profile = context.UserProfiles.Single(p => p.IdentityUserId == "user-1");
            profile.ActivityLevel.Should().Be("Medium");
            profile.Age.Should().Be(30);
        }

        [Fact]
        public async Task DeleteMyProfileAsync_ShouldDeleteProfileAndRelatedData_WhenProfileExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new UserProfileService(context);

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
                ActivityLevel = "Medium",
                TrainingDiaries = new List<TrainingDiary>
                {
                    new TrainingDiary
                    {
                        Id = 10,
                        Name = "Diary 1",
                        Date = new DateTime(2026, 3, 25),
                        Place = "Gym",
                        UserProfileId = 1,
                        TrainingEntries = new List<TrainingEntry>
                        {
                            new TrainingEntry
                            {
                                Id = 100,
                                SportName = "Running",
                                DurationMinutes = 30,
                                Calories = 200,
                                TrainingDiaryId = 10
                            }
                        }
                    }
                }
            };

            context.UserProfiles.Add(profile);
            await context.SaveChangesAsync();

            var result = await service.DeleteMyProfileAsync("user-1");

            result.Should().BeTrue();
            context.UserProfiles.Should().BeEmpty();
            context.TrainingDiaries.Should().BeEmpty();
            context.TrainingEntries.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteMyProfileAsync_ShouldReturnFalse_WhenProfileDoesNotExist()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new UserProfileService(context);

            var result = await service.DeleteMyProfileAsync("missing-user");

            result.Should().BeFalse();
        }


    }
}