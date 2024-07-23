using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.FluentValidations.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.FluentValidations.Models;
using AIIncidentAnalysisAuthServiceAPI.Models;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class FluentValidationExtension
{
    public static void AddFluentValidationExtension(this IServiceCollection service)
    {
        service.AddValidatorsFromAssemblyContaining<PoliceOfficerValidation>();
        service.AddScoped<IValidator<PoliceOfficer>, PoliceOfficerValidation>();

        service.AddValidatorsFromAssemblyContaining<ChangePasswordDtoRequestValidation>();
        service.AddScoped<IValidator<ChangePasswordDtoRequest>, ChangePasswordDtoRequestValidation>();
        
        service.AddValidatorsFromAssemblyContaining<ForgotPasswordDtoRequestValidation>();
        service.AddScoped<IValidator<ForgotPasswordDtoRequest>, ForgotPasswordDtoRequestValidation>();
        
        service.AddValidatorsFromAssemblyContaining<LoginDtoRequestValidation>();
        service.AddScoped<IValidator<LoginDtoRequest>, LoginDtoRequestValidation>();
        
        service.AddValidatorsFromAssemblyContaining<RegisterDtoRequestValidation>();
        service.AddScoped<IValidator<RegisterDtoRequest>, RegisterDtoRequestValidation>();
        
        service.AddValidatorsFromAssemblyContaining<UpdateUserDtoRequestValidation>();
        service.AddScoped<IValidator<UpdateUserDtoRequest>, UpdateUserDtoRequestValidation>();
        
        service.AddValidatorsFromAssemblyContaining<AuthDtoResponseValidation>();
        service.AddScoped<IValidator<AuthDtoResponse>, AuthDtoResponseValidation>();
        
        service.AddValidatorsFromAssemblyContaining<RegisterDtoResponseValidation>();
        service.AddScoped<IValidator<RegisterDtoResponse>, RegisterDtoResponseValidation>();
        
        service.AddValidatorsFromAssemblyContaining<UpdateDtoResponseValidation>();
        service.AddScoped<IValidator<UpdateDtoResponse>, UpdateDtoResponseValidation>();
        
        service.AddValidatorsFromAssemblyContaining<RefreshTokenDtoRequestValidation>();
        service.AddScoped<IValidator<RefreshTokenDtoRequest>, RefreshTokenDtoRequestValidation>();
        
        service.AddValidatorsFromAssemblyContaining<TokenDtoResponseValidation>();
        service.AddScoped<IValidator<TokenDtoResponse>, TokenDtoResponseValidation>();

        service.AddFluentValidationAutoValidation();
        service.AddFluentValidationClientsideAdapters();
    }
}