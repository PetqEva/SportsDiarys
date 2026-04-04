using FluentAssertions;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Implementations;
using SportsDiarys.Tests.Helpers;
using Xunit;

namespace SportsDiarys.Tests.Services
{
    public class NutritionTargetServiceTests
    {
        [Fact]
        public async Task SaveAsync_ShouldCreateNutritionTarget_WhenMissing()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new NutritionTargetService(context);

            await service.SaveAsync("user-1", 2450.4, 153.6);

            var entity = context.NutritionTargets.SingleOrDefault(x => x.UserId == "user-1");

            entity.Should().NotBeNull();
            entity!.Calories.Should().Be(2450);
            entity.ProteinGrams.Should().Be(154);
        }

        [Fact]
        public async Task SaveAsync_ShouldUpdateNutritionTarget_WhenExisting()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();

            context.NutritionTargets.Add(new NutritionTarget
            {
                Id = 1,
                UserId = "user-1",
                Calories = 2000,
                ProteinGrams = 120,
                CreatedOn = new DateTime(2024, 1, 1)
            });

            await context.SaveChangesAsync();

            var service = new NutritionTargetService(context);

            await service.SaveAsync("user-1", 2600.2, 180.7);

            var allTargets = context.NutritionTargets.Where(x => x.UserId == "user-1").ToList();
            var entity = allTargets.Single();

            allTargets.Should().HaveCount(1);
            entity.Calories.Should().Be(2600);
            entity.ProteinGrams.Should().Be(181);
            entity.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnNull_WhenTargetDoesNotExist()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new NutritionTargetService(context);

            var result = await service.GetByUserIdAsync("missing-user");

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnTarget_WhenTargetExists()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();

            context.NutritionTargets.Add(new NutritionTarget
            {
                Id = 1,
                UserId = "user-1",
                Calories = 2300,
                ProteinGrams = 150
            });

            await context.SaveChangesAsync();

            var service = new NutritionTargetService(context);

            var result = await service.GetByUserIdAsync("user-1");

            result.Should().NotBeNull();
            result!.UserId.Should().Be("user-1");
            result.Calories.Should().Be(2300);
            result.ProteinGrams.Should().Be(150);
        }

        [Fact]
        public async Task SaveAsync_ShouldThrow_WhenUserIdIsNullOrWhiteSpace()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new NutritionTargetService(context);

            var act = async () => await service.SaveAsync("", 2200, 140);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*User id is required*");
        }

        [Fact]
        public async Task SaveAsync_ShouldThrow_WhenCaloriesAreInvalid()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new NutritionTargetService(context);

            var act = async () => await service.SaveAsync("user-1", 0, 140);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Calories must be greater than 0*");
        }

        [Fact]
        public async Task SaveAsync_ShouldThrow_WhenProteinIsInvalid()
        {
            using var context = TestDbHelper.CreateInMemoryDbContext();
            var service = new NutritionTargetService(context);

            var act = async () => await service.SaveAsync("user-1", 2200, 0);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Protein grams must be greater than 0*");
        }
    }
}
