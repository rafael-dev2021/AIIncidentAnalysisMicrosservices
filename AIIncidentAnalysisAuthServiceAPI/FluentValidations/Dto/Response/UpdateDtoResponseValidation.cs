using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using FluentValidation;

namespace AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Response;

public class UpdateDtoResponseValidation : AbstractValidator<UpdateDtoResponse>
{
    public UpdateDtoResponseValidation()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Update failed.")
            .When(x => !x.Success);

        RuleFor(x => x.Message)
            .Empty().WithMessage("Update successful.")
            .When(x => x.Success);
    }
}