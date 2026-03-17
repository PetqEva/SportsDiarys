using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.Exercises;

namespace SportsDiarys.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class ExercisesController : Controller
    {
        private readonly IExerciseService _exerciseService;

        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ExerciseQueryVm query)
        {
            var vm = await _exerciseService.GetPagedAsync(query);

            ViewBag.Query = query; // за да си запазим филтрите във view-то
            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ExerciseFormVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExerciseFormVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var id = await _exerciseService.CreateAsync(model);
            return RedirectToAction(nameof(Edit), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _exerciseService.GetForEditAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExerciseFormVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ok = await _exerciseService.UpdateAsync(model);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id, bool isActive)
        {
            var ok = await _exerciseService.SetActiveAsync(id, isActive);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}


