using AIIncidentAnalysisAuthServiceAPI.Services;
using AIIncidentAnalysisAuthServiceAPI.Services.Interfaces;

namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class DependencyInjectionServices
{
    public static void AddDependencyInjectionService(this IServiceCollection service)
    {
        service.AddScoped<IAuthenticateService, AuthenticateService>();
        service.AddScoped<ITokenService, TokenService>();
        service.AddScoped<ITokenManagerService, TokenManagerService>();
    }
}