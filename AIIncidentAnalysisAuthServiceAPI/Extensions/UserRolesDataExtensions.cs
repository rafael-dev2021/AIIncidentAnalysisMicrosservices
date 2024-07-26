using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class UserRolesDataExtensions
{
    public static async Task AddUserRolesDataExtensions(IApplicationBuilder builder)
    {
        var scope = builder.ApplicationServices.CreateScope();
        var result = scope.ServiceProvider.GetService<IUserRoleRepository>();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await InitializeRolesAsync(roleManager);
        
        if (result != null)
        {
            await result.RoleAsync();
            await result.UserAsync();
        }
    }
    
    private static async Task InitializeRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "ReadOnly", "ReadWrite", "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
