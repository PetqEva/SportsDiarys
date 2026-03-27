using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.TrainingEntries;

namespace SportsDiarys.Controllers
{
    [Authorize]
    public class TrainingEntriesController : BaseController
    {
        private readonly ITrainingEntryService _entryService;
        
        public TrainingEntriesController(
            ITrainingEntryService entryService,
            IUserProfileService profileService,
            UserManager<ApplicationUser> userManager)
            : base(userManager, profileService)
        {
            _entryService = entryService;
        }

        private async Task<int?> GetProfileIdOrRedirectAsync() => await GetMyProfileIdAsync();

        private async Task<List<SelectListItem>> BuildMyDiariesSelectAsync(int userProfileId, int? selectedId = null)
        {
            var diaries = await _entryService.GetMyDiarySelectItemsAsync(userProfileId);
            return diaries.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Text,
                Selected = selectedId.HasValue && d.Id == selectedId.Value
            }).ToList();
        }

        // ===================== INDEX =====================
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] EntriesQueryVm? query)
        {
            var userProfileId = await GetProfileIdOrRedirectAsync();
            if (userProfileId == null)
                return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;
            var safeQuery = query ?? new EntriesQueryVm();

            var vm = new EntriesIndexVm
            {
                Query = safeQuery,
                Diaries = await BuildMyDiariesSelectAsync(pid, safeQuery.DiaryId),
                Result = await _entryService.GetMyEntriesPagedAsync(pid, safeQuery)
            };

            return View(vm);
        }

        // ===================== DETAILS =====================
        [HttpGet]
        public async Task<IActionResult> Details(int? id, string? returnUrl)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetProfileIdOrRedirectAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;
            var entry = await _entryService.GetMyEntryDetailsAsync(id.Value, pid);
            if (entry == null) return NotFound();

            var exercises = await _entryService.GetEntryExercisesAsync(id.Value, pid);
            var available = await _entryService.GetActiveExercisesAsync();

            var vm = new TrainingEntryDetailsPageVm
            {
                Entry = entry,
                Exercises = exercises,
                AddExercise = new AddExerciseToEntryVm
                {
                    TrainingEntryId = id.Value,
                    AvailableExercises = available.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    }).ToList()
                },
                ReturnUrl = returnUrl
            };

            return View(vm);
        }

        // ===================== CREATE =====================
        [HttpGet]
        public async Task<IActionResult> Create(int? diaryId, string? returnUrl)
        {
            var userProfileId = await GetProfileIdOrRedirectAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;
            var vm = new TrainingEntryFormVm
            {
                TrainingDiaryId = diaryId ?? 0,
                Diaries = await BuildMyDiariesSelectAsync(pid, diaryId),
                ReturnUrl = returnUrl
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingEntryFormVm vm)
        {
            var userProfileId = await GetProfileIdOrRedirectAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;

            foreach (var error in _entryService.ValidateBusinessRules(vm))
                ModelState.AddModelError(error.Field, error.Message);

            var diaryIsMine = await _entryService.DiaryBelongsToMeAsync(vm.TrainingDiaryId, pid);
            if (!diaryIsMine)
                ModelState.AddModelError(nameof(vm.TrainingDiaryId), "Невалиден дневник.");

            if (!ModelState.IsValid)
            {
                vm.Diaries = await BuildMyDiariesSelectAsync(pid, vm.TrainingDiaryId);
                return View(vm);
            }

            var entity = new TrainingEntry
            {
                SportName = vm.SportName.Trim(),
                DurationMinutes = vm.DurationMinutes,
                Calories = vm.Calories,
                DistanceKm = vm.DistanceKm,
                TrainingDiaryId = vm.TrainingDiaryId
            };

            await _entryService.CreateAsync(entity, pid);

            if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                return Redirect(vm.ReturnUrl);

            return RedirectToAction(nameof(Index));
        }

        // ===================== EDIT =====================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetProfileIdOrRedirectAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;
            var entry = await _entryService.GetMyEntryForEditAsync(id.Value, pid);
            if (entry == null) return NotFound();

            var vm = new TrainingEntryFormVm
            {
                Id = entry.Id,
                SportName = entry.SportName,
                DurationMinutes = entry.DurationMinutes,
                Calories = entry.Calories,
                DistanceKm = entry.DistanceKm,
                TrainingDiaryId = entry.TrainingDiaryId,
                Diaries = await BuildMyDiariesSelectAsync(pid, entry.TrainingDiaryId),
                ReturnUrl = returnUrl
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingEntryFormVm vm)
        {
            if (id != vm.Id) return NotFound();

            var userProfileId = await GetProfileIdOrRedirectAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;

            foreach (var error in _entryService.ValidateBusinessRules(vm))
                ModelState.AddModelError(error.Field, error.Message);

            var diaryIsMine = await _entryService.DiaryBelongsToMeAsync(vm.TrainingDiaryId, pid);
            if (!diaryIsMine)
                ModelState.AddModelError(nameof(vm.TrainingDiaryId), "Невалиден дневник.");

            if (!ModelState.IsValid)
            {
                vm.Diaries = await BuildMyDiariesSelectAsync(pid, vm.TrainingDiaryId);
                return View(vm);
            }

            var entity = new TrainingEntry
            {
                Id = vm.Id,
                SportName = vm.SportName.Trim(),
                DurationMinutes = vm.DurationMinutes,
                Calories = vm.Calories,
                DistanceKm = vm.DistanceKm,
                TrainingDiaryId = vm.TrainingDiaryId
            };

            var updated = await _entryService.UpdateAsync(entity, pid);
            if (!updated) return NotFound();

            if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                return Redirect(vm.ReturnUrl);

            return RedirectToAction(nameof(Index));
        }

        // ===================== DELETE =====================
        [HttpGet]
        public async Task<IActionResult> Delete(int? id, string? returnUrl)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetProfileIdOrRedirectAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;
            var entry = await _entryService.GetMyEntryDetailsAsync(id.Value, pid);
            if (entry == null) return NotFound();

            ViewBag.ReturnUrl = returnUrl;
            return View(entry);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            var userProfileId = await GetProfileIdOrRedirectAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;
            var deleted = await _entryService.DeleteAsync(id, pid);
            if (!deleted) return NotFound();

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }

        // ===================== EXERCISES =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExercise([Bind(Prefix = "AddExercise")] AddExerciseToEntryVm model, string? returnUrl)
        {
            var userProfileId = await GetProfileIdOrRedirectAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;

            if (model.ExerciseId is null)
                ModelState.AddModelError("AddExercise.ExerciseId", "Избери упражнение.");

            if (!ModelState.IsValid)
            {
                var entry = await _entryService.GetMyEntryDetailsAsync(model.TrainingEntryId, pid);
                if (entry == null) return NotFound();

                var exercises = await _entryService.GetEntryExercisesAsync(model.TrainingEntryId, pid);
                var available = await _entryService.GetActiveExercisesAsync();

                model.AvailableExercises = available.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();

                var pageVm = new TrainingEntryDetailsPageVm
                {
                    Entry = entry,
                    Exercises = exercises,
                    AddExercise = model,
                    ReturnUrl = returnUrl
                };

                return View("Details", pageVm);
            }

            var added = await _entryService.AddExerciseAsync(model, pid);
            if (!added)
            {
                ModelState.AddModelError("AddExercise.ExerciseId", "Упражнението не можа да бъде добавено. Вече е добавено, не съществува или записът не е твой.");

                var entry = await _entryService.GetMyEntryDetailsAsync(model.TrainingEntryId, pid);
                if (entry == null) return NotFound();

                var exercises = await _entryService.GetEntryExercisesAsync(model.TrainingEntryId, pid);
                var available = await _entryService.GetActiveExercisesAsync();

                model.AvailableExercises = available.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();

                var pageVm = new TrainingEntryDetailsPageVm
                {
                    Entry = entry,
                    Exercises = exercises,
                    AddExercise = model,
                    ReturnUrl = returnUrl
                };

                return View("Details", pageVm);
            }

            return RedirectToAction(nameof(Details), new { id = model.TrainingEntryId, returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveExercise(int trainingEntryId, int exerciseId, string? returnUrl)
        {
            var userProfileId = await GetProfileIdOrRedirectAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;
            var removed = await _entryService.RemoveExerciseAsync(trainingEntryId, exerciseId, pid);

            if (!removed)
                return NotFound();

            return RedirectToAction(nameof(Details), new { id = trainingEntryId, returnUrl });
        }
    }
}