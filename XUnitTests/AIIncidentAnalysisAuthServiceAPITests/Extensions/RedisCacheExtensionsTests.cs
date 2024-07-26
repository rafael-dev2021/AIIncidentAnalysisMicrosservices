﻿using AIIncidentAnalysisAuthServiceAPI.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Extensions;

public class RedisCacheExtensionsTests
{
    [Fact]
    public void AddRedisCacheDependencyInjection_ShouldRegisterRedisCache()
    {
        // Arrange
        var services = new ServiceCollection();

        Environment.SetEnvironmentVariable("REDIS_CONNECTION", "localhost:6379");

        // Act
        services.AddRedisCacheExtensions();
        var serviceProvider = services.BuildServiceProvider();
        var cache = serviceProvider.GetService<IDistributedCache>();

        // Assert
        Assert.NotNull(cache);
        Assert.IsAssignableFrom<IDistributedCache>(cache);

        var cacheType = cache.GetType();
        Assert.True(cacheType.Namespace == "Microsoft.Extensions.Caching.StackExchangeRedis" && cacheType.Name.Contains("RedisCache"));

        // Cleanup
        Environment.SetEnvironmentVariable("REDIS_CONNECTION", null);
    }
}