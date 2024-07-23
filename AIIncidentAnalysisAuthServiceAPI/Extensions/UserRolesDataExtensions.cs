using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;

namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class UserRolesDataExtensions
{
    public static async Task AddUserRolesDataExtensions(IApplicationBuilder builder)
    {
        var scope = builder.ApplicationServices.CreateScope();
        var result = scope.ServiceProvider.GetService<IUserRoleRepository>();

        if (result != null)
        {
            await result.RoleAsync();
            await result.UserAsync();
        }
    }
}