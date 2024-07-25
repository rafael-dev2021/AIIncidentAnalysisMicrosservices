using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Serilog;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories.Strategies.AuthUser;

public class AuthStrategyTests
{
    private readonly Mock<SignInManager<PoliceOfficer>> _signInManagerMock;
    private readonly Mock<ILoginAttemptsManagerStrategy> _loginAttemptsManagerStrategyMock;
    private readonly Mock<IAuthenticationLoggerStrategy> _authenticationLoggerStrategyMock;
    private readonly AuthStrategy _authStrategy;

    public AuthStrategyTests()
    {
        var store = new Mock<IUserStore<PoliceOfficer>>();
        var userManagerMock = new Mock<UserManager<PoliceOfficer>>(store.Object, null!, null!, null!, null!, null!,
            null!, null!, null!);

        _signInManagerMock = new Mock<SignInManager<PoliceOfficer>>(
            userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<PoliceOfficer>>().Object,
            null!, null!, null!, null!);

        _loginAttemptsManagerStrategyMock = new Mock<ILoginAttemptsManagerStrategy>();
        _authenticationLoggerStrategyMock = new Mock<IAuthenticationLoggerStrategy>();

        _authStrategy = new AuthStrategy(
            _signInManagerMock.Object,
            _loginAttemptsManagerStrategyMock.Object,
            _authenticationLoggerStrategyMock.Object
        );

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
    }

    [Fact]
    public async Task AuthenticatedAsync_SuccessfulLogin_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new LoginDtoRequest("user@example.com", "password", true);
        _loginAttemptsManagerStrategyMock.Setup(m => m.GetLoginAttemptsAsync(It.IsAny<string>()))
            .ReturnsAsync(0);
        _signInManagerMock.Setup(s =>
                s.PasswordSignInAsync(request.Email!, request.Password!, request.RememberMe, false))
            .ReturnsAsync(SignInResult.Success);

        // Act
        var result = await _authStrategy.AuthenticatedAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Login successfully.", result.Message);
        _loginAttemptsManagerStrategyMock.Verify(m => m.ResetLoginAttemptsAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task AuthenticatedAsync_AccountLocked_ReturnsLockedResponse()
    {
        // Arrange
        var request = new LoginDtoRequest("user@example.com", "password", true);
        _loginAttemptsManagerStrategyMock.Setup(m => m.GetLoginAttemptsAsync(It.IsAny<string>()))
            .ReturnsAsync(3);
        _signInManagerMock.Setup(s =>
                s.PasswordSignInAsync(request.Email!, request.Password!, request.RememberMe, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _authStrategy.AuthenticatedAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Your account is locked. Please contact support.", result.Message);
        _authenticationLoggerStrategyMock.Verify(m => m.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task AuthenticatedAsync_InvalidPassword_IncrementsLoginAttempts()
    {
        // Arrange
        var request = new LoginDtoRequest("user@example.com", "wrongpassword", true);
        _loginAttemptsManagerStrategyMock.Setup(m => m.GetLoginAttemptsAsync(It.IsAny<string>()))
            .ReturnsAsync(1);
        _signInManagerMock.Setup(s =>
                s.PasswordSignInAsync(request.Email!, request.Password!, request.RememberMe, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _authStrategy.AuthenticatedAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid email or password. Please try again.", result.Message);
        _loginAttemptsManagerStrategyMock.Verify(m => m.IncrementLoginAttemptsAsync(It.IsAny<string>()),
            Times.Once);
    }
}