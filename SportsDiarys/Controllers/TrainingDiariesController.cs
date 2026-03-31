using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.TrainingDiaries;

namespace SportsDiarys.Controllers
{
    [Authorize]
    public class TrainingDiariesController : BaseController
    {
        private readonly ITrainingDiaryService _service;

        public TrainingDiariesController(
            ITrainingDiaryService service,
            UserManager<ApplicationUser> userManager,
            IUserProfileService userProfileService)
            : base(userManager, userProfileService)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            var profileId = await GetMyProfileIdAsync();
            if (profileId == null)
            {
                return RedirectToAction("Create", "UserProfiles");
            }

            var result = await _service.GetMyDiariesAsync(profileId.Value, search, page, 5);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var profileId = await GetMyProfileIdAsync();
            if (profileId == null)
            {
                return RedirectToAction("Create", "UserProfiles");
            }

            var diary = await _service.GetByIdAsync(id, profileId.Value);

            if (diary == null)
            {
                return NotFound();
            }

            return View(diary);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateTrainingDiaryViewModel
            {
                Date = DateTime.Today,
                PlaceOptions = GetPlaceOptions()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTrainingDiaryViewModel model)
        {
            var profileId = await GetMyProfileIdAsync();
            if (profileId == null)
            {
                return RedirectToAction("Create", "UserProfiles");
            }

            if (!ModelState.IsValid)
            {
                model.PlaceOptions = GetPlaceOptions();
                return View(model);
            }

            bool exists = await _service.ExistsForDateAsync(profileId.Value, model.Date);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Вече има дневник за тази дата.");
                model.PlaceOptions = GetPlaceOptions();
                return View(model);
            }

            await _service.CreateAsync(model, profileId.Value);

            TempData["Success"] = "Дневникът беше създаден успешно.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var profileId = await GetMyProfileIdAsync();
            if (profileId == null)
            {
                return RedirectToAction("Create", "UserProfiles");
            }

            var diary = await _service.GetForEditAsync(id, profileId.Value);

            if (diary == null)
            {
                return NotFound();
            }

            var model = new UpdateTrainingDiaryViewModel
            {
                Id = diary.Id,
                Date = diary.Date,
                Notes = diary.Notes,
                Calories = diary.Calories,
                DurationMinutes = diary.DurationMinutes,
                DistanceKm = diary.DistanceKm,
                WaterLiters = diary.WaterLiters,
                Place = diary.Place,
                PlaceOptions = GetPlaceOptions()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateTrainingDiaryViewModel model)
        {
            var profileId = await GetMyProfileIdAsync();
            if (profileId == null)
            {
                return RedirectToAction("Create", "UserProfiles");
            }

            if (!ModelState.IsValid)
            {
                model.PlaceOptions = GetPlaceOptions();
                return View(model);
            }

            bool exists = await _service.ExistsForDateAsync(profileId.Value, model.Date, model.Id);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Вече има дневник за тази дата.");
                model.PlaceOptions = GetPlaceOptions();
                return View(model);
            }

            bool success = await _service.UpdateAsync(model, profileId.Value);

            if (!success)
            {
                return NotFound();
            }

            TempData["Success"] = "Дневникът беше редактиран успешно.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var profileId = await GetMyProfileIdAsync();
            if (profileId == null)
            {
                return RedirectToAction("Create", "UserProfiles");
            }

            var diary = await _service.GetByIdAsync(id, profileId.Value);

            if (diary == null)
            {
                return NotFound();
            }

            return View(diary);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profileId = await GetMyProfileIdAsync();
            if (profileId == null)
            {
                return RedirectToAction("Create", "UserProfiles");
            }

            bool success = await _service.DeleteAsync(id, profileId.Value);

            if (!success)
            {
                return NotFound();
            }

            TempData["Success"] = "Дневникът беше изтрит успешно.";

            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<SelectListItem> GetPlaceOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Home", Text = "В къщи" },
                new SelectListItem { Value = "Gym", Text = "Фитнес" },
                new SelectListItem { Value = "Outdoor", Text = "Навън" },
                new SelectListItem { Value = "Other", Text = "Друго" }
            };
        }
    }
}