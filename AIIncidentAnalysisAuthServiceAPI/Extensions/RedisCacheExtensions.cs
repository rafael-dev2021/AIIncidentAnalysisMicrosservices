namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class RedisCacheExtensions
{
    public static void AddRedisCacheExtensions(this IServiceCollection service)
    {
        var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION");

        service.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "SampleInstance";
        });
    }
}