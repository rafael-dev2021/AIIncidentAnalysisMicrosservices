using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies;
using Moq;
using Serilog;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories.Strategies;

public class LoggerStrategiesTests : LoggerTestBase  
{
    [Fact]
    public void LogInformation_ShouldCallLogInformation()
    {
        // Arrange
        var logger = new Mock<ILogger>();
        Log.Logger = logger.Object;
        var strategy = new LoggerStrategies();
        const string message = "Test message";
        var args = new object[] { "arg1", "arg2" };

        // Act
        strategy.LogInformation(message, args);

        // Assert
        logger.Verify(x => x.Information(message, args), Times.Once);
    }

    [Fact]
    public void LogWarning_ShouldCallLogWarning()
    {
        // Arrange
        var logger = new Mock<ILogger>();
        Log.Logger = logger.Object;
        var strategy = new LoggerStrategies();
        const string message = "Test warning";
        var args = new object[] { "arg1", "arg2" };

        // Act
        strategy.LogWarning(message, args);

        // Assert
        logger.Verify(x => x.Warning(message, args), Times.Once);
    }
}