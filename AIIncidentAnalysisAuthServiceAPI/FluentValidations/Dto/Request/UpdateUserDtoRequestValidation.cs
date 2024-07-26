using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using FluentValidation;

namespace AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Request;

public class UpdateUserDtoRequestValidation : AbstractValidator<UpdateUserDtoRequest>
{
    public UpdateUserDtoRequestValidation()
    {
        RuleFor(x => x.Name)!.NameRules();
        RuleFor(x => x.LastName)!.LastNameRules();
        RuleFor(x => x.Email)!.EmailRules();
        RuleFor(x => x.PhoneNumber).PhoneNumberRules(x => x.PhoneNumber);
    }
}