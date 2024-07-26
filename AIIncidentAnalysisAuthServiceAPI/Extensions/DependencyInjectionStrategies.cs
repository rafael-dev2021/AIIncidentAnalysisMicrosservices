using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser.Interfaces;

namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class DependencyInjectionStrategies
{
    public static void AddDependencyInjectionAuthStrategies(this IServiceCollection service)
    {
        service.AddScoped<IAuthStrategy, AuthStrategy>();
        service.AddScoped<ILoginAttemptsManagerStrategy, LoginAttemptsManagerStrategy>();
    }

    public static void AddDependencyInjectionRegisterStrategies(this IServiceCollection service)
    {
        service.AddScoped<IRegisterStrategy, RegisterStrategy>();
        service.AddScoped<ILocalCacheManagerStrategy, LocalCacheManagerStrategy>();
        service.AddScoped<IUserValidationManagerStrategy, UserValidationManagerStrategy>();
    }

    public static void AddDependencyInjectionUpdateStrategies(this IServiceCollection service)
    {
        service.AddScoped<IUpdateProfileStrategy, UpdateProfileStrategy>();
        service.AddScoped<ITransactionHandlerStrategy, TransactionHandlerStrategy>();
        service.AddScoped<IUserDetailsValidatorStrategy, UserDetailsValidatorStrategy>();
        service.AddScoped<IUserFieldUpdaterStrategy, UserFieldUpdaterStrategy>();
    }
}