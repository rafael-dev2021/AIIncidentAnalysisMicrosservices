using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Response;
using FluentValidation.TestHelper;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Dto.Response;

public class AuthDtoResponseValidatorTests
{
    private readonly AuthDtoResponseValidation _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Authentication_Is_Successful()
    {
        // Arrange
        var result = new AuthDtoResponse(true, string.Empty);

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Authentication_Fails()
    {
        // Arrange
        var result = new AuthDtoResponse(false, "Invalid email or password. Please try again.");

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Should_Have_Error_When_ErrorMessage_Is_Not_Empty_On_Successful_Authentication()
    {
        // Arrange
        var result = new AuthDtoResponse(true, "Some error message");

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Message)
            .WithErrorMessage("Login successful.");
    }

    [Fact]
    public void Should_Have_Error_When_ErrorMessage_Is_Empty_On_Failed_Authentication()
    {
        // Arrange
        var result = new AuthDtoResponse(false, string.Empty);

        // Act
        var validationResult = _validator.TestValidate(result);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Message)
            .WithErrorMessage("Invalid email or password. Please try again.");
    }
}
