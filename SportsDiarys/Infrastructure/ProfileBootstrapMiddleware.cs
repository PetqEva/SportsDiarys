using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;

namespace SportDiary.Infrastructure
{
    public class ProfileBootstrapMiddleware
    {
        private readonly RequestDelegate _next;

        public ProfileBootstrapMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = userManager.GetUserId(context.User);

                // Пази се от null, макар че при authenticated обикновено има userId
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var hasProfile = await db.UserProfiles.AsNoTracking()
                        .AnyAsync(p => p.IdentityUserId == userId);

                    if (!hasProfile)
                    {
                        // Минимален профил (после потребителят го редактира)
                        var email = context.User.Identity?.Name ?? "user";
                        var profile = new UserProfile
                        {
                            IdentityUserId = userId,
                            Name = email,
                            Age = 18
                        };

                        db.UserProfiles.Add(profile);
                        await db.SaveChangesAsync();
                    }
                }
            }

            await _next(context);
        }
    }
}
