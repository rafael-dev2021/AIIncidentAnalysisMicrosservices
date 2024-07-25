using System.Collections.Concurrent;
using System.Reflection;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories.Strategies.RegisterUser;

public class LocalCacheManagerStrategyTests
{
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly LocalCacheManagerStrategy _localCacheManagerStrategy;

    public LocalCacheManagerStrategyTests()
    {
        _cacheMock = new Mock<IDistributedCache>();
        _localCacheManagerStrategy = new LocalCacheManagerStrategy(_cacheMock.Object);
    }


    [Fact]
    public async Task IsKeyAlreadyUsedAsync_KeyExistsInLocalCache_ReturnsTrue()
    {
        // Arrange
        const string key = "testKey";
        LocalCacheManagerStrategyTestsAccessor.AddToLocalCache(key);

        // Act
        var result = await _localCacheManagerStrategy.IsKeyAlreadyUsedAsync(key);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsKeyAlreadyUsedAsync_KeyExistsInDistributedCache_ReturnsTrue()
    {
        // Arrange
        const string key = "testKey";
        const string value = "value";
        _cacheMock.Setup(c => c.GetAsync(It.Is<string>(k => k == key), default))
            .ReturnsAsync(System.Text.Encoding.UTF8.GetBytes(value));

        // Act
        var result = await _localCacheManagerStrategy.IsKeyAlreadyUsedAsync(key);

        // Assert
        Assert.True(result);
        Assert.True(LocalCacheManagerStrategyTestsAccessor.LocalCacheContains(key));
    }

    [Fact]
    public async Task IsKeyAlreadyUsedAsync_KeyDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const string key = "testKey";
        _cacheMock.Setup(c => c.GetAsync(It.Is<string>(k => k == key), default))
            .ReturnsAsync((byte[])null!);

        // Act
        var result = await _localCacheManagerStrategy.IsKeyAlreadyUsedAsync(key);

        // Assert
        Assert.False(result);
    }
}

public static class LocalCacheManagerStrategyTestsAccessor
{
    public static void AddToLocalCache(string key)
    {
        var localCacheField = typeof(LocalCacheManagerStrategy)
            .GetField("LocalCache", BindingFlags.Static | BindingFlags.NonPublic);

        if (localCacheField == null) return;
        var localCache = localCacheField.GetValue(null) as ConcurrentDictionary<string, bool>;
        localCache?.TryAdd(key, true);
    }

    public static bool LocalCacheContains(string key)
    {
        var localCacheField = typeof(LocalCacheManagerStrategy)
            .GetField("LocalCache", BindingFlags.Static | BindingFlags.NonPublic);

        if (localCacheField == null) return false;
        var localCache = localCacheField.GetValue(null) as ConcurrentDictionary<string, bool>;
        return localCache != null && localCache.ContainsKey(key);
    }
}