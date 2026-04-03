using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsDiarys.Controllers;
using SportsDiarys.Models;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.Home;
using System.Security.Claims;
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

        private static HomeController CreateController(
            IHomeDashboardService dashboardService,
            ClaimsPrincipal? user = null)
        {
            var controller = new HomeController(dashboardService);

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
            var controller = CreateController(dashboardService);

            var result = await controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<HomeDashboardVm>().Subject;

            model.IsAuthenticated.Should().BeFalse();
        }

        [Fact]
        public async Task Index_ShouldReturnDashboard_WhenUserIsAuthenticated()
        {
            var vm = new HomeDashboardVm
            {
                IsAuthenticated = true,
                ProfileName = "Petq",
                DiariesCount = 2,
                EntriesCount = 3
            };

            var dashboardService = new FakeHomeDashboardService(vm);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user-1")
                },
                "TestAuth"));

            var controller = CreateController(dashboardService, principal);

            var result = await controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<HomeDashboardVm>().Subject;

            model.IsAuthenticated.Should().BeTrue();
            model.ProfileName.Should().Be("Petq");
            model.DiariesCount.Should().Be(2);
            model.EntriesCount.Should().Be(3);
        }

        [Fact]
        public void Privacy_ShouldReturnViewResult()
        {
            var dashboardService = new FakeHomeDashboardService(new HomeDashboardVm());
            var controller = CreateController(dashboardService);

            var result = controller.Privacy();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void StatusCodeError_ShouldReturnView_AndSetErrorCode()
        {
            var dashboardService = new FakeHomeDashboardService(new HomeDashboardVm());
            var controller = CreateController(dashboardService);

            var result = controller.StatusCodeError(404);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewData["ErrorCode"].Should().Be(404);
        }

        [Fact]
        public void Error_ShouldReturnView_WithErrorViewModel()
        {
            var dashboardService = new FakeHomeDashboardService(new HomeDashboardVm());
            var controller = CreateController(dashboardService);

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