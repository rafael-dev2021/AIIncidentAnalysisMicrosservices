using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories.Strategies.UpdateUser;

public class UserDetailsValidatorStrategyTests : IDisposable
{
    private readonly AppDbContext _appDbContext;
    private readonly UserDetailsValidatorStrategy _userDetailsValidatorStrategy;

    public UserDetailsValidatorStrategyTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _appDbContext = new AppDbContext(options);
        _userDetailsValidatorStrategy = new UserDetailsValidatorStrategy(_appDbContext);
    }

    [Fact(DisplayName = "Should return validation error for email already used by another user")]
    public async Task ValidateAsync_Should_ReturnValidationError_ForEmailAlreadyUsedByAnotherUser()
    {
        // Arrange
        var existingUser = new PoliceOfficer { Id = "1", Email = "existing@example.com", PhoneNumber = "+1234567890" };
        existingUser.SetIdentificationNumber("SD324D2");
        existingUser.SetName("John Doe");
        existingUser.SetLastName("John Doe");
        existingUser.SetCpf("123.456.342-21");
        existingUser.SetBadgeNumber("RF2313");
        existingUser.SetRole("Admin");
        existingUser.SetDateOfBirth(DateTime.Now.AddYears(25));
        existingUser.SetDateOfJoining(DateTime.Now.AddYears(-5));
        existingUser.SetERank(ERank.Captain);
        existingUser.SetEDepartment(EDepartment.Administrative);
        existingUser.SetEOfficerStatus(EOfficerStatus.Active);
        existingUser.SetEAccessLevel(EAccessLevel.Admin);

        await _appDbContext.Users.AddAsync(existingUser);
        await _appDbContext.SaveChangesAsync();

        var user = new PoliceOfficer { Id = "2", Email = "john@example.com", PhoneNumber = "+1234567890" };
        user.SetIdentificationNumber("SD324D2");
        user.SetName("NewName");
        user.SetLastName("NewLastName");
        user.SetCpf("123.456.342-26");
        user.SetBadgeNumber("RF2313");
        user.SetRole("Admin");
        user.SetDateOfBirth(DateTime.Now.AddYears(25));
        user.SetDateOfJoining(DateTime.Now.AddYears(-5));
        user.SetERank(ERank.Captain);
        user.SetEDepartment(EDepartment.Administrative);
        user.SetEOfficerStatus(EOfficerStatus.Active);
        user.SetEAccessLevel(EAccessLevel.Admin);
        var updateUserDtoRequest = new UpdateUserDtoRequest(
            "NewName",
            "NewLastName",
            "existing@example.com",
            "+1234567890"
        );

        // Act
        var validationErrors = await _userDetailsValidatorStrategy.ValidateAsync(updateUserDtoRequest, user);

        // Assert
        validationErrors.Should().Contain("Email already used by another user.");
    }

    [Fact(DisplayName = "Should return validation error for phone number already used by another user")]
    public async Task ValidateAsync_Should_ReturnValidationError_ForPhoneNumberAlreadyUsedByAnotherUser()
    {
        // Arrange
        var existingUser = new PoliceOfficer { Id = "1", Email = "john@example.com", PhoneNumber = "+0987654321" };
        existingUser.SetIdentificationNumber("SD324D2");
        existingUser.SetName("John Doe");
        existingUser.SetLastName("John Doe");
        existingUser.SetCpf("123.456.342-21");
        existingUser.SetBadgeNumber("RF2313");
        existingUser.SetRole("Admin");
        existingUser.SetDateOfBirth(DateTime.Now.AddYears(25));
        existingUser.SetDateOfJoining(DateTime.Now.AddYears(-5));
        existingUser.SetERank(ERank.Captain);
        existingUser.SetEDepartment(EDepartment.Administrative);
        existingUser.SetEOfficerStatus(EOfficerStatus.Active);
        existingUser.SetEAccessLevel(EAccessLevel.Admin);
        await _appDbContext.Users.AddAsync(existingUser);
        await _appDbContext.SaveChangesAsync();

        var user = new PoliceOfficer { Id = "2", Email = "john@example.com", PhoneNumber = "+1234567890" };
        user.SetIdentificationNumber("SD324D2");
        user.SetName("NewName");
        user.SetLastName("NewLastName");
        user.SetCpf("123.456.342-26");
        user.SetBadgeNumber("RF2313");
        user.SetRole("Admin");
        user.SetDateOfBirth(DateTime.Now.AddYears(25));
        user.SetDateOfJoining(DateTime.Now.AddYears(-5));
        user.SetERank(ERank.Captain);
        user.SetEDepartment(EDepartment.Administrative);
        user.SetEOfficerStatus(EOfficerStatus.Active);
        user.SetEAccessLevel(EAccessLevel.Admin);

        var updateUserDtoRequest = new UpdateUserDtoRequest(
            "NewName",
            "NewLastName",
            "newemail@example.com",
            "+0987654321"
        );

        // Act
        var validationErrors = await _userDetailsValidatorStrategy.ValidateAsync(updateUserDtoRequest, user);

        // Assert
        validationErrors.Should().Contain("Phone number already used by another user.");
    }

    [Fact(DisplayName = "Should not return validation errors for valid update request")]
    public async Task ValidateAsync_Should_NotReturnValidationErrors_ForValidUpdateRequest()
    {
        // Arrange
        var user = new PoliceOfficer { Id = "1", Email = "john@example.com", PhoneNumber = "+1234567890" };
        user.SetIdentificationNumber("SD324D2");
        user.SetName("John Doe");
        user.SetLastName("John Doe");
        user.SetCpf("123.456.342-21");
        user.SetBadgeNumber("RF2313");
        user.SetRole("Admin");
        user.SetDateOfBirth(DateTime.Now.AddYears(25));
        user.SetDateOfJoining(DateTime.Now.AddYears(-5));
        user.SetERank(ERank.Captain);
        user.SetEDepartment(EDepartment.Administrative);
        user.SetEOfficerStatus(EOfficerStatus.Active);
        user.SetEAccessLevel(EAccessLevel.Admin);

        var updateUserDtoRequest = new UpdateUserDtoRequest(
            "NewName",
            "NewLastName",
            "john@example.com",
            "+1234567890"
        );

        // Act
        var validationErrors = await _userDetailsValidatorStrategy.ValidateAsync(updateUserDtoRequest, user);

        // Assert
        validationErrors.Should().BeEmpty();
    }

    public void Dispose()
    {
        _appDbContext.Database.EnsureDeleted();
        _appDbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}