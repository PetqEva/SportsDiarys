using FluentAssertions;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Implementations;
using SportsDiarys.Tests.Helpers;
using SportsDiarys.ViewModels.TrainingEntries;
using Xunit;

namespace SportsDiarys.Tests.Services
{
    public class TrainingEntryServiceTests
    {
        [Fact]
        public void ValidateBusinessRules_ShouldReturnErrors_WhenDataIsInvalid()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var vm = new TrainingEntryFormVm
            {
                SportName = " ",
                DurationMinutes = 0,
                Calories = -5
            };

            var errors = service.ValidateBusinessRules(vm).ToList();

            errors.Should().HaveCount(4);
            errors.Should().Contain(e => e.Field == nameof(vm.SportName));
            errors.Should().Contain(e => e.Field == nameof(vm.DurationMinutes));
            errors.Should().Contain(e => e.Field == nameof(vm.Calories));
            errors.Should().Contain(e => e.Field == nameof(vm.TrainingDiaryId));
        }
        [Fact]
        public async Task CreateAsync_ShouldCreateEntry_WhenDiaryBelongsToUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = new DateTime(2026, 3, 20),
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 2
            });

            await context.SaveChangesAsync();

            var entry = new TrainingEntry
            {
                SportName = "  Running  ",
                DurationMinutes = 30,
                Calories = 250,
                DistanceKm = 5,
                TrainingDiaryId = 1
            };

            var id = await service.CreateAsync(entry, 1);

            var created = await context.TrainingEntries.FindAsync(id);

            created.Should().NotBeNull();
            created!.SportName.Should().Be("Running");
            created.DurationMinutes.Should().Be(30);
            created.Calories.Should().Be(250);
            created.DistanceKm.Should().Be(5);
            created.TrainingDiaryId.Should().Be(1);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenDiaryDoesNotBelongToUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 1,
                UserProfileId = 2,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 2
            });

            await context.SaveChangesAsync();

            var entry = new TrainingEntry
            {
                SportName = "Cycling",
                DurationMinutes = 45,
                Calories = 400,
                TrainingDiaryId = 1
            };

            var act = async () => await service.CreateAsync(entry, 1);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_AndUpdateEntry_WhenOwnedByUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 2
            };

            var entry = new TrainingEntry
            {
                Id = 1,
                SportName = "Old",
                DurationMinutes = 20,
                Calories = 100,
                DistanceKm = 1,
                TrainingDiaryId = 1,
                TrainingDiary = diary
            };

            context.TrainingDiaries.Add(diary);
            context.TrainingEntries.Add(entry);
            await context.SaveChangesAsync();

            var updated = new TrainingEntry
            {
                Id = 1,
                SportName = "  New Sport  ",
                DurationMinutes = 50,
                Calories = 350,
                DistanceKm = 7,
                TrainingDiaryId = 1
            };

            var result = await service.UpdateAsync(updated, 1);

            result.Should().BeTrue();

            var dbEntry = await context.TrainingEntries.FindAsync(1);
            dbEntry.Should().NotBeNull();
            dbEntry!.SportName.Should().Be("New Sport");
            dbEntry.DurationMinutes.Should().Be(50);
            dbEntry.Calories.Should().Be(350);
            dbEntry.DistanceKm.Should().Be(7);
            dbEntry.TrainingDiaryId.Should().Be(1);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenEntryIsNotOwnedByUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 2,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 2
            };

            context.TrainingDiaries.Add(diary);
            context.TrainingEntries.Add(new TrainingEntry
            {
                Id = 1,
                SportName = "Old",
                DurationMinutes = 20,
                Calories = 100,
                TrainingDiaryId = 1,
                TrainingDiary = diary
            });

            await context.SaveChangesAsync();

            var result = await service.UpdateAsync(new TrainingEntry
            {
                Id = 1,
                SportName = "New",
                DurationMinutes = 30,
                Calories = 150,
                TrainingDiaryId = 1
            }, 1);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenEntryExistsAndBelongsToUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 2
            };

            context.TrainingDiaries.Add(diary);
            context.TrainingEntries.Add(new TrainingEntry
            {
                Id = 1,
                SportName = "Run",
                DurationMinutes = 20,
                Calories = 100,
                TrainingDiaryId = 1,
                TrainingDiary = diary
            });

            await context.SaveChangesAsync();

            var result = await service.DeleteAsync(1, 1);

            result.Should().BeTrue();
            (await context.TrainingEntries.FindAsync(1)).Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenEntryDoesNotBelongToUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 2,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 2
            };

            context.TrainingDiaries.Add(diary);
            context.TrainingEntries.Add(new TrainingEntry
            {
                Id = 1,
                SportName = "Run",
                DurationMinutes = 20,
                Calories = 100,
                TrainingDiaryId = 1,
                TrainingDiary = diary
            });

            await context.SaveChangesAsync();

            var result = await service.DeleteAsync(1, 1);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetMyEntriesPagedAsync_ShouldFilterBySport_AndReturnPagedItems()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = new DateTime(2026, 3, 20),
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 2
            };

            context.TrainingDiaries.Add(diary);
            context.TrainingEntries.AddRange(
                new TrainingEntry
                {
                    Id = 1,
                    SportName = "Running",
                    DurationMinutes = 30,
                    Calories = 200,
                    TrainingDiaryId = 1,
                    TrainingDiary = diary
                },
                new TrainingEntry
                {
                    Id = 2,
                    SportName = "Swimming",
                    DurationMinutes = 40,
                    Calories = 250,
                    TrainingDiaryId = 1,
                    TrainingDiary = diary
                });

            await context.SaveChangesAsync();

            var query = new EntriesQueryVm
            {
                Sport = "Run",
                Page = 1,
                PageSize = 10
            };

            var result = await service.GetMyEntriesPagedAsync(1, query);

            result.Items.Should().HaveCount(1);
            result.Items.First().SportName.Should().Be("Running");
            result.Page.Should().Be(1);
        }

        [Fact]
        public async Task GetMyEntriesAsync_ShouldReturnOnlyEntriesForCurrentUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var myDiary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "My Diary",
                Date = new DateTime(2026, 3, 20),
                Place = "Gym"
            };

            var otherDiary = new TrainingDiary
            {
                Id = 2,
                UserProfileId = 2,
                Name = "Other Diary",
                Date = new DateTime(2026, 3, 21),
                Place = "Park"
            };

            context.TrainingDiaries.AddRange(myDiary, otherDiary);

            context.TrainingEntries.AddRange(
                new TrainingEntry
                {
                    Id = 1,
                    SportName = "Running",
                    DurationMinutes = 30,
                    Calories = 200,
                    TrainingDiaryId = 1,
                    TrainingDiary = myDiary
                },
                new TrainingEntry
                {
                    Id = 2,
                    SportName = "Cycling",
                    DurationMinutes = 40,
                    Calories = 300,
                    TrainingDiaryId = 2,
                    TrainingDiary = otherDiary
                });

            await context.SaveChangesAsync();

            var result = await service.GetMyEntriesAsync(1);

            result.Should().HaveCount(1);
            result.First().Id.Should().Be(1);
        }

        [Fact]
        public async Task DiaryBelongsToMeAsync_ShouldReturnTrue_WhenDiaryIsOwnedByUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 2
            });

            await context.SaveChangesAsync();

            var result = await service.DiaryBelongsToMeAsync(1, 1);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DiaryBelongsToMeAsync_ShouldReturnFalse_WhenDiaryIsNotOwnedByUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            context.TrainingDiaries.Add(new TrainingDiary
            {
                Id = 1,
                UserProfileId = 2,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym"
            });

            await context.SaveChangesAsync();

            var result = await service.DiaryBelongsToMeAsync(1, 1);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetActiveExercisesAsync_ShouldReturnOnlyActiveExercises()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            context.Exercises.AddRange(
                new Exercise
                {
                    Id = 1,
                    Name = "Push-Up",
                    MuscleGroup = "Chest",
                    IsActive = true
                },
                new Exercise
                {
                    Id = 2,
                    Name = "Old Exercise",
                    MuscleGroup = "Back",
                    IsActive = false
                });

            await context.SaveChangesAsync();

            var result = (await service.GetActiveExercisesAsync()).ToList();

            result.Should().HaveCount(1);
            result[0].Name.Should().Be("Push-Up");
        }

        [Fact]
        public async Task AddExerciseAsync_ShouldReturnTrue_WhenEntryBelongsToUser_AndExerciseNotAddedYet()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 2
            };

            var entry = new TrainingEntry
            {
                Id = 1,
                SportName = "Workout",
                DurationMinutes = 45,
                Calories = 350,
                TrainingDiaryId = 1,
                TrainingDiary = diary
            };

            var exercise = new Exercise
            {
                Id = 1,
                Name = "Squat",
                MuscleGroup = "Legs",
                IsActive = true
            };

            context.TrainingDiaries.Add(diary);
            context.TrainingEntries.Add(entry);
            context.Exercises.Add(exercise);
            await context.SaveChangesAsync();

            var model = new AddExerciseToEntryVm
            {
                TrainingEntryId = 1,
                ExerciseId = 1,
                Sets = 4,
                Reps = 10
            };

            var result = await service.AddExerciseAsync(model, 1);

            result.Should().BeTrue();
            context.TrainingEntryExercises.Should().ContainSingle();
        }

        [Fact]
        public async Task AddExerciseAsync_ShouldReturnFalse_WhenExerciseAlreadyExistsInEntry()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 2
            };

            var entry = new TrainingEntry
            {
                Id = 1,
                SportName = "Workout",
                DurationMinutes = 45,
                Calories = 350,
                TrainingDiaryId = 1,
                TrainingDiary = diary
            };

            var exercise = new Exercise
            {
                Id = 1,
                Name = "Bench Press",
                MuscleGroup = "Chest",
                IsActive = true
            };

            context.TrainingDiaries.Add(diary);
            context.TrainingEntries.Add(entry);
            context.Exercises.Add(exercise);
            context.TrainingEntryExercises.Add(new TrainingEntryExercise
            {
                TrainingEntryId = 1,
                ExerciseId = 1,
                Sets = 3,
                Reps = 8
            });

            await context.SaveChangesAsync();

            var result = await service.AddExerciseAsync(new AddExerciseToEntryVm
            {
                TrainingEntryId = 1,
                ExerciseId = 1,
                Sets = 3,
                Reps = 8
            }, 1);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveExerciseAsync_ShouldReturnTrue_WhenExerciseExistsInEntry_AndEntryBelongsToUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym"
            };

            var entry = new TrainingEntry
            {
                Id = 1,
                SportName = "Workout",
                DurationMinutes = 45,
                Calories = 350,
                TrainingDiaryId = 1,
                TrainingDiary = diary
            };

            context.TrainingDiaries.Add(diary);
            context.TrainingEntries.Add(entry);
            context.TrainingEntryExercises.Add(new TrainingEntryExercise
            {
                TrainingEntryId = 1,
                ExerciseId = 1,
                Sets = 3,
                Reps = 10
            });

            await context.SaveChangesAsync();

            var result = await service.RemoveExerciseAsync(1, 1, 1);

            result.Should().BeTrue();
            context.TrainingEntryExercises.Should().BeEmpty();
        }

        [Fact]
        public async Task GetMyEntryDetailsAsync_ShouldReturnNull_WhenEntryDoesNotBelongToUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var userProfile = new UserProfile
            {
                Id = 2,
                IdentityUserId = "user-2",
                Name = "Other User",
                Age = 25,
                Gender = "Female",
                StartWeightKg = 60,
                CurrentWeightKg = 58,
                HeightCm = 165,
                ActivityLevel = "Active"
            };

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 2,
                UserProfile = userProfile,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym",
                DurationMinutes = 60,
                WaterLiters = 2,
                Calories = 300,
                DistanceKm = 2
            };

            context.UserProfiles.Add(userProfile);
            context.TrainingDiaries.Add(diary);
            context.TrainingEntries.Add(new TrainingEntry
            {
                Id = 1,
                SportName = "Workout",
                DurationMinutes = 45,
                Calories = 350,
                TrainingDiaryId = 1,
                TrainingDiary = diary
            });

            await context.SaveChangesAsync();

            var result = await service.GetMyEntryDetailsAsync(1, 1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetMyEntriesPagedAsync_ShouldReturnAllOwnedEntries_WhenNoFilters()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = new DateTime(2026, 3, 20),
                Place = "Gym"
            };

            context.TrainingDiaries.Add(diary);
            context.TrainingEntries.AddRange(
                new TrainingEntry
                {
                    Id = 1,
                    SportName = "Running",
                    DurationMinutes = 30,
                    Calories = 200,
                    TrainingDiaryId = 1,
                    TrainingDiary = diary
                },
                new TrainingEntry
                {
                    Id = 2,
                    SportName = "Cycling",
                    DurationMinutes = 40,
                    Calories = 300,
                    TrainingDiaryId = 1,
                    TrainingDiary = diary
                });

            await context.SaveChangesAsync();

            var query = new EntriesQueryVm
            {
                Page = 1,
                PageSize = 10
            };

            var result = await service.GetMyEntriesPagedAsync(1, query);

            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Page.Should().Be(1);
        }

        [Fact]
        public async Task GetMyEntriesPagedAsync_ShouldReturnSecondPage_WhenPagingIsApplied()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 1,
                Name = "Diary",
                Date = new DateTime(2026, 3, 20),
                Place = "Gym"
            };

            context.TrainingDiaries.Add(diary);

            for (int i = 1; i <= 6; i++)
            {
                context.TrainingEntries.Add(new TrainingEntry
                {
                    Id = i,
                    SportName = $"Sport {i}",
                    DurationMinutes = 20 + i,
                    Calories = 100 + i,
                    TrainingDiaryId = 1,
                    TrainingDiary = diary
                });
            }

            await context.SaveChangesAsync();

            var query = new EntriesQueryVm
            {
                Page = 2,
                PageSize = 5
            };

            var result = await service.GetMyEntriesPagedAsync(1, query);

            result.Should().NotBeNull();
            result.Page.Should().Be(2);
            result.PageSize.Should().Be(5);
            result.TotalCount.Should().Be(6);
            result.Items.Should().HaveCount(1);
            result.Items.First().Id.Should().Be(1);
        }

        [Fact]
        public async Task GetMyEntriesPagedAsync_ShouldReturnEmpty_WhenUserHasNoEntries()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var query = new EntriesQueryVm
            {
                Page = 1,
                PageSize = 10
            };

            var result = await service.GetMyEntriesPagedAsync(1, query);

            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenEntryDoesNotExist()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var result = await service.DeleteAsync(999, 1);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetMyEntryDetailsAsync_ShouldReturnDetails_WhenEntryBelongsToUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var userProfile = new UserProfile
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
                UserProfile = userProfile,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym"
            };

            var entry = new TrainingEntry
            {
                Id = 1,
                SportName = "Running",
                DurationMinutes = 35,
                Calories = 250,
                DistanceKm = 5,
                TrainingDiaryId = 1,
                TrainingDiary = diary
            };

            context.UserProfiles.Add(userProfile);
            context.TrainingDiaries.Add(diary);
            context.TrainingEntries.Add(entry);
            await context.SaveChangesAsync();

            var result = await service.GetMyEntryDetailsAsync(1, 1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.SportName.Should().Be("Running");
            result.DurationMinutes.Should().Be(35);
            result.Calories.Should().Be(250);
        }

        [Fact]
        public async Task AddExerciseAsync_ShouldReturnFalse_WhenEntryDoesNotBelongToUser()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new TrainingEntryService(context);

            var diary = new TrainingDiary
            {
                Id = 1,
                UserProfileId = 2,
                Name = "Diary",
                Date = DateTime.Today,
                Place = "Gym"
            };

            var entry = new TrainingEntry
            {
                Id = 1,
                SportName = "Workout",
                DurationMinutes = 45,
                Calories = 300,
                TrainingDiaryId = 1,
                TrainingDiary = diary
            };

            var exercise = new Exercise
            {
                Id = 1,
                Name = "Squat",
                MuscleGroup = "Legs",
                IsActive = true
            };

            context.TrainingDiaries.Add(diary);
            context.TrainingEntries.Add(entry);
            context.Exercises.Add(exercise);
            await context.SaveChangesAsync();

            var model = new AddExerciseToEntryVm
            {
                TrainingEntryId = 1,
                ExerciseId = 1,
                Sets = 3,
                Reps = 10
            };

            var result = await service.AddExerciseAsync(model, 1);

            result.Should().BeFalse();
        }
    }
}