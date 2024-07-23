namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class InfrastructureModule
{
    public static void AddInfrastructureModule(this IServiceCollection service)
    {
        service.AddDatabaseDependencyInjection();
        service.AddJwtExtensions();
        service.AddIdentityRulesExtensions();
        service.AddOpenApiExtensions();
        service.AddRedisCacheExtensions();
        service.AddFluentValidationExtension();
        service.AddDependencyInjectionRepositories();
        service.AddDependencyInjectionService();
        service.AddMappingProfileExtension();
        service.AddPoliciesExtensions();
    }
}