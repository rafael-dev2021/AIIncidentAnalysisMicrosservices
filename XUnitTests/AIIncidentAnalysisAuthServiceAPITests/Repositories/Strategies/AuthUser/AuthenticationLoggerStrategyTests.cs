using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories.Strategies.AuthUser;

public class AuthenticationLoggerStrategyTests
{
    private readonly AuthenticationLoggerStrategy _loggerStrategy;
    private readonly MockSink _mockSink;

    public AuthenticationLoggerStrategyTests()
    {
        _mockSink = new MockSink();
        ILogger logger = new LoggerConfiguration()
            .WriteTo.Sink(_mockSink)
            .CreateLogger();

        Log.Logger = logger;
        _loggerStrategy = new AuthenticationLoggerStrategy();
    }

    [Fact]
    public void LogInformation_CallsSerilogWithCorrectMessage()
    {
        // Arrange
        const string message = "Information message with {Parameter}";
        const string parameter = "value";

        // Act
        _loggerStrategy.LogInformation(message, parameter);

        // Assert
        Assert.Single(_mockSink.LogEvents);
        var logEvent = _mockSink.LogEvents[0];
        Assert.Equal(LogEventLevel.Information, logEvent.Level);
        Assert.Equal(message, logEvent.MessageTemplate.Text);
        Assert.Contains(parameter, logEvent.Properties["Parameter"].ToString());
    }

    [Fact]
    public void LogWarning_CallsSerilogWithCorrectMessage()
    {
        // Arrange
        const string message = "Warning message with {Parameter}";
        const string parameter = "value";

        // Act
        _loggerStrategy.LogWarning(message, parameter);

        // Assert
        Assert.Single(_mockSink.LogEvents);
        var logEvent = _mockSink.LogEvents[0];
        Assert.Equal(LogEventLevel.Warning, logEvent.Level);
        Assert.Equal(message, logEvent.MessageTemplate.Text);
        Assert.Contains(parameter, logEvent.Properties["Parameter"].ToString());
    }
}

public class MockSink : ILogEventSink
{
    public List<LogEvent> LogEvents { get; } = [];

    public void Emit(LogEvent logEvent)
    {
        LogEvents.Add(logEvent);
    }
}

