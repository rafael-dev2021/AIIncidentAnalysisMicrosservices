using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories.Strategies.RegisterUser;

public class UserValidationManagerStrategyTests
{
    private readonly Mock<ILocalCacheManagerStrategy> _localCacheManagerStrategyMock;
    private readonly UserValidationManagerStrategy _userValidationManagerStrategy;

    public UserValidationManagerStrategyTests()
    {
        _localCacheManagerStrategyMock = new Mock<ILocalCacheManagerStrategy>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var appDbContext = new AppDbContext(options);
        _userValidationManagerStrategy = new UserValidationManagerStrategy(
            _localCacheManagerStrategyMock.Object,
            appDbContext);
    }

    [Fact]
    public async Task ValidateUserDetailsAsync_CpfAlreadyUsed_ReturnsValidationError()
    {
        // Arrange
        const string cpf = "12345678900";

        _localCacheManagerStrategyMock
            .Setup(lc => lc.IsKeyAlreadyUsedAsync($"CPF:{cpf}"))
            .ReturnsAsync(true);

        // Act
        var result = await _userValidationManagerStrategy.ValidateUserDetailsAsync(cpf, null, null);

        // Assert
        Assert.Contains("[CPF already used]", result);
    }

    [Fact]
    public async Task ValidateUserDetailsAsync_EmailAlreadyUsed_ReturnsValidationError()
    {
        // Arrange
        const string email = "user@example.com";

        _localCacheManagerStrategyMock
            .Setup(lc => lc.IsKeyAlreadyUsedAsync($"Email:{email}"))
            .ReturnsAsync(true);

        // Act
        var result = await _userValidationManagerStrategy.ValidateUserDetailsAsync(null, email, null);

        // Assert
        Assert.Contains("[Email already used]", result);
    }

    [Fact]
    public async Task ValidateUserDetailsAsync_PhoneNumberAlreadyUsed_ReturnsValidationError()
    {
        // Arrange
        const string phoneNumber = "1234567890";

        _localCacheManagerStrategyMock
            .Setup(lc => lc.IsKeyAlreadyUsedAsync($"PhoneNumber:{phoneNumber}"))
            .ReturnsAsync(true);

        // Act
        var result = await _userValidationManagerStrategy.ValidateUserDetailsAsync(null, null, phoneNumber);

        // Assert
        Assert.Contains("[Phone number already used]", result);
    }


    [Fact]
    public async Task ValidateUserDetailsAsync_NoDuplicates_ReturnsEmptyList()
    {
        // Arrange
        const string cpf = "12345678900";
        const string email = "user@example.com";
        const string phoneNumber = "1234567890";

        _localCacheManagerStrategyMock
            .Setup(lc => lc.IsKeyAlreadyUsedAsync($"CPF:{cpf}"))
            .ReturnsAsync(false);
        _localCacheManagerStrategyMock
            .Setup(lc => lc.IsKeyAlreadyUsedAsync($"Email:{email}"))
            .ReturnsAsync(false);
        _localCacheManagerStrategyMock
            .Setup(lc => lc.IsKeyAlreadyUsedAsync($"PhoneNumber:{phoneNumber}"))
            .ReturnsAsync(false);

        // Act
        var result = await _userValidationManagerStrategy.ValidateUserDetailsAsync(cpf, email, phoneNumber);

        // Assert
        Assert.Empty(result);
    }
}