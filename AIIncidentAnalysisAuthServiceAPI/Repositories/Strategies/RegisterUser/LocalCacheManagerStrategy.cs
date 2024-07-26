using System.Collections.Concurrent;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser;

public class LocalCacheManagerStrategy(IDistributedCache cache) : ILocalCacheManagerStrategy
{
    private static readonly ConcurrentDictionary<string, bool> LocalCache = new();

    public async Task<bool> IsKeyAlreadyUsedAsync(string key)
    {
        if (LocalCache.TryGetValue(key, out _))
            return true;

        var exists = await cache.GetStringAsync(key) != null;

        if (exists)
            LocalCache.TryAdd(key, true);

        return exists;
    }
}