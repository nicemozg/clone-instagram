using Instagramm.Models;
using Microsoft.AspNetCore.Identity;

namespace Instagramm.Services;

public class AdminInitializer
{
    public static async Task SeedAdminUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        string adminEmail = "admin@admin.com";
        string adminUserName = "admin";
        string password = "Qwerty123@";

        var roles = new[] { "admin", "user" };
        foreach (var role in roles)
        {
            if (await roleManager.FindByNameAsync(role) is null)
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        if (await userManager.FindByNameAsync(adminEmail) is null)
        {
            User admin = new User { Email = adminEmail, UserName = adminUserName,  AvatarFileName = "3be4e88b-0ce6-40d0-b7cf-e7cbaac8d3f6_photo_2023-01-25_01-21-38.jpg"};
            IdentityResult result = await userManager.CreateAsync(admin, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "admin");
        }
    }
}