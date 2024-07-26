namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;

public interface ILocalCacheManagerStrategy
{
    Task<bool> IsKeyAlreadyUsedAsync(string key);
}