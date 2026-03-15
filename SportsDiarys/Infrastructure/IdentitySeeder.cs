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
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 1️⃣ Създаване на роли
            if (!await roleManager.RoleExistsAsync(Roles.User))
                await roleManager.CreateAsync(new IdentityRole(Roles.User));

            if (!await roleManager.RoleExistsAsync(Roles.Administrator))
                await roleManager.CreateAsync(new IdentityRole(Roles.Administrator));

            // 2️⃣ Админ потребител
            var adminEmail = "admin@sportdiary.bg";
            var adminPassword = "Admin123!";

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (!result.Succeeded)
                    return; // спира ако има грешка
            }

            // 3️⃣ Добавяне на админа в роля Administrator
            if (!await userManager.IsInRoleAsync(admin, Roles.Administrator))
                await userManager.AddToRoleAsync(admin, Roles.Administrator);

            // 4️⃣ Създаване на UserProfile за админа
            var profileExists = await dbContext.UserProfiles
                .AnyAsync(up => up.IdentityUserId == admin.Id);

            if (!profileExists)
            {
                var profile = new UserProfile
                {
                    IdentityUserId = admin.Id,
                    Name = "Administrator",
                    Age = 30,
                    Gender = "Male",
                    StartWeightKg = 70,
                    CurrentWeightKg = 70,
                    HeightCm = 180,
                    ActivityLevel = "Medium"
                };

                dbContext.UserProfiles.Add(profile);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}