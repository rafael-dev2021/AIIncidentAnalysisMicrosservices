namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class InfrastructureModuleStrategies
{
    public static void AddInfrastructureModuleStrategies(this IServiceCollection service)
    {
        service.AddDependencyInjectionAuthStrategies();
        service.AddDependencyInjectionUpdateStrategies();
        service.AddDependencyInjectionRegisterStrategies();
    }
}