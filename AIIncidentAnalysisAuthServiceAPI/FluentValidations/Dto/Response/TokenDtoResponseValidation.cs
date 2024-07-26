using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using FluentValidation;

namespace AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Response;

public class TokenDtoResponseValidation : AbstractValidator<TokenDtoResponse>
{
    public TokenDtoResponseValidation()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.")
            .MinimumLength(10).WithMessage("Token must be at least 10 characters long.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .MinimumLength(10).WithMessage("Refresh token must be at least 10 characters long.");
    }
}