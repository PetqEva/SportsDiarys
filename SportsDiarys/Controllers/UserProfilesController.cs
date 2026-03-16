using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportsDiarys.ViewModels.UserProfiles;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.Data.Models;

namespace SportsDiarys.Controllers
{
    [Authorize]
    public class UserProfilesController : BaseController
    {
        private readonly IUserProfileService _profileService;

        private static readonly string[] AllowedActivityLevels = { "Low", "Medium", "High" };

        public UserProfilesController(
    IUserProfileService profileService,
    UserManager<ApplicationUser> userManager)
    : base(userManager, profileService)
        {
            _profileService = profileService;
        }


        private static List<SelectListItem> BuildActivityOptions() => new()
        {
            new SelectListItem { Value = "Low", Text = "Ниска активност" },
            new SelectListItem { Value = "Medium", Text = "Средна активност" },
            new SelectListItem { Value = "High", Text = "Висока активност" }
        };

        private static void ValidateActivityLevel(EditUserProfileVm vm, ModelStateDictionary modelState)
        {
            if (string.IsNullOrWhiteSpace(vm.ActivityLevel) || !AllowedActivityLevels.Contains(vm.ActivityLevel))
            {
                modelState.AddModelError(nameof(vm.ActivityLevel), "Невалидно ниво на активност.");
            }
        }

        [HttpGet]
        public IActionResult Me() => RedirectToAction(nameof(Details));

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var profile = await _profileService.GetMyProfileAsync(userId);

            if (profile == null)
                return RedirectToAction(nameof(Create));

            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var userId = GetUserId();
            var profile = await _profileService.GetMyProfileWithDiariesAsync(userId);

            if (profile == null)
                return RedirectToAction(nameof(Create));

            return View(profile);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new EditUserProfileVm
            {
                ActivityOptions = BuildActivityOptions()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditUserProfileVm vm)
        {
            ValidateActivityLevel(vm, ModelState);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Моля, коригирайте грешките във формата.");

                vm.ActivityOptions = BuildActivityOptions();
                return View(vm);
            }


            var userId = GetUserId();

            var created = await _profileService.CreateMyProfileAsync(
                userId,
                vm.Name,
                vm.Age,
                vm.Gender,
                vm.StartWeightKg,
                vm.CurrentWeightKg,
                vm.HeightCm,
                vm.ActivityLevel);

            if (!created)
            {
                ModelState.AddModelError(string.Empty, "Профилът вече съществува. Използвай Редакция.");
                vm.ActivityOptions = BuildActivityOptions();
                return View(vm);
            }

            return RedirectToAction(nameof(Details));
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = GetUserId();
            var profile = await _profileService.GetMyProfileAsync(userId);

            if (profile == null)
                return RedirectToAction(nameof(Create));

            var vm = new EditUserProfileVm
            {
                Name = profile.Name,
                Age = profile.Age,
                Gender = profile.Gender,
                StartWeightKg = profile.StartWeightKg,
                CurrentWeightKg = profile.CurrentWeightKg,
                HeightCm = profile.HeightCm,
                ActivityLevel = profile.ActivityLevel,
                ActivityOptions = BuildActivityOptions()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserProfileVm vm)
        {
            ValidateActivityLevel(vm, ModelState);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Моля, коригирайте грешките във формата.");

                vm.ActivityOptions = BuildActivityOptions();
                return View(vm);
            }

            var userId = GetUserId();

            var updated = await _profileService.UpdateMyProfileAsync(
                userId,
                vm.Name,
                vm.Age,
                vm.Gender,
                vm.StartWeightKg,
                vm.CurrentWeightKg,
                vm.HeightCm,
                vm.ActivityLevel);

            if (!updated)
                return RedirectToAction(nameof(Create));

            return RedirectToAction(nameof(Details));
        }

        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            var userId = GetUserId();
            var profile = await _profileService.GetMyProfileAsync(userId);

            if (profile == null)
                return RedirectToAction(nameof(Create));

            return View(profile);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var userId = GetUserId();

            var deleted = await _profileService.DeleteMyProfileAsync(userId);
            if (!deleted) return NotFound();

            return RedirectToAction("Index", "Home");
        }
    }
}
