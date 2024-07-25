using AIIncidentAnalysisAuthServiceAPI.Algorithms;
using AIIncidentAnalysisAuthServiceAPI.Algorithms.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Repositories;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies;

namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class DependencyInjectionRepositories
{
    public static void AddDependencyInjectionRepositories(this IServiceCollection service)
    {
        service.AddScoped<IAuthRepository, AuthRepository>();
        service.AddScoped<IUserRoleRepository, UserRoleRepository>();
        service.AddScoped<ITokenRepository, TokenRepository>();
        service.AddScoped<IAccountNumberGenerator, AccountNumberGenerator>();
        service.AddScoped<ILoggerStrategies, LoggerStrategies>();
    }
}