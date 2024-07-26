using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using FluentValidation;

namespace AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Response;

public class AuthDtoResponseValidation : AbstractValidator<AuthDtoResponse>
{
    public AuthDtoResponseValidation()
    {
        RuleFor(x => x.Success)
            .Must(_ => true)
            .WithMessage("Invalid authentication state.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Invalid email or password. Please try again.")
            .When(x => !x.Success);

        RuleFor(x => x.Message)
            .Empty().WithMessage("Login successful.")
            .When(x => x.Success);
    }
}