using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories;

public class AuthRepository(
    SignInManager<PoliceOfficer> signInManager,
    UserManager<PoliceOfficer> userManager,
    IAuthStrategy authStrategy,
    IRegisterStrategy registerStrategy,
    IUpdateProfileStrategy updateProfileStrategy,
    AppDbContext appDbContext) : IAuthRepository
{
    public async Task<IEnumerable<PoliceOfficer>> ListPoliceOfficersAsync()
    {
        var users = await appDbContext.Users
            .AsNoTracking()
            .ToListAsync();

        var usersWithRoles = new List<PoliceOfficer>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            var userWithRole = new PoliceOfficer
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            userWithRole.SetIdentificationNumber(user.IdentificationNumber);
            userWithRole.SetName(user.Name);
            userWithRole.SetLastName(user.LastName);
            userWithRole.SetCpf(user.Cpf);
            userWithRole.SetBadgeNumber(user.BadgeNumber);
            userWithRole.SetRole(roles.FirstOrDefault());
            userWithRole.SetDateOfBirth(user.DateOfBirth);
            userWithRole.SetDateOfJoining(user.DateOfJoining);
            userWithRole.SetERank(user.ERank);
            userWithRole.SetEDepartment(user.EDepartment);
            userWithRole.SetEOfficerStatus(user.EOfficerStatus);
            userWithRole.SetEAccessLevel(user.EAccessLevel);

            usersWithRoles.Add(userWithRole);
        }

        return usersWithRoles;
    }

    public async Task<AuthDtoResponse> AuthenticateAsync(LoginDtoRequest loginDtoRequest)
    {
        return await authStrategy.AuthenticatedAsync(loginDtoRequest);
    }

    public async Task<RegisterDtoResponse> RegisterAsync(RegisterDtoRequest registerDtoRequest)
    {
        return await registerStrategy.CreateUserAsync(registerDtoRequest);
    }

    public async Task<UpdateDtoResponse> UpdateAsync(UpdateUserDtoRequest updateUserDtoRequest, string userId)
    {
        return await updateProfileStrategy.UpdateProfileAsync(updateUserDtoRequest, userId);
    }

    public async Task<PoliceOfficer?> GetUserProfileAsync(string? userEmail)
    {
        var user = await userManager.FindByEmailAsync(userEmail!);
        return user ?? null;
    }

    public async Task<PoliceOfficer?> GetUserIdProfileAsync(string? userId)
    {
        var user = await userManager.FindByIdAsync(userId!);
        return user ?? null;
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordDtoRequest changePasswordDtoRequest)
    {
        var user = await userManager.FindByEmailAsync(changePasswordDtoRequest.Email!);
        if (user == null) return false;

        var changePasswordResult =
            await userManager.ChangePasswordAsync(
                user,
                changePasswordDtoRequest.CurrentPassword!,
                changePasswordDtoRequest.NewPassword!);

        return changePasswordResult.Succeeded;
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDtoRequest forgotPasswordDtoRequest)
    {
        var user = await userManager.FindByEmailAsync(forgotPasswordDtoRequest.Email!);
        if (user == null) return false;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetPasswordResult =
            await userManager.ResetPasswordAsync(user, token, forgotPasswordDtoRequest.NewPassword!);

        return resetPasswordResult.Succeeded;
    }

    public async Task SaveUserAsync(PoliceOfficer policeOfficer)
    {
        appDbContext.Update(policeOfficer);
        await appDbContext.SaveChangesAsync();
    }

    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }
}