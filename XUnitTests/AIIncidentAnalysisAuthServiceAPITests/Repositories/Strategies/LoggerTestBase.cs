using Serilog;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories.Strategies;

public class LoggerTestBase : IDisposable
{
    private readonly ILogger _originalLogger = Log.Logger;

    public void Dispose()
    {
        Log.Logger = _originalLogger;
        GC.SuppressFinalize(this);
    }
}