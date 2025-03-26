using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.Authorization;

public static class RoleTask
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = ["Admin", "Employer", "Employee", "Searcher"];

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}