using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser.Interfaces;
using Serilog;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser;

public class AuthenticationLoggerStrategy : IAuthenticationLoggerStrategy
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