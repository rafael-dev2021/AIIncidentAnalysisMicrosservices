using System.Text;
using AIIncidentAnalysisAuthServiceAPI.Exceptions;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories.Strategies.AuthUser;

public class LoginAttemptsManagerStrategyTests
{
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly LoginAttemptsManagerStrategy _loginAttemptsManagerStrategy;
    
    public LoginAttemptsManagerStrategyTests()
    {
        _cacheMock = new Mock<IDistributedCache>();
        _loginAttemptsManagerStrategy = new LoginAttemptsManagerStrategy(_cacheMock.Object);
    }

    [Fact]
    public async Task GetLoginAttemptsAsync_ReturnsCorrectLoginAttempts()
    {
        // Arrange
        const string cacheKey = "login_attempts_user@example.com";
        const int expectedAttempts = 3;
        var attemptsBytes = Encoding.UTF8.GetBytes(expectedAttempts.ToString());
        _cacheMock.Setup(c => c.GetAsync(cacheKey, default))
            .ReturnsAsync(attemptsBytes);

        // Act
        var result = await _loginAttemptsManagerStrategy.GetLoginAttemptsAsync(cacheKey);

        // Assert
        Assert.Equal(expectedAttempts, result);
        _cacheMock.Verify(c => c.GetAsync(cacheKey, default), Times.Once);
    }
    
    [Fact]
    public async Task IncrementLoginAttemptsAsync_CacheIncrementsAttempts()
    {
        // Arrange
        const string cacheKey = "login_attempts_user@example.com";
        var initialAttemptsBytes = "2"u8.ToArray();
        _cacheMock.Setup(c => c.GetAsync(cacheKey, default))
            .ReturnsAsync(initialAttemptsBytes);

        // Act
        await _loginAttemptsManagerStrategy.IncrementLoginAttemptsAsync(cacheKey);

        // Assert
        var expectedAttemptsBytes = "3"u8.ToArray();
        _cacheMock.Verify(c => c.SetAsync(cacheKey, expectedAttemptsBytes, It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
    }

    [Fact]
    public async Task IncrementLoginAttemptsAsync_ExceptionThrown_LogsErrorAndThrows()
    {
        // Arrange
        const string cacheKey = "login_attempts_user@example.com";
        var exception = new CacheException($"[CACHE] Error incrementing login attempts for cache key [{cacheKey}]",new Exception());

        _cacheMock.Setup(c => c.GetAsync(cacheKey, default)).ThrowsAsync(exception);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<CacheException>(() =>
            _loginAttemptsManagerStrategy.IncrementLoginAttemptsAsync(cacheKey));
        Assert.Equal(exception.Message, ex.Message);
        _cacheMock.Verify(c => c.GetAsync(cacheKey, default), Times.Once);
    }

    [Fact]
    public async Task ResetLoginAttemptsAsync_CacheResetsAttempts()
    {
        // Arrange
        const string cacheKey = "login_attempts_user@example.com";

        // Act
        await _loginAttemptsManagerStrategy.ResetLoginAttemptsAsync(cacheKey);

        // Assert
        _cacheMock.Verify(c => c.RemoveAsync(cacheKey, default), Times.Once);
    }

    [Fact]
    public async Task ResetLoginAttemptsAsync_ExceptionThrown_LogsErrorAndThrows()
    {
        // Arrange
        const string cacheKey = "login_attempts_user@example.com";
        var exception = new CacheException($"[CACHE] Error resetting login attempts for cache key [{cacheKey}]",new Exception());

        _cacheMock.Setup(c => c.RemoveAsync(cacheKey, default)).ThrowsAsync(exception);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<CacheException>(() =>
            _loginAttemptsManagerStrategy.ResetLoginAttemptsAsync(cacheKey));
        Assert.Equal(exception.Message, ex.Message);
        _cacheMock.Verify(c => c.RemoveAsync(cacheKey, default), Times.Once);
    }
}