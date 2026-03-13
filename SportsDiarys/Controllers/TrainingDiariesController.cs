using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportsDiarys.Models;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.TrainingDiaries;


namespace SportDiary.Controllers
{
    [Authorize]
    public class TrainingDiariesController : BaseController
    {
        private readonly ITrainingDiaryService _diaryService;

        public TrainingDiariesController(
            ITrainingDiaryService diaryService,
            IUserProfileService profileService,
            UserManager<ApplicationUser> userManager)
            : base(userManager, profileService)
        {
            _diaryService = diaryService;
        }

        private static List<SelectListItem> BuildPlaceOptions() => new()
        {
            new SelectListItem { Value = "Home", Text = "В къщи" },
            new SelectListItem { Value = "Gym", Text = "Фитнес" },
            new SelectListItem { Value = "Outdoor", Text = "Навън" },
            new SelectListItem { Value = "Other", Text = "Друго" }
        };

        // GET: TrainingDiaries
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var diaries = await _diaryService.GetMyDiariesAsync(userProfileId.Value);
            return View(diaries);
        }

        // GET: TrainingDiaries/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var diary = await _diaryService.GetMyDiaryDetailsVmAsync(id.Value, userProfileId.Value);
            if (diary == null) return NotFound();

            return View(diary);
        }

        // GET: TrainingDiaries/Create
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new TrainingDiaryFormVm
            {
                Date = DateTime.Today,
                PlaceOptions = BuildPlaceOptions()
            };

            return View(vm);
        }

        // POST: TrainingDiaries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingDiaryFormVm vm)
        {
            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;

            var allowed = new[] { "Home", "Gym", "Outdoor", "Other" };
            if (!allowed.Contains(vm.Place))
            {
                ModelState.AddModelError(nameof(vm.Place), "Невалидно място.");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Моля, коригирайте грешките във формата.");

                vm.PlaceOptions = BuildPlaceOptions();
                return View(vm);
            }


            var exists = await _diaryService.DiaryExistsForDateAsync(pid, vm.Date);
            if (exists)
            {
                ModelState.AddModelError(nameof(vm.Date), "Вече има дневник за тази дата.");
                vm.PlaceOptions = BuildPlaceOptions();
                return View(vm);
            }

            var entity = new TrainingDiary
            {
                UserProfileId = pid,
                Date = vm.Date,
                DurationMinutes = vm.DurationMinutes,
                Place = vm.Place,
                WaterLiters = vm.WaterLiters,
                Notes = vm.Notes
            };

            await _diaryService.CreateAsync(entity, pid);
            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingDiaries/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;

            var diary = await _diaryService.GetMyDiaryForEditAsync(id.Value, pid);
            if (diary == null) return NotFound();

            var vm = new TrainingDiaryFormVm
            {
                Id = diary.Id,
                Date = diary.Date,
                DurationMinutes = diary.DurationMinutes,
                Place = diary.Place,
                WaterLiters = diary.WaterLiters,
                Notes = diary.Notes,
                PlaceOptions = BuildPlaceOptions()
            };

            return View(vm);
        }

        // POST: TrainingDiaries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingDiaryFormVm vm)
        {
            if (id != vm.Id) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;

            var allowed = BuildPlaceOptions().Select(x => x.Value);

            if (!allowed.Contains(vm.Place))
            {
                ModelState.AddModelError(nameof(vm.Place), "Невалидно място.");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Моля, коригирайте грешките във формата.");

                vm.PlaceOptions = BuildPlaceOptions();
                return View(vm);
            }


            var exists = await _diaryService.DiaryExistsForDateAsync(pid, vm.Date, excludeDiaryId: vm.Id);
            if (exists)
            {
                ModelState.AddModelError(nameof(vm.Date), "Вече има дневник за тази дата.");
                vm.PlaceOptions = BuildPlaceOptions();
                return View(vm);
            }

            var updated = await _diaryService.UpdateAsync(
                vm.Id,
                pid,
                vm.Date,
                vm.DurationMinutes,
                vm.Place,
                vm.WaterLiters,
                vm.Notes
            );

            if (!updated) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingDiaries/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var diary = await _diaryService.GetMyDiaryDetailsVmAsync(id.Value, userProfileId.Value);
            if (diary == null) return NotFound();

            return View(diary);
        }

        // POST: TrainingDiaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var deleted = await _diaryService.DeleteAsync(id, userProfileId.Value);
            if (!deleted) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}

