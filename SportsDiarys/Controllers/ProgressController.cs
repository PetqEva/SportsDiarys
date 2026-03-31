using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsDiarys.Data.Models;
using SportsDiarys.Services.Interfaces;

namespace SportsDiarys.Controllers
{
    [Authorize]
    public class ProgressController : BaseController
    {
        private readonly IProgressService _progressService;

        public ProgressController(
            IProgressService progressService,
            UserManager<ApplicationUser> userManager,
            IUserProfileService userProfileService)
            : base(userManager, userProfileService)
        {
            _progressService = progressService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var profileId = await GetMyProfileIdAsync();
            if (profileId == null)
            {
                return RedirectToAction("Create", "UserProfiles");
            }

            var vm = await _progressService.GetMyProgressAsync(profileId.Value);
            return View(vm);
        }
    }
}