namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies;

public interface ILoggerStrategies
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
}