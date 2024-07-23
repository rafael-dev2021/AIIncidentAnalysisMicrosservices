using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser;

public class RegisterStrategy(
    SignInManager<PoliceOfficer> signInManager,
    UserManager<PoliceOfficer> userManager,
    AppDbContext appDbContext,
    IUserValidationManagerStrategy userValidationManagerStrategy,
    IRegistrationLoggerStrategy registrationLoggerStrategy)
    : IRegisterStrategy
{
    public async Task<RegisterDtoResponse> CreateUserAsync(RegisterDtoRequest request)
    {
        registrationLoggerStrategy.LogInformation("[REGISTRATION] Attempting to register user with Email= [{Email}]",
            request.Email!);

        if (!IsPasswordConfirmed(request))
        {
            registrationLoggerStrategy.LogWarning(
                "[REGISTRATION] Password and confirm password do not match for Email= [{Email}]",
                request.Email!);

            return new RegisterDtoResponse(false, "Password and confirm password do not match.");
        }

        var validationErrors =
            await userValidationManagerStrategy.ValidateUserDetailsAsync(request.Cpf, request.Email!, request.PhoneNumber);

        if (validationErrors.Count != 0)
            return new RegisterDtoResponse(false, string.Join(", ", validationErrors));

        var appUser = CreateUser(request);

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

    private static bool IsPasswordConfirmed(RegisterDtoRequest request) =>
        request.Password == request.ConfirmPassword;

    private static PoliceOfficer CreateUser(RegisterDtoRequest request)
    {
        var appUser = new PoliceOfficer
        {
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber
        };
        appUser.SetName(request.Name);
        appUser.SetLastName(request.LastName);
        appUser.SetCpf(request.Cpf);
        appUser.SetRole("User");
        appUser.SetDateOfBirth(request.DateOfBirth);
        appUser.SetDateOfJoining(request.DateOfJoining);
        appUser.SetERank(request.ERank);
        appUser.SetEDepartment(request.EDepartment);
        appUser.SetEOfficerStatus(request.EOfficerStatus);
        appUser.SetEAccessLevel(request.EAccessLevel);

        return appUser;
    }

    private async Task AssignUserRoleAndSignInAsync(PoliceOfficer appUser)
    {
        await userManager.AddToRoleAsync(appUser, "User");
        await signInManager.SignInAsync(appUser, isPersistent: false);
    }
}