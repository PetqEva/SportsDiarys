using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsDiarys.Infrastructure;
using SportsDiarys.Services.Interfaces;
using SportsDiarys.ViewModels.Exercises;

namespace SportsDiarys.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Administrator)]
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
            query.Page = query.Page < 1 ? 1 : query.Page;
            query.PageSize = query.PageSize < 1 ? 10 : query.PageSize;

            var model = await _exerciseService.GetPagedAsync(query);
            ViewBag.Query = query;

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ExerciseFormVm();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExerciseFormVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var id = await _exerciseService.CreateAsync(model);

            TempData["AdminSuccess"] = "Упражнението беше създадено успешно.";
            return RedirectToAction(nameof(Edit), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var model = await _exerciseService.GetForEditAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExerciseFormVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _exerciseService.UpdateAsync(model);
            if (!success)
            {
                return NotFound();
            }

            TempData["AdminSuccess"] = "Упражнението беше редактирано успешно.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id, bool isActive)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var success = await _exerciseService.SetActiveAsync(id, isActive);
            if (!success)
            {
                return NotFound();
            }

            TempData["AdminSuccess"] = isActive
                ? "Упражнението беше активирано."
                : "Упражнението беше деактивирано.";

            return RedirectToAction(nameof(Index));
        }
    }
}