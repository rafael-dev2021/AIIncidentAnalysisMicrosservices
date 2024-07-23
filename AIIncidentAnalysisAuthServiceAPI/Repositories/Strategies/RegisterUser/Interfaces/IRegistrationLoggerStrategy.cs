namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;

public interface IRegistrationLoggerStrategy
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
}