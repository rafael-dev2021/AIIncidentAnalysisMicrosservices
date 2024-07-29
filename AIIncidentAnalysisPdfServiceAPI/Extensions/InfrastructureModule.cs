namespace AIIncidentAnalysisPdfServiceAPI.Extensions;

public static class InfrastructureModule
{
    public static void AddInfrastructureModule(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddDatabaseDependencyInjection(configuration);
        service.AddDependencyInjectionRepositories();
        service.AddDependencyInjectionServices();
    }
}