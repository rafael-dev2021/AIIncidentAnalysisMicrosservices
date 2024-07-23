using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;
using Serilog;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser;

public class RegistrationLoggerStrategy : IRegistrationLoggerStrategy
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