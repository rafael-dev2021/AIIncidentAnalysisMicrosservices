using System.Linq.Expressions;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using FluentValidation;

namespace AIIncidentAnalysisAuthServiceAPI.FluentValidations;

public static class CommonValidators
{
    public static void IdentificationNumberRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Identification Number is required.")
            .Length(5, 25)
            .WithMessage("Identification Number must be between 5 and 25 characters.");
    }

    public static void NameRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Name is required.")
            .Length(5, 15)
            .WithMessage("Name must be between 5 and 15 characters long.");
    }

    public static void LastNameRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Last Name is required.")
            .Length(5, 15)
            .WithMessage("Last Name must be between 5 and 15 characters long.");
    }

    public static void BadgeNumberRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Badge number is required.")
            .Length(5, 10)
            .WithMessage("Badge number must be between 5 and 10 characters.");
    }

    public static void EmailRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Email is required.")
            .Length(10, 50)
            .WithMessage("Email must be between 10 and 50 characters.")
            .EmailAddress()
            .WithMessage("Invalid email format.");
    }

    public static void PhoneNumberRules<T>(this IRuleBuilder<T, string?> ruleBuilder,
        Expression<Func<T, string?>> propertyExpression)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\+?[0-9]{1,15}$")
            .WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrWhiteSpace(propertyExpression.Compile()(x)))
            .Length(9, 15)
            .WithMessage("Phone number must be between 9 and 15 characters.");
    }

    public static void CpfRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder
            .Matches(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$")
            .WithMessage("Invalid CPF format.")
            .Length(14)
            .WithMessage("Maximum 14 characters");
    }

    public static void DateOfBirthRules<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Date of Birth is required.")
            .LessThan(DateTime.Now.AddYears(-18))
            .WithMessage("The officer must be at least 18 years old.");
    }

    public static void DateOfJoiningRules<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Date of Joining is required.")
            .LessThan(DateTime.Now)
            .WithMessage("Date of Joining cannot be in the future.");
    }

    public static void ERankRules<T>(this IRuleBuilder<T, ERank> ruleBuilder)
    {
        ruleBuilder
            .NotEqual(ERank.Undefined)
            .WithMessage("Rank is required.")
            .IsInEnum()
            .WithMessage("Invalid rank.");
    }

    public static void EDepartmentRules<T>(this IRuleBuilder<T, EDepartment> ruleBuilder)
    {
        ruleBuilder
            .NotEqual(EDepartment.Undefined)
            .WithMessage("Department is required.")
            .IsInEnum()
            .WithMessage("Invalid department.");
    }

    public static void EOfficerStatusRules<T>(this IRuleBuilder<T, EOfficerStatus> ruleBuilder)
    {
        ruleBuilder
            .NotEqual(EOfficerStatus.Undefined)
            .WithMessage("Officer status is required.")
            .IsInEnum()
            .WithMessage("Invalid officer status.");
    }

    public static void EAccessLevelRules<T>(this IRuleBuilder<T, EAccessLevel> ruleBuilder)
    {
        ruleBuilder
            .NotEqual(EAccessLevel.Undefined)
            .WithMessage("Access level is required.")
            .IsInEnum()
            .WithMessage("Invalid access level.");
    }
}