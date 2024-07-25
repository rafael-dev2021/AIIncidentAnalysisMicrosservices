using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser;
using FluentAssertions;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories.Strategies.UpdateUser;

public class UserFieldUpdaterStrategyTests
{
    private readonly UserFieldUpdaterStrategy _userFieldUpdaterStrategy;

    public UserFieldUpdaterStrategyTests()
    {
        _userFieldUpdaterStrategy = new UserFieldUpdaterStrategy();
    }

    [Fact(DisplayName = "Should update user email")]
    public void UpdateFields_Should_UpdateUserEmail()
    {
        // Arrange
        var user = new PoliceOfficer { Email = "oldemail@example.com" };
        var updateUserDtoRequest = new UpdateUserDtoRequest(
            "NewName",
            "NewLastName",
            "newemail@example.com",
            "+1234567890"
        );

        // Act
        _userFieldUpdaterStrategy.UpdateFields(user, updateUserDtoRequest);

        // Assert
        user.Email.Should().Be("newemail@example.com");
    }

    [Fact(DisplayName = "Should not update user email when new email is null")]
    public void UpdateFields_Should_NotUpdateUserEmail_When_NewEmailIsNull()
    {
        // Arrange
        var user = new PoliceOfficer { Email = "oldemail@example.com" };
        var updateUserDtoRequest = new UpdateUserDtoRequest(
            "NewName",
            "NewLastName",
            null,
            "+1234567890"
        );

        // Act
        _userFieldUpdaterStrategy.UpdateFields(user, updateUserDtoRequest);

        // Assert
        user.Email.Should().Be("oldemail@example.com");
    }

    [Fact(DisplayName = "Should update user phone number")]
    public void UpdateFields_Should_UpdateUserPhoneNumber()
    {
        // Arrange
        var user = new PoliceOfficer { PhoneNumber = "+9876543210" };
        var updateUserDtoRequest = new UpdateUserDtoRequest(
            "NewName",
            "NewLastName",
            "newemail@example.com",
            "+1234567890"
        );

        // Act
        _userFieldUpdaterStrategy.UpdateFields(user, updateUserDtoRequest);

        // Assert
        user.PhoneNumber.Should().Be("+1234567890");
    }

    [Fact(DisplayName = "Should update user name")]
    public void UpdateFields_Should_UpdateUserName()
    {
        // Arrange
        var user = new PoliceOfficer();
        user.SetName("OldName");
        var updateUserDtoRequest = new UpdateUserDtoRequest(
            "NewName",
            "NewLastName",
            "newemail@example.com",
            "+1234567890"
        );

        // Act
        _userFieldUpdaterStrategy.UpdateFields(user, updateUserDtoRequest);

        // Assert
        user.Name.Should().Be("NewName");
    }

    [Fact(DisplayName = "Should update user last name")]
    public void UpdateFields_Should_UpdateUserLastName()
    {
        // Arrange
        var user = new PoliceOfficer();
        user.SetLastName("OldLastName");
        var updateUserDtoRequest = new UpdateUserDtoRequest(
            "NewName",
            "NewLastName",
            "newemail@example.com",
            "+1234567890"
        );

        // Act
        _userFieldUpdaterStrategy.UpdateFields(user, updateUserDtoRequest);

        // Assert
        user.LastName.Should().Be("NewLastName");
    }

    [Fact(DisplayName = "Should not update fields when new values are default")]
    public void UpdateFields_Should_NotUpdateFields_When_NewValuesAreDefault()
    {
        // Arrange
        var user = new PoliceOfficer();
        user.SetName("OldName");
        user.SetLastName("OldLastName");
        user.Email = "oldemail@example.com";
        user.PhoneNumber = "+9876543210";

        var updateUserDtoRequest = new UpdateUserDtoRequest(
            default,
            default,
            default,
            default
        );

        // Act
        _userFieldUpdaterStrategy.UpdateFields(user, updateUserDtoRequest);

        // Assert
        user.Name.Should().Be("OldName");
        user.LastName.Should().Be("OldLastName");
        user.Email.Should().Be("oldemail@example.com");
        user.PhoneNumber.Should().Be("+9876543210");
    }
}