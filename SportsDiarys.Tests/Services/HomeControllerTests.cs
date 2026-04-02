using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsDiarys.Controllers;
using SportsDiarys.Data.Models;
using SportsDiarys.Infrastructure;
using SportsDiarys.Models;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.Home;
using Xunit;

namespace SportsDiarys.Tests.Controllers
{
    public class HomeControllerTests
    {
        private class FakeHomeDashboardService : IHomeDashboardService
        {
            private readonly HomeDashboardVm _result;

            public FakeHomeDashboardService(HomeDashboardVm result)
            {
                _result = result;
            }

            public Task<HomeDashboardVm> GetDashboardAsync(string userId)
            {
                return Task.FromResult(_result);
            }
        }

        private class FakeUserStore : IUserStore<ApplicationUser>
        {
            public void Dispose() { }

            public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
                => Task.FromResult(user.Id);

            public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
                => Task.FromResult(user.UserName);

            public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
            {
                user.UserName = userName;
                return Task.CompletedTask;
            }

            public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
                => Task.FromResult(user.NormalizedUserName);

            public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
            {
                user.NormalizedUserName = normalizedName;
                return Task.CompletedTask;
            }

            public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
                => Task.FromResult(IdentityResult.Success);

            public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
                => Task.FromResult(IdentityResult.Success);

            public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
                => Task.FromResult(IdentityResult.Success);

            public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
                => Task.FromResult<ApplicationUser?>(null);

            public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
                => Task.FromResult<ApplicationUser?>(null);
        }

        private class FakeUserManager : UserManager<ApplicationUser>
        {
            private readonly ApplicationUser? _user;
            private readonly bool _isInRole;
            public bool AddToRoleCalled { get; private set; }

            public FakeUserManager(ApplicationUser? user, bool isInRole)
                 : base(
                     new FakeUserStore(),
                     null!,
                     null!,
                     null!,
                     null!,
                     null!,
                     null!,
                     null!,
                     null!)
                        {
                            _user = user;
                            _isInRole = isInRole;
                        }

            public override Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal)
            {
                return Task.FromResult(_user);
            }

            public override Task<bool> IsInRoleAsync(ApplicationUser user, string role)
            {
                return Task.FromResult(_isInRole);
            }

            public override Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
            {
                AddToRoleCalled = true;
                return Task.FromResult(IdentityResult.Success);
            }
        }

        private static HomeController CreateController(
            IHomeDashboardService dashboardService,
            UserManager<ApplicationUser> userManager,
            ClaimsPrincipal? user = null)
        {
            var controller = new HomeController(dashboardService, userManager);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user ?? new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            return controller;
        }

        [Fact]
        public async Task Index_ShouldReturnUnauthenticatedModel_WhenUserIsAnonymous()
        {
            var dashboardService = new FakeHomeDashboardService(new HomeDashboardVm());
            var userManager = new FakeUserManager(null, false);
            var controller = CreateController(dashboardService, userManager);

            var result = await controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<HomeDashboardVm>().Subject;

            model.IsAuthenticated.Should().BeFalse();
        }

        [Fact]
        public async Task Index_ShouldReturnDashboard_WhenUserIsAuthenticated_AndAlreadyInUserRole()
        {
            var vm = new HomeDashboardVm
            {
                IsAuthenticated = true,
                ProfileName = "Petq",
                DiariesCount = 2,
                EntriesCount = 3
            };

            var dashboardService = new FakeHomeDashboardService(vm);

            var appUser = new ApplicationUser
            {
                Id = "user-1",
                UserName = "petq@test.bg",
                Email = "petq@test.bg"
            };

            var userManager = new FakeUserManager(appUser, true);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user-1")
                },
                "TestAuth"));

            var controller = CreateController(dashboardService, userManager, principal);

            var result = await controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<HomeDashboardVm>().Subject;

            model.IsAuthenticated.Should().BeTrue();
            model.ProfileName.Should().Be("Petq");
            model.DiariesCount.Should().Be(2);
            model.EntriesCount.Should().Be(3);
            userManager.AddToRoleCalled.Should().BeFalse();
        }

        [Fact]
        public async Task Index_ShouldAddUserRole_WhenAuthenticatedUserIsNotInUserRole()
        {
            var vm = new HomeDashboardVm
            {
                IsAuthenticated = true,
                ProfileName = "Petq"
            };

            var dashboardService = new FakeHomeDashboardService(vm);

            var appUser = new ApplicationUser
            {
                Id = "user-1",
                UserName = "petq@test.bg",
                Email = "petq@test.bg"
            };

            var userManager = new FakeUserManager(appUser, false);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user-1")
                },
                "TestAuth"));

            var controller = CreateController(dashboardService, userManager, principal);

            var result = await controller.Index();

            result.Should().BeOfType<ViewResult>();
            userManager.AddToRoleCalled.Should().BeTrue();
        }

        [Fact]
        public void Privacy_ShouldReturnViewResult()
        {
            var dashboardService = new FakeHomeDashboardService(new HomeDashboardVm());
            var userManager = new FakeUserManager(null, false);
            var controller = CreateController(dashboardService, userManager);

            var result = controller.Privacy();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void StatusCodeError_ShouldReturnView_AndSetErrorCode()
        {
            var dashboardService = new FakeHomeDashboardService(new HomeDashboardVm());
            var userManager = new FakeUserManager(null, false);
            var controller = CreateController(dashboardService, userManager);

            var result = controller.StatusCodeError(404);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewData["ErrorCode"].Should().Be(404);
        }

        [Fact]
        public void Error_ShouldReturnView_WithErrorViewModel()
        {
            var dashboardService = new FakeHomeDashboardService(new HomeDashboardVm());
            var userManager = new FakeUserManager(null, false);
            var controller = CreateController(dashboardService, userManager);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = controller.Error();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<ErrorViewModel>();
        }
    }
}
