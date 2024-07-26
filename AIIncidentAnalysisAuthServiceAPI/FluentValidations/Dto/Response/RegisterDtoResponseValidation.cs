using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using FluentValidation;

namespace AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Response;

public class RegisterDtoResponseValidation : AbstractValidator<RegisterDtoResponse>
{
    public RegisterDtoResponseValidation()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Registration failed.")
            .When(x => !x.Success);

        RuleFor(x => x.Message)
            .Empty().WithMessage("Registration successful.")
            .When(x => x.Success);
    }
}