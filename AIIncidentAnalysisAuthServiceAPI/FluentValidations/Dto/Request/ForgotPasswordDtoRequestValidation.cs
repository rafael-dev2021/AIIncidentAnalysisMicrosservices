using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using FluentValidation;

namespace AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Request;

public class ForgotPasswordDtoRequestValidation: AbstractValidator<ForgotPasswordDtoRequest>
{
    public ForgotPasswordDtoRequestValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\S+$).{10,30}$")
            .WithMessage("Password must be between 10 and 30 characters, and include at least one digit, one lowercase letter, one uppercase letter, and one special character.");
    }
}