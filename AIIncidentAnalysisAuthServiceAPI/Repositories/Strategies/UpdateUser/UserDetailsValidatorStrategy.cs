using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser;

public class UserDetailsValidatorStrategy(AppDbContext appDbContext) : IUserDetailsValidatorStrategy
{
    public async Task<List<string>> ValidateAsync(UpdateUserDtoRequest updateUserDtoRequest, PoliceOfficer user)
    {
        var validationErrors = new List<string>();

        if (updateUserDtoRequest.Email != user.Email && await IsEmailAlreadyUsedByAnotherUserAsync(updateUserDtoRequest.Email!, user.Id))
        {
            validationErrors.Add("Email already used by another user.");
        }

        if (updateUserDtoRequest.PhoneNumber != user.PhoneNumber && await IsPhoneNumberAlreadyUsedByAnotherUserAsync(updateUserDtoRequest.PhoneNumber!, user.Id))
        {
            validationErrors.Add("Phone number already used by another user.");
        }

        return validationErrors;
    }

    private async Task<bool> IsEmailAlreadyUsedByAnotherUserAsync(string email, string userId) =>
        await appDbContext.Users.AnyAsync(x => x.Email == email && x.Id != userId);

    private async Task<bool> IsPhoneNumberAlreadyUsedByAnotherUserAsync(string phone, string userId) =>
        await appDbContext.Users.AnyAsync(x => x.PhoneNumber == phone && x.Id != userId);

    public void LogValidationErrors(string userId, List<string> validationErrors)
    {
        Log.Warning("[PROFILE UPDATE] Validation errors for user [{UserId}] with message: [{Errors}]", userId, string.Join(", ", validationErrors));
    }
}