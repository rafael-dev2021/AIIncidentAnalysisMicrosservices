using System.Collections.Concurrent;
using System.Text;
using AIIncidentAnalysisAuthServiceAPI.Exceptions;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser;

public class LoginAttemptsManagerStrategy(IDistributedCache cache) : ILoginAttemptsManagerStrategy
{
    public const string LoginAttemptsKeyPrefix = "login_attempts";
    private static readonly SemaphoreSlim Semaphore = new(10);
    private static readonly ConcurrentDictionary<string, int> LocalCache = new();

    public async Task<int> GetLoginAttemptsAsync(string cacheKey)
    {
        try
        {
            if (LocalCache.TryGetValue(cacheKey, out var attempts))
            {
                return attempts;
            }

            var attemptsStr = await cache.GetStringAsync(cacheKey);
            attempts = attemptsStr != null ? int.Parse(attemptsStr) : 0;

            LocalCache[cacheKey] = attempts;

            return attempts;
        }
        catch (CacheException ex)
        {
            throw new CacheException($"[CACHE] Error retrieving login attempts for cache key [{cacheKey}]", ex);
        }
    }

    public async Task IncrementLoginAttemptsAsync(string cacheKey)
    {
        await Semaphore.WaitAsync();

        try
        {
            var attemptsBytes = await cache.GetAsync(cacheKey);
            var attempts = attemptsBytes != null ? int.Parse(Encoding.UTF8.GetString(attemptsBytes)) : 0;
            attempts++;

            await cache.SetStringAsync(cacheKey, attempts.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            LocalCache[cacheKey] = attempts;
        }
        catch (CacheException ex)
        {
            throw new CacheException($"[CACHE] Error incrementing login attempts for cache key [{cacheKey}]", ex);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public async Task ResetLoginAttemptsAsync(string cacheKey)
    {
        await Semaphore.WaitAsync();

        try
        {
            await cache.RemoveAsync(cacheKey);
            LocalCache.TryRemove(cacheKey, out _);
        }
        catch (CacheException ex)
        {
            throw new CacheException($"[CACHE] Error resetting login attempts for cache key [{cacheKey}]", ex);
        }
        finally
        {
            Semaphore.Release();
        }
    }
}