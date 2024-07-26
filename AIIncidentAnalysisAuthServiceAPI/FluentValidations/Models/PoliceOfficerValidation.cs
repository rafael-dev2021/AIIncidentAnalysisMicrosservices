using AIIncidentAnalysisAuthServiceAPI.Models;
using FluentValidation;

namespace AIIncidentAnalysisAuthServiceAPI.FluentValidations.Models;

public class PoliceOfficerValidation : AbstractValidator<PoliceOfficer>
{
    public PoliceOfficerValidation()
    {
        RuleFor(x => x.IdentificationNumber)!.IdentificationNumberRules();
        RuleFor(x => x.Name)!.NameRules();
        RuleFor(x => x.LastName)!.LastNameRules();
        RuleFor(x => x.BadgeNumber)!.BadgeNumberRules();
        RuleFor(x => x.Email)!.EmailRules();
        RuleFor(x => x.PhoneNumber)!.PhoneNumberRules(x => x.PhoneNumber);
        RuleFor(x => x.Cpf)!.CpfRules();
        RuleFor(x => x.DateOfBirth)!.DateOfBirthRules();
        RuleFor(x => x.DateOfJoining)!.DateOfJoiningRules();
        RuleFor(x => x.ERank)!.ERankRules();
        RuleFor(x => x.EDepartment)!.EDepartmentRules();
        RuleFor(x => x.EOfficerStatus)!.EOfficerStatusRules();
        RuleFor(x => x.EAccessLevel)!.EAccessLevelRules();
        RuleFor(x => x.Tokens)
            .Must(tokens => tokens != null && !tokens.Contains(null!))
            .WithMessage("Tokens collection cannot contain null elements."); 
    }
}