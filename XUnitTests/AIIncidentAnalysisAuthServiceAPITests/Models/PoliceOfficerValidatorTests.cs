using AIIncidentAnalysisAuthServiceAPI.FluentValidations.Models;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Models;

public class PoliceOfficerValidatorTests
{
    private readonly PoliceOfficerValidation _validator = new();
    private readonly PoliceOfficer _model = new();

    [Fact]
    public void Should_Not_Have_Validation_Errors_For_Valid_Model()
    {
        _model.SetIdentificationNumber("12345");
        _model.SetName("John Doe");
        _model.SetLastName("John Doe");
        _model.SetBadgeNumber("S123456");
        _model.SetEmail("john.doe@example.com");
        _model.SetPhoneNumber("+123456789");
        _model.SetCpf("123.456.789-01");
        _model.SetDateOfBirth(DateTime.Now.AddYears(-30));
        _model.SetDateOfJoining(DateTime.Now.AddYears(-1));
        _model.SetERank(ERank.Sergeant);
        _model.SetEDepartment(EDepartment.TrafficDivision);
        _model.SetEOfficerStatus(EOfficerStatus.Active);
        _model.SetEAccessLevel(EAccessLevel.Admin);

        var result = _validator.TestValidate(_model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Validation_Error_When_IdentificationNumber_Is_Empty()
    {
        _model.SetIdentificationNumber(string.Empty);

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.IdentificationNumber)
            .WithErrorMessage("Identification Number is required.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_IdentificationNumber_Is_Too_Short()
    {
        _model.SetIdentificationNumber("123");

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.IdentificationNumber)
            .WithErrorMessage("Identification Number must be between 5 and 25 characters.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_Name_Is_Empty()
    {
        _model.SetName(string.Empty);

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_Name_Is_Too_Short()
    {
        _model.SetName("John");

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name must be between 5 and 15 characters long.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_LastName_Is_Empty()
    {
        _model.SetLastName(string.Empty);

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last Name is required.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_LastName_Is_Too_Short()
    {
        _model.SetLastName("Doe");

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last Name must be between 5 and 15 characters long.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_BadgeNumber_Is_Empty()
    {
        _model.SetBadgeNumber(string.Empty);

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.BadgeNumber)
            .WithErrorMessage("Badge number is required.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_BadgeNumber_Is_Too_Short()
    {
        _model.SetBadgeNumber("123");

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.BadgeNumber)
            .WithErrorMessage("Badge number must be between 5 and 10 characters.");
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("123abc")]
    [InlineData("123-abc-456")]
    public void Should_Have_Error_When_PhoneNumber_Has_Letters(string phoneNumber)
    {
        // Arrange
        _model.SetPhoneNumber(phoneNumber);

        // Act
        var result = _validator.TestValidate(_model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Invalid phone number format.");
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("+1234567890")]
    [InlineData("123456789012345")]
    public void Should_Not_Have_Error_When_PhoneNumber_Is_Valid(string phoneNumber)
    {
        // Arrange
        _model.SetPhoneNumber(phoneNumber);

        // Act
        var result = _validator.TestValidate(_model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("1234567890123456")]
    [InlineData("+1234567890123456")]
    public void Should_Have_Error_When_PhoneNumber_Is_Too_Long(string phoneNumber)
    {
        // Arrange
        _model.SetPhoneNumber(phoneNumber);

        // Act
        var result = _validator.TestValidate(_model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Invalid phone number format.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Email_Is_Null_Or_Empty(string? email)
    {
        // Arrange
        _model.SetEmail(email);

        // Act
        var result = _validator.TestValidate(_model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Theory]
    [InlineData("invalid-email")]
    public void Should_Have_Error_When_Email_Is_Invalid(string email)
    {
        // Arrange
        _model.SetEmail(email);

        // Act
        var result = _validator.TestValidate(_model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Theory]
    [InlineData("123.456.789-10")]
    [InlineData("987.654.321-00")]
    public void Should_Not_Have_Error_When_Cpf_Is_Valid(string cpf)
    {
        // Arrange
        _model.SetCpf(cpf);

        // Act
        var result = _validator.TestValidate(_model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cpf);
    }

    [Theory]
    [InlineData("12345678910")]
    [InlineData("123.456.789-100")]
    [InlineData("123.4567.89-10")]
    public void Should_Have_Error_When_Cpf_Is_Invalid(string cpf)
    {
        // Arrange
        _model.SetCpf(cpf);

        // Act
        var result = _validator.TestValidate(_model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("Invalid CPF format.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_DateOfBirth_Is_Future()
    {
        _model.SetDateOfBirth(DateTime.Now.AddYears(1));

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("The officer must be at least 18 years old.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_DateOfJoining_Is_Future()
    {
        _model.SetDateOfJoining(DateTime.Now.AddYears(1));

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.DateOfJoining)
            .WithErrorMessage("Date of Joining cannot be in the future.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_DateOfBirth_Is_Too_Recent()
    {
        _model.SetDateOfBirth(DateTime.Now.AddYears(-17).AddDays(1));

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("The officer must be at least 18 years old.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_ERank_Is_Undefined()
    {
        _model.SetERank(ERank.Undefined);

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.ERank)
            .WithErrorMessage("Rank is required.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_EDepartment_Is_Undefined()
    {
        _model.SetEDepartment(EDepartment.Undefined);

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.EDepartment)
            .WithErrorMessage("Department is required.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_EOfficerStatus_Is_Undefined()
    {
        _model.SetEOfficerStatus(EOfficerStatus.Undefined);

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.EOfficerStatus)
            .WithErrorMessage("Officer status is required.");
    }

    [Fact]
    public void Should_Have_Validation_Error_When_EAccessLevel_Is_Undefined()
    {
        _model.SetEAccessLevel(EAccessLevel.Undefined);

        var result = _validator.TestValidate(_model);

        result.ShouldHaveValidationErrorFor(x => x.EAccessLevel)
            .WithErrorMessage("Access level is required.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Tokens_Collection_Is_Not_Null()
    {
        // Arrange
        _model.Tokens.Should().NotBeNull();

        // Act
        var result = _validator.TestValidate(_model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Tokens);
    }

    [Fact]
    public void Should_Have_Error_When_Tokens_Collection_Contains_Null_Element()
    {
        // Arrange
        _model.SetIdentificationNumber("12345");
        _model.SetName("John Doe");
        _model.SetLastName("John Doe");
        _model.SetBadgeNumber("S123456");
        _model.SetEmail("john.doe@example.com");
        _model.SetPhoneNumber("+123456789");
        _model.SetCpf("123.456.789-01");
        _model.SetDateOfBirth(DateTime.Now.AddYears(-30));
        _model.SetDateOfJoining(DateTime.Now.AddYears(-1));
        _model.SetERank(ERank.Sergeant);
        _model.SetEDepartment(EDepartment.TrafficDivision);
        _model.SetEOfficerStatus(EOfficerStatus.Active);
        _model.SetEAccessLevel(EAccessLevel.Admin);
        _model.AddToken(null!);

        // Act
        var result = _validator.TestValidate(_model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tokens)
            .WithErrorMessage("Tokens collection cannot contain null elements.");
    }
}