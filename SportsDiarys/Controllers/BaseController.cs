using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportDiary.Data.Models;
using SportDiary.Services.Interfaces;
using SportsDiarys.Services.Interfaces;

namespace SportDiary.Controllers;

[Authorize]
public abstract class BaseController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserProfileService _profileService;

    protected BaseController(
        UserManager<ApplicationUser> userManager,
        IUserProfileService profileService)
    {
        _userManager = userManager;
        _profileService = profileService;
    }

    protected string GetUserId() => _userManager.GetUserId(User)!;

    protected async Task<int?> GetMyProfileIdAsync()
    {
        var profile = await _profileService.GetMyProfileAsync(GetUserId());
        return profile?.Id;
    }
}
