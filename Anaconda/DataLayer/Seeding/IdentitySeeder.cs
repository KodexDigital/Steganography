using Anaconda.Models;
using Anaconda.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Anaconda.DataLayer.Seeding
{
    public static class IdentitySeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var config = serviceProvider.GetRequiredService<IOptions<SystemSettings>>().Value;

            // 1. Ensure role exists
            if (!await roleManager.RoleExistsAsync(config.DefaultAdminRole!))
                await roleManager.CreateAsync(new ApplicationRole { Name = config.DefaultAdminRole! });

            // 2. Check if user exists
            var adminUser = await userManager.FindByEmailAsync(config.DefaultAdmin!);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = config.DefaultAdmin!,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, config.DefaultAdminPassword!);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, config.DefaultAdminRole!);
                else
                    throw new Exception("Admin user seeding failed: " + string.Join("; ", result.Errors));
            }
        }
    }
}