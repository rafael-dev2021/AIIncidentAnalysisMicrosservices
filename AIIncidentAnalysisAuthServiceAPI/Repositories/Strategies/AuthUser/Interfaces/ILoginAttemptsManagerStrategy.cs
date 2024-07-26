namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser.Interfaces;

public interface ILoginAttemptsManagerStrategy
{
    Task<int> GetLoginAttemptsAsync(string cacheKey);
    Task IncrementLoginAttemptsAsync(string cacheKey);
    Task ResetLoginAttemptsAsync(string cacheKey);
}