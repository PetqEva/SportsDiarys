using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportsDiarys.Data;
using SportsDiarys.Data.Models;

namespace SportsDiarys.Infrastructure
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = services.GetRequiredService<AppDbContext>();

            if (!await roleManager.RoleExistsAsync(Roles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.User));
            }

            if (!await roleManager.RoleExistsAsync(Roles.Administrator))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Administrator));
            }

            const string adminEmail = "admin@sportdiary.bg";
            const string adminPassword = "Admin123!";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", createResult.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, Roles.Administrator))
            {
                var addAdminRoleResult = await userManager.AddToRoleAsync(admin, Roles.Administrator);
                if (!addAdminRoleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", addAdminRoleResult.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, Roles.User))
            {
                var addUserRoleResult = await userManager.AddToRoleAsync(admin, Roles.User);
                if (!addUserRoleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", addUserRoleResult.Errors.Select(e => e.Description)));
                }
            }

            var hasProfile = await dbContext.UserProfiles
                .AnyAsync(x => x.IdentityUserId == admin.Id);

            if (!hasProfile)
            {
                dbContext.UserProfiles.Add(new UserProfile
                {
                    IdentityUserId = admin.Id,
                    Name = "Administrator",
                    Age = 30,
                    Gender = "Male",
                    StartWeightKg = 80,
                    CurrentWeightKg = 78,
                    HeightCm = 180,
                    ActivityLevel = "Medium"
                });

                await dbContext.SaveChangesAsync();
            }
        }
    }
}