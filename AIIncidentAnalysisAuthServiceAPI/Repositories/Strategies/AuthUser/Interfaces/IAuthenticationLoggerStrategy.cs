namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser.Interfaces;

public interface IAuthenticationLoggerStrategy
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
}