using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.TrainingDiaries;

namespace SportsDiarys.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class TrainingDiaryController : Controller
    {
        private readonly ITrainingDiaryService _diaryService;

        public TrainingDiaryController(ITrainingDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            var profileId = 1;
            var diaries = await _diaryService.GetMyDiariesAsync(profileId, search, page, 10);
            return View(diaries);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new CreateTrainingDiaryViewModel
            {
                Date = DateTime.Today
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTrainingDiaryViewModel vm)
        {
            var profileId = 1;

            if (!ModelState.IsValid)
                return View(vm);

            var exists = await _diaryService.ExistsForDateAsync(profileId, vm.Date);
            if (exists)
            {
                ModelState.AddModelError(nameof(vm.Date), "Вече има дневник за тази дата.");
                return View(vm);
            }

            await _diaryService.CreateAsync(vm, profileId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var profileId = 1;
            var vm = await _diaryService.GetForEditAsync(id, profileId);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditTrainingDiaryViewModel vm)
        {
            var profileId = 1;

            if (!ModelState.IsValid)
                return View(vm);

            var exists = await _diaryService.ExistsForDateAsync(profileId, vm.Date, vm.Id);
            if (exists)
            {
                ModelState.AddModelError(nameof(vm.Date), "Вече има дневник за тази дата.");
                return View(vm);
            }

            var updateVm = new UpdateTrainingDiaryViewModel
            {
                Id = vm.Id,
                Date = vm.Date,
                Notes = vm.Notes,
                Calories = vm.Calories,
                DurationMinutes = vm.DurationMinutes,
                DistanceKm = vm.DistanceKm,
                WaterLiters = vm.WaterLiters,
                Place = vm.Place
            };

            var updated = await _diaryService.UpdateAsync(updateVm, profileId);

            if (!updated)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var profileId = 1;
            var diary = await _diaryService.GetByIdAsync(id, profileId);

            if (diary == null)
                return NotFound();

            return View(diary);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profileId = 1;
            await _diaryService.DeleteAsync(id, profileId);
            return RedirectToAction(nameof(Index));
        }
    }
}