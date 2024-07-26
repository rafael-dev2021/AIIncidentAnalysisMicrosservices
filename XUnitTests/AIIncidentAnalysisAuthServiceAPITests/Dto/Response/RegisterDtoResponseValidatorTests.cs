using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Response;
using FluentValidation.TestHelper;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Dto.Response;

public class RegisterDtoResponseValidatorTests
{
    private readonly RegisterDtoResponseValidation _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Registration_Is_Successful()
    {
        // Arrange
        var result = new RegisterDtoResponse(true, string.Empty);

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Registration_Fails()
    {
        // Arrange
        var result = new RegisterDtoResponse(false, "Registration failed.");

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Should_Have_Error_When_ErrorMessage_Is_Not_Empty_On_Successful_Registration()
    {
        // Arrange
        var result = new RegisterDtoResponse(true, "Some error message");

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Message)
            .WithErrorMessage("Registration successful.");
    }

    [Fact]
    public void Should_Have_Error_When_ErrorMessage_Is_Empty_On_Failed_Registration()
    {
        // Arrange
        var result = new RegisterDtoResponse(false, string.Empty);

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Message)
            .WithErrorMessage("Registration failed.");
    }
}
