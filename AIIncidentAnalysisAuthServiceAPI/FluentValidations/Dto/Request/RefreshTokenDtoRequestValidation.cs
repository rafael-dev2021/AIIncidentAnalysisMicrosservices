using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using FluentValidation;

namespace AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Request;

public class RefreshTokenDtoRequestValidation : AbstractValidator<RefreshTokenDtoRequest>
{
    public RefreshTokenDtoRequestValidation()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}