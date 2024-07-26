using AIIncidentAnalysisAuthServiceAPI.Algorithms.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories.Strategies.RegisterUser;

public class RegisterStrategyTests : IDisposable
{
    private readonly Mock<UserManager<PoliceOfficer>> _userManagerMock;
    private readonly AppDbContext _appDbContext;
    private readonly RegisterStrategy _registerStrategy;
    private readonly Mock<IUserValidationManagerStrategy> _userValidationManagerStrategyMock;
    private readonly Mock<IAccountNumberGenerator> _accountNumberGeneratorMock;

    public RegisterStrategyTests()
    {
        var userStoreMock = new Mock<IUserStore<PoliceOfficer>>();
        _userManagerMock = new Mock<UserManager<PoliceOfficer>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<PoliceOfficer>>();
        Mock<SignInManager<PoliceOfficer>> signInManagerMock = new(_userManagerMock.Object, contextAccessorMock.Object, claimsFactoryMock.Object, null!, null!, null!, null!);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _appDbContext = new AppDbContext(options);

        _userValidationManagerStrategyMock = new Mock<IUserValidationManagerStrategy>();
        Mock<ILoggerStrategies> registrationLoggerStrategyMock = new();
        _accountNumberGeneratorMock = new Mock<IAccountNumberGenerator>();

        _registerStrategy = new RegisterStrategy(
            signInManagerMock.Object, 
            _userManagerMock.Object, 
            _appDbContext, 
            _userValidationManagerStrategyMock.Object, 
            registrationLoggerStrategyMock.Object, 
            _accountNumberGeneratorMock.Object);
    }

    [Fact(DisplayName = "Should return successful registration response")]
    public async Task CreateUserAsync_Should_Return_SuccessfulRegistrationResponse()
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "John",
            "Doe",
            "john@example.com",
            "+1234567890",
            "123.456.789-10",
            new DateTime(1980, 1, 1),
            DateTime.Now,
            ERank.Constable,
            EDepartment.TrafficDivision,
            EOfficerStatus.Active,
            EAccessLevel.ReadOnly,
            "Password123!",
            "Password123!"
        );

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<PoliceOfficer>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<PoliceOfficer>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userValidationManagerStrategyMock.Setup(uv =>
                uv.ValidateUserDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<string>());
        _accountNumberGeneratorMock.Setup(ag => ag.GenerateIdentificationNumberAsync())
            .ReturnsAsync("ID123");
        _accountNumberGeneratorMock.Setup(ag => ag.GenerateBadgeNumberAsync())
            .ReturnsAsync("BADGE123");

        // Act
        var response = await _registerStrategy.CreateUserAsync(request);

        // Assert
        response.Should().BeEquivalentTo(new RegisterDtoResponse(true, "Registration successful."));
    }

    [Fact(DisplayName = "Should return error when user creation fails")]
    public async Task CreateUserAsync_Should_Return_Error_When_User_Creation_Captain_Fails()
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "ReadWrite",
            "ReadWrite",
            "readwrite@localhost.com",
            "+5512345681",
            "123.456.789-13",
            new DateTime(2000, 2, 15),
            DateTime.Now.AddYears(-2),
            ERank.Captain,
            EDepartment.TrafficDivision,
            EOfficerStatus.Active,
            EAccessLevel.ReadWrite,
            "@Visual24k+",
            "@Visual24k+"
        );

        _userValidationManagerStrategyMock
            .Setup(x => x.ValidateUserDetailsAsync(request.Cpf, request.Email, request.PhoneNumber))
            .ReturnsAsync([]);

        _accountNumberGeneratorMock.Setup(x => x.GenerateIdentificationNumberAsync())
            .ReturnsAsync("ID12345");

        _accountNumberGeneratorMock.Setup(x => x.GenerateBadgeNumberAsync())
            .ReturnsAsync("BADGE123");

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<PoliceOfficer>(), request.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

        // Act
        var response = await _registerStrategy.CreateUserAsync(request);

        // Assert
        response.Should().BeEquivalentTo(new RegisterDtoResponse(false, "Registration failed."));
    }
    
    [Fact(DisplayName = "Should return error response when internal exception occurs during registration")]
    public async Task CreateUserAsync_Should_Return_ErrorResponse_When_InternalExceptionOccurs()
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "John",
            "Doe",
            "john@example.com",
            "+1234567890",
            "123.456.789-10",
            new DateTime(1980, 1, 1),
            DateTime.Now,
            ERank.Constable,
            EDepartment.TrafficDivision,
            EOfficerStatus.Active,
            EAccessLevel.ReadOnly,
            "Password123!",
            "Password123!"
        );

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<PoliceOfficer>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Internal error"));

        _userValidationManagerStrategyMock.Setup(uv =>
                uv.ValidateUserDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<string>());
        _accountNumberGeneratorMock.Setup(ag => ag.GenerateIdentificationNumberAsync())
            .ReturnsAsync("ID123");
        _accountNumberGeneratorMock.Setup(ag => ag.GenerateBadgeNumberAsync())
            .ReturnsAsync("BADGE123");

        // Act
        var response = await _registerStrategy.CreateUserAsync(request);

        // Assert
        response.Should().BeEquivalentTo(new RegisterDtoResponse(false, "Registration failed due to an internal error."));
    }
    
    [Fact(DisplayName = "Should return error when user creation fails")]
    public async Task CreateUserAsync_Should_Return_Error_When_User_Creation_Sergeant_Fails()
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "ReadWrite",
            "ReadWrite",
            "readwrite@localhost.com",
            "+5512345681",
            "123.456.789-13",
            new DateTime(2000, 2, 15),
            DateTime.Now.AddYears(-2),
            ERank.Sergeant,
            EDepartment.TrafficDivision,
            EOfficerStatus.Active,
            EAccessLevel.ReadWrite,
            "@Visual24k+",
            "@Visual24k+"
        );

        _userValidationManagerStrategyMock
            .Setup(x => x.ValidateUserDetailsAsync(request.Cpf, request.Email, request.PhoneNumber))
            .ReturnsAsync([]);

        _accountNumberGeneratorMock.Setup(x => x.GenerateIdentificationNumberAsync())
            .ReturnsAsync("ID12345");

        _accountNumberGeneratorMock.Setup(x => x.GenerateBadgeNumberAsync())
            .ReturnsAsync("BADGE123");

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<PoliceOfficer>(), request.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

        // Act
        var response = await _registerStrategy.CreateUserAsync(request);

        // Assert
        response.Should().BeEquivalentTo(new RegisterDtoResponse(false, "Registration failed."));
    }
    
    [Fact(DisplayName = "Should return error when user creation fails")]
    public async Task CreateUserAsync_Should_Return_Error_When_User_Creation_Undefined_Fails()
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "ReadWrite",
            "ReadWrite",
            "readwrite@localhost.com",
            "+5512345681",
            "123.456.789-13",
            new DateTime(2000, 2, 15),
            DateTime.Now.AddYears(-2),
            ERank.Undefined,
            EDepartment.TrafficDivision,
            EOfficerStatus.Active,
            EAccessLevel.ReadWrite,
            "@Visual24k+",
            "@Visual24k+"
        );

        _userValidationManagerStrategyMock
            .Setup(x => x.ValidateUserDetailsAsync(request.Cpf, request.Email, request.PhoneNumber))
            .ReturnsAsync([]);

        _accountNumberGeneratorMock.Setup(x => x.GenerateIdentificationNumberAsync())
            .ReturnsAsync("ID12345");

        _accountNumberGeneratorMock.Setup(x => x.GenerateBadgeNumberAsync())
            .ReturnsAsync("BADGE123");

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<PoliceOfficer>(), request.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

        // Act
        var response = await _registerStrategy.CreateUserAsync(request);

        // Assert
        response.Should().BeEquivalentTo(new RegisterDtoResponse(false, "Registration failed."));
    }

    public void Dispose()
    {
        _appDbContext.Database.EnsureDeleted();
        _appDbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}