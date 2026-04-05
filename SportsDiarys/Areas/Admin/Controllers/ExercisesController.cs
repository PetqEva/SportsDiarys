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
        private readonly IWebHostEnvironment _environment;

        public ExercisesController(
            IExerciseService exerciseService,
            IWebHostEnvironment environment)
        {
            _exerciseService = exerciseService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ExerciseQueryVm query)
        {
            query.Page = query.Page < 1 ? 1 : query.Page;
            query.PageSize = query.PageSize < 1 ? 10 : Math.Min(query.PageSize, 50);

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
            ValidateImage(model.ImageFile);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ImageFile != null)
            {
                model.ImagePath = await SaveImageAsync(model.ImageFile);
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
            ValidateImage(model.ImageFile);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ImageFile != null)
            {
                model.ImagePath = await SaveImageAsync(model.ImageFile);
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

        private void ValidateImage(IFormFile? imageFile)
        {
            if (imageFile == null)
            {
                return;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(ExerciseFormVm.ImageFile),
                    "Моля, качете валидно изображение (.jpg, .jpeg, .png, .webp).");
            }

            if (imageFile.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(ExerciseFormVm.ImageFile),
                    "Снимката трябва да бъде до 2 MB.");
            }
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "exercises");
            Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return $"/images/exercises/{fileName}";
        }
    }
}