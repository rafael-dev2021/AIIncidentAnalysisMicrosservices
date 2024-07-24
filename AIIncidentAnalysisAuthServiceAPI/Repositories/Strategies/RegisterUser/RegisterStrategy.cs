using AIIncidentAnalysisAuthServiceAPI.Algorithms.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser;

public class RegisterStrategy(
    SignInManager<PoliceOfficer> signInManager,
    UserManager<PoliceOfficer> userManager,
    AppDbContext appDbContext,
    IUserValidationManagerStrategy userValidationManagerStrategy,
    IRegistrationLoggerStrategy registrationLoggerStrategy,
    IAccountNumberGenerator accountNumberGenerator)
    : IRegisterStrategy
{
    public async Task<RegisterDtoResponse> CreateUserAsync(RegisterDtoRequest request)
    {
        registrationLoggerStrategy.LogInformation("[REGISTRATION] Attempting to register user with Email= [{Email}]",
            request.Email!);

        var validationErrors =
            await userValidationManagerStrategy.ValidateUserDetailsAsync(request.Cpf, request.Email!, request.PhoneNumber);

        if (validationErrors.Count != 0)
            return new RegisterDtoResponse(false, string.Join(", ", validationErrors));

      
        var identificationNumber = await accountNumberGenerator.GenerateIdentificationNumberAsync();
        var badgeNumber = await accountNumberGenerator.GenerateBadgeNumberAsync();
        var appUser = CreateUser(request, identificationNumber, badgeNumber);

        SetUserRoleAndAccessLevel(appUser, request.ERank);

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await userManager.CreateAsync(appUser, request.Password);

            if (!result.Succeeded)
                return new RegisterDtoResponse(false, "Registration failed.");

            await AssignUserRoleAndSignInAsync(appUser);
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return new RegisterDtoResponse(false, "Registration failed due to an internal error.");
        }

        registrationLoggerStrategy.LogInformation(
            "[REGISTRATION] User registered and signed in successfully with Email= [{Email}]",
            request.Email!);

        return new RegisterDtoResponse(true, "Registration successful.");
    }

    private static PoliceOfficer CreateUser(RegisterDtoRequest request, string identificationNumber, string badgeNumber)
    {
        var appUser = new PoliceOfficer
        {
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
        };
        appUser.SetIdentificationNumber(identificationNumber);
        appUser.SetName(request.Name);
        appUser.SetLastName(request.LastName);
        appUser.SetBadgeNumber(badgeNumber);
        appUser.SetCpf(request.Cpf);
        appUser.SetRole("User"); 
        appUser.SetDateOfBirth(request.DateOfBirth);
        appUser.SetDateOfJoining(request.DateOfJoining);
        appUser.SetERank(request.ERank);
        appUser.SetEDepartment(request.EDepartment);
        appUser.SetEOfficerStatus(request.EOfficerStatus);
        appUser.SetEAccessLevel(request.EAccessLevel);

        SetUserRoleAndAccessLevel(appUser, request.ERank);

        return appUser;
    }

    private static void SetUserRoleAndAccessLevel(PoliceOfficer user, ERank rank)
    {
        switch (rank)
        {
            case ERank.Constable:
                user.SetEAccessLevel(EAccessLevel.ReadOnly);
                user.SetRole("ReadOnly");
                break;
            case ERank.Sergeant:
            case ERank.Lieutenant:
                user.SetEAccessLevel(EAccessLevel.ReadWrite);
                user.SetRole("ReadWrite");
                break;
            case ERank.Captain:
            case ERank.Inspector:
            case ERank.Chief:
                user.SetEAccessLevel(EAccessLevel.Admin);
                user.SetRole("Admin");
                break;
            case ERank.Undefined:
            default:
                user.SetEAccessLevel(EAccessLevel.Undefined);
                user.SetRole("User");
                break;
        }
    }

    private async Task AssignUserRoleAndSignInAsync(PoliceOfficer appUser)
    {
        await userManager.AddToRoleAsync(appUser, appUser.Role!);
        await signInManager.SignInAsync(appUser, isPersistent: false);
    }
}
