using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories.Strategies.UpdateUser;

public class UpdateProfileStrategyTests : IDisposable
{
    private readonly Mock<UserManager<PoliceOfficer>> _userManagerMock;
    private readonly AppDbContext _appDbContext;
    private readonly Mock<IUserDetailsValidatorStrategy> _userDetailsValidatorStrategyMock;
    private readonly Mock<ITransactionHandlerStrategy> _transactionHandlerStrategyMock;
    private readonly Mock<IUserFieldUpdaterStrategy> _userFieldUpdaterStrategyMock;
    private readonly UpdateProfileStrategy _updateProfileStrategy;

    public UpdateProfileStrategyTests()
    {
        _userFieldUpdaterStrategyMock = new Mock<IUserFieldUpdaterStrategy>();
        var userStoreMock = new Mock<IUserStore<PoliceOfficer>>();
        _userManagerMock = new Mock<UserManager<PoliceOfficer>>(userStoreMock.Object, null!, null!, null!, null!, null!,
            null!, null!, null!);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _appDbContext = new AppDbContext(options);

        _userDetailsValidatorStrategyMock = new Mock<IUserDetailsValidatorStrategy>();
        Mock<IUserFieldUpdaterStrategy> userFieldUpdaterStrategyMock = new();
        _transactionHandlerStrategyMock = new Mock<ITransactionHandlerStrategy>();

        _updateProfileStrategy = new UpdateProfileStrategy(
            _userManagerMock.Object,
            _appDbContext,
            _userDetailsValidatorStrategyMock.Object,
            userFieldUpdaterStrategyMock.Object,
            _transactionHandlerStrategyMock.Object
        );
    }

    [Fact(DisplayName = "Should return error response when validation errors exist")]
    public async Task UpdateProfileAsync_Should_Return_ErrorResponse_When_ValidationErrorsExist()
    {
        // Arrange
        const string userId = "123";
        var user = new PoliceOfficer { Id = userId, Email = "john@example.com" };
        var updateUserDtoRequest = new UpdateUserDtoRequest(
            "NewName",
            "NewLastName",
            "newemail@example.com",
            "+1234567890"
        );

        var validationErrors = new List<string> { "Email is invalid.", "Phone number is invalid." };

        _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userDetailsValidatorStrategyMock
            .Setup(uv => uv.ValidateAsync(It.IsAny<UpdateUserDtoRequest>(), It.IsAny<PoliceOfficer>()))
            .ReturnsAsync(validationErrors);

        // Act
        var response = await _updateProfileStrategy.UpdateProfileAsync(updateUserDtoRequest, userId);

        // Assert
        response.Should()
            .BeEquivalentTo(new UpdateDtoResponse(false, string.Join(Environment.NewLine, validationErrors)));
        _userDetailsValidatorStrategyMock.Verify(uv => uv.LogValidationErrors(userId, validationErrors), Times.Once);
        _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<PoliceOfficer>()), Times.Never);
    }

    [Fact(DisplayName = "Should return successful update response")]
    public async Task UpdateProfileAsync_Should_Return_SuccessfulUpdateResponse()
    {
        // Arrange
        const string userId = "123";
        var user = new PoliceOfficer { Id = userId, Email = "john@example.com" };
        var updateUserDtoRequest = new UpdateUserDtoRequest(
            "NewName",
            "NewLastName",
            "newemail@example.com",
            "+1234567890"
        );

        _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userDetailsValidatorStrategyMock
            .Setup(uv => uv.ValidateAsync(It.IsAny<UpdateUserDtoRequest>(), It.IsAny<PoliceOfficer>()))
            .ReturnsAsync([]);
        _transactionHandlerStrategyMock.Setup(th =>
                th.ExecuteInTransactionAsync(It.IsAny<AppDbContext>(), It.IsAny<Func<Task<UpdateDtoResponse>>>()))
            .ReturnsAsync(new UpdateDtoResponse(true, "Profile updated successfully."));

        // Act
        var response = await _updateProfileStrategy.UpdateProfileAsync(updateUserDtoRequest, userId);

        // Assert
        response.Should().BeEquivalentTo(new UpdateDtoResponse(true, "Profile updated successfully."));
    }

    [Fact(DisplayName = "Should return error response when internal exception occurs during profile update")]
    public async Task UpdateProfileAsync_Should_Return_ErrorResponse_When_InternalExceptionOccurs()
    {
        // Arrange
        const string userId = "123";
        var user = new PoliceOfficer { Id = userId, Email = "john@example.com" };
        var updateUserDtoRequest = new UpdateUserDtoRequest(
            "NewName",
            "NewLastName",
            "newemail@example.com",
            "+1234567890"
        );

        _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userDetailsValidatorStrategyMock
            .Setup(uv => uv.ValidateAsync(It.IsAny<UpdateUserDtoRequest>(), It.IsAny<PoliceOfficer>()))
            .ReturnsAsync([]);
        _transactionHandlerStrategyMock.Setup(th =>
                th.ExecuteInTransactionAsync(It.IsAny<AppDbContext>(), It.IsAny<Func<Task<UpdateDtoResponse>>>()))
            .ThrowsAsync(new Exception("Internal error"));

        // Assert
        var exception = await Assert.ThrowsAsync<Exception>(Act);
        exception.Message.Should().Be("Internal error");
        return;

        // Act
        async Task Act() => await _updateProfileStrategy.UpdateProfileAsync(updateUserDtoRequest, userId);
    }


    [Fact(DisplayName = "Should return error response when user not found")]
    public async Task UpdateProfileAsync_Should_Return_ErrorResponse_When_UserNotFound()
    {
        // Arrange
        const string userId = "123";
        var updateUserDtoRequest = new UpdateUserDtoRequest(
            "NewName",
            "NewLastName",
            "newemail@example.com",
            "+1234567890"
        );

        _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((PoliceOfficer)null!);

        // Act
        var response = await _updateProfileStrategy.UpdateProfileAsync(updateUserDtoRequest, userId);

        // Assert
        response.Should().BeEquivalentTo(new UpdateDtoResponse(false, "User not found."));
        _userManagerMock.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
        _userDetailsValidatorStrategyMock.Verify(
            uv => uv.ValidateAsync(It.IsAny<UpdateUserDtoRequest>(), It.IsAny<PoliceOfficer>()), Times.Never);
        _userFieldUpdaterStrategyMock.Verify(
            uf => uf.UpdateFields(It.IsAny<PoliceOfficer>(), It.IsAny<UpdateUserDtoRequest>()), Times.Never);
        _transactionHandlerStrategyMock.Verify(
            th => th.ExecuteInTransactionAsync(It.IsAny<AppDbContext>(), It.IsAny<Func<Task<UpdateDtoResponse>>>()),
            Times.Never);
    }

    public void Dispose()
    {
        _appDbContext.Database.EnsureDeleted();
        _appDbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}