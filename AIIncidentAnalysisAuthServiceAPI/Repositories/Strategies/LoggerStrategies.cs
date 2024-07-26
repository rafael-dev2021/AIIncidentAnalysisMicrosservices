using Serilog;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies;

public class LoggerStrategies : ILoggerStrategies
{
    public void LogInformation(string message, params object[] args)
    {
        Log.Information(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        Log.Warning(message, args);
    }
}