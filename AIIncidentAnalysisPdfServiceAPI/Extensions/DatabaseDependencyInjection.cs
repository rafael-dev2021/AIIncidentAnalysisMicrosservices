using AIIncidentAnalysisPdfServiceAPI.Utils;

namespace AIIncidentAnalysisPdfServiceAPI.Extensions;

public static class DatabaseDependencyInjection
{
    public static void AddDatabaseDependencyInjection(this IServiceCollection service, IConfiguration configuration)
    {
        service.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
    }
}