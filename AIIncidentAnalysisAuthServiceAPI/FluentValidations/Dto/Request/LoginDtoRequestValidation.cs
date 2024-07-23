using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using FluentValidation;

namespace AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Request;

public class LoginDtoRequestValidation : AbstractValidator<LoginDtoRequest>
{
    public LoginDtoRequestValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");

        RuleFor(x => x.RememberMe)
            .Must(_ => true)
            .WithMessage("RememberMe must be a boolean value.");
    }
}