using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using FluentValidation.TestHelper;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Dto.Request;

public class RegisterDtoRequestValidatorTests
{
    private readonly RegisterDtoRequestValidation _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
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

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Name_Is_Null_Or_Empty(string? name)
    {
        // Arrange
        var request = new RegisterDtoRequest(
            name,
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

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_LastName_Is_Null_Or_Empty(string? lastName)
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "ReadWrite",
            lastName,
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

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last Name is required.");
    }

    [Theory]
    [InlineData("invalid-phone")]
    public void Should_Have_Error_When_PhoneNumber_Is_Invalid(string phoneNumber)
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "ReadWrite",
            "ReadWrite",
            "readwrite@localhost.com",
            phoneNumber,
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

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("invalid-cpf")]
    public void Should_Have_Error_When_Cpf_Is_Invalid(string cpf)
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "ReadWrite",
            "ReadWrite",
            "readwrite@localhost.com",
            "+5512345681",
            cpf,
            new DateTime(2000, 2, 15),
            DateTime.Now.AddYears(-2),
            ERank.Sergeant,
            EDepartment.TrafficDivision,
            EOfficerStatus.Active,
            EAccessLevel.ReadWrite,
            "@Visual24k+",
            "@Visual24k+"
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf);
    }

    [Theory]
    [InlineData("invalid-email")]
    public void Should_Have_Error_When_Email_Is_Invalid(string email)
    {
        // Arrange
        var request = new RegisterDtoRequest(
            "ReadWrite",
            "ReadWrite",
            email,
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

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Passwords_Do_Not_Match()
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
            "@Visual224k+"
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
            .WithErrorMessage("Passwords do not match.");
    }
}