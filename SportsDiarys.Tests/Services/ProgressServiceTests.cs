using FluentAssertions;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Implementations;
using SportsDiarys.Tests.Helpers;
using Xunit;

namespace SportsDiarys.Tests.Services
{
    public class ProgressServiceTests
    {
        [Fact]
        public async Task GetMyProgressAsync_ShouldReturnZeroStats_WhenUserHasNoData()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ProgressService(context);

            var result = await service.GetMyProgressAsync(1);

            result.Should().NotBeNull();
            result.TotalDiaries.Should().Be(0);
            result.TotalEntries.Should().Be(0);
            result.Last7DaysDurationMinutes.Should().Be(0);
            result.Last30DaysDurationMinutes.Should().Be(0);
            result.Last7DaysCalories.Should().Be(0);
            result.Last30DaysCalories.Should().Be(0);
            result.Last7DaysDistanceKm.Should().Be(0);
            result.Last30DaysDistanceKm.Should().Be(0);
            result.SportBreakdown.Should().BeEmpty();
            result.DailyProgress.Should().HaveCount(7);
            result.MostActiveSport.Should().BeNull();
        }

        [Fact]
        public async Task GetMyProgressAsync_ShouldReturnCorrectTotals_WhenUserHasDiariesAndEntries()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ProgressService(context);

            var today = DateTime.Today;

            var diary1 = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary 1",
                Date = today,
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 400,
                DistanceKm = 5
            };

            var diary2 = new TrainingDiary
            {
                Id = 2,
                UserProfileId = 1,
                Name = "Diary 2",
                Date = today.AddDays(-10),
                Place = "Park",
                DurationMinutes = 30,
                WaterLiters = 1,
                Calories = 200,
                DistanceKm = 2
            };

            context.TrainingDiaries.AddRange(diary1, diary2);

            context.TrainingEntries.AddRange(
                new TrainingEntry
                {
                    Id = 1,
                    SportName = "Running",
                    DurationMinutes = 40,
                    Calories = 300,
                    DistanceKm = 4,
                    TrainingDiaryId = 1,
                    TrainingDiary = diary1
                },
                new TrainingEntry
                {
                    Id = 2,
                    SportName = "Fitness",
                    DurationMinutes = 20,
                    Calories = 100,
                    DistanceKm = 1,
                    TrainingDiaryId = 1,
                    TrainingDiary = diary1
                },
                new TrainingEntry
                {
                    Id = 3,
                    SportName = "Running",
                    DurationMinutes = 30,
                    Calories = 200,
                    DistanceKm = 2,
                    TrainingDiaryId = 2,
                    TrainingDiary = diary2
                });

            await context.SaveChangesAsync();

            var result = await service.GetMyProgressAsync(1);

            result.TotalDiaries.Should().Be(2);
            result.TotalEntries.Should().Be(3);

            result.Last7DaysDurationMinutes.Should().Be(60);
            result.Last30DaysDurationMinutes.Should().Be(90);

            result.Last7DaysCalories.Should().Be(400);
            result.Last30DaysCalories.Should().Be(600);

            result.Last7DaysDistanceKm.Should().Be(5);
            result.Last30DaysDistanceKm.Should().Be(7);
        }

        [Fact]
        public async Task GetMyProgressAsync_ShouldIgnoreOtherUsersData()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ProgressService(context);

            var today = DateTime.Today;

            var myDiary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "My Diary",
                Date = today,
                Place = "Gym",
                DurationMinutes = 45,
                WaterLiters = 2,
                Calories = 250,
                DistanceKm = 3
            };

            var otherDiary = new TrainingDiary
            {
                Id = 2,
                UserProfileId = 2,
                Name = "Other Diary",
                Date = today,
                Place = "Park",
                DurationMinutes = 999,
                WaterLiters = 5,
                Calories = 999,
                DistanceKm = 99
            };

            context.TrainingDiaries.AddRange(myDiary, otherDiary);

            context.TrainingEntries.AddRange(
                new TrainingEntry
                {
                    Id = 1,
                    SportName = "Running",
                    DurationMinutes = 45,
                    Calories = 250,
                    DistanceKm = 3,
                    TrainingDiaryId = 1,
                    TrainingDiary = myDiary
                },
                new TrainingEntry
                {
                    Id = 2,
                    SportName = "Swimming",
                    DurationMinutes = 999,
                    Calories = 999,
                    DistanceKm = 99,
                    TrainingDiaryId = 2,
                    TrainingDiary = otherDiary
                });

            await context.SaveChangesAsync();

            var result = await service.GetMyProgressAsync(1);

            result.TotalDiaries.Should().Be(1);
            result.TotalEntries.Should().Be(1);
            result.Last7DaysDurationMinutes.Should().Be(45);
            result.Last7DaysCalories.Should().Be(250);
            result.Last7DaysDistanceKm.Should().Be(3);
            result.SportBreakdown.Should().HaveCount(1);
            result.SportBreakdown.First().SportName.Should().Be("Running");
        }

        [Fact]
        public async Task GetMyProgressAsync_ShouldReturnSportBreakdownOrderedByEntriesCount()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ProgressService(context);

            var today = DateTime.Today;

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = today,
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 4
            };

            context.TrainingDiaries.Add(diary);

            context.TrainingEntries.AddRange(
                new TrainingEntry
                {
                    Id = 1,
                    SportName = "Running",
                    DurationMinutes = 20,
                    Calories = 100,
                    DistanceKm = 2,
                    TrainingDiaryId = 1,
                    TrainingDiary = diary
                },
                new TrainingEntry
                {
                    Id = 2,
                    SportName = "Running",
                    DurationMinutes = 30,
                    Calories = 150,
                    DistanceKm = 3,
                    TrainingDiaryId = 1,
                    TrainingDiary = diary
                },
                new TrainingEntry
                {
                    Id = 3,
                    SportName = "Fitness",
                    DurationMinutes = 10,
                    Calories = 50,
                    DistanceKm = 0,
                    TrainingDiaryId = 1,
                    TrainingDiary = diary
                });

            await context.SaveChangesAsync();

            var result = await service.GetMyProgressAsync(1);

            result.SportBreakdown.Should().HaveCount(2);

            result.SportBreakdown[0].SportName.Should().Be("Running");
            result.SportBreakdown[0].EntriesCount.Should().Be(2);
            result.SportBreakdown[0].TotalDurationMinutes.Should().Be(50);
            result.SportBreakdown[0].TotalCalories.Should().Be(250);
            result.SportBreakdown[0].TotalDistanceKm.Should().Be(5);

            result.SportBreakdown[1].SportName.Should().Be("Fitness");
            result.MostActiveSport.Should().Be("Running");
        }

        [Fact]
        public async Task GetMyProgressAsync_ShouldReturnDailyProgressForLastSevenDays()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ProgressService(context);

            var today = DateTime.Today;

            context.TrainingDiaries.AddRange(
                new TrainingDiary
                {
                    Id = 1,
                    UserProfileId = 1,
                    Name = "Today",
                    Date = today,
                    Place = "Gym",
                    DurationMinutes = 50,
                    WaterLiters = 2,
                    Calories = 300,
                    DistanceKm = 4
                },
                new TrainingDiary
                {
                    Id = 2,
                    UserProfileId = 1,
                    Name = "Three days ago",
                    Date = today.AddDays(-3),
                    Place = "Park",
                    DurationMinutes = 20,
                    WaterLiters = 1,
                    Calories = 120,
                    DistanceKm = 2
                },
                new TrainingDiary
                {
                    Id = 3,
                    UserProfileId = 1,
                    Name = "Older than 7 days",
                    Date = today.AddDays(-8),
                    Place = "Home",
                    DurationMinutes = 999,
                    WaterLiters = 1,
                    Calories = 999,
                    DistanceKm = 99
                });

            await context.SaveChangesAsync();

            var result = await service.GetMyProgressAsync(1);

            result.DailyProgress.Should().HaveCount(7);

            result.DailyProgress.Last().DurationMinutes.Should().Be(50);
            result.DailyProgress.Last().Calories.Should().Be(300);
            result.DailyProgress.Last().DistanceKm.Should().Be(4);

            result.DailyProgress.Single(x => x.DateLabel == today.AddDays(-3).ToString("dd.MM"))
                .DurationMinutes.Should().Be(20);

            result.DailyProgress.Sum(x => x.DurationMinutes).Should().Be(70);
            result.DailyProgress.Sum(x => x.Calories).Should().Be(420);
            result.DailyProgress.Sum(x => x.DistanceKm).Should().Be(6);
        }

        [Fact]
        public async Task GetMyProgressAsync_ShouldAggregateMultipleDiariesOnSameDay()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ProgressService(context);

            var today = DateTime.Today;

            context.TrainingDiaries.AddRange(
                new TrainingDiary
                {
                    Id = 1,
                    UserProfileId = 1,
                    Name = "Morning",
                    Date = today,
                    Place = "Gym",
                    DurationMinutes = 30,
                    WaterLiters = 1,
                    Calories = 150,
                    DistanceKm = 2
                },
                new TrainingDiary
                {
                    Id = 2,
                    UserProfileId = 1,
                    Name = "Evening",
                    Date = today,
                    Place = "Park",
                    DurationMinutes = 25,
                    WaterLiters = 1,
                    Calories = 100,
                    DistanceKm = 3
                });

            await context.SaveChangesAsync();

            var result = await service.GetMyProgressAsync(1);

            var todayRow = result.DailyProgress.Last();

            todayRow.DurationMinutes.Should().Be(55);
            todayRow.Calories.Should().Be(250);
            todayRow.DistanceKm.Should().Be(5);
        }

        [Fact]
        public async Task GetMyProgressAsync_ShouldReturnEmptyMostActiveSport_WhenNoEntries()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ProgressService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym"
            });

            await context.SaveChangesAsync();

            var result = await service.GetMyProgressAsync(1);

            result.MostActiveSport.Should().BeNull();
        }

        [Fact]
        public async Task GetMyProgressAsync_ShouldHandleNullDistance()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new ProgressService(context);

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym"
            };

            context.TrainingDiaries.Add(diary);

            context.TrainingEntries.Add(new TrainingEntry
            {
                Id = 1,
                SportName = "Running",
                DurationMinutes = 20,
                Calories = 100,
                DistanceKm = null,
                TrainingDiaryId = 1,
                TrainingDiary = diary
            });

            await context.SaveChangesAsync();

            var result = await service.GetMyProgressAsync(1);

            result.Last7DaysDistanceKm.Should().Be(0);
        }
    }
}