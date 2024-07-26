using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser.Interfaces;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser;

public class UpdateProfileStrategy(
    UserManager<PoliceOfficer> userManager,
    AppDbContext appDbContext,
    IUserDetailsValidatorStrategy userDetailsValidatorStrategy,
    IUserFieldUpdaterStrategy userFieldUpdaterStrategy,
    ITransactionHandlerStrategy transactionHandlerStrategy)
    : IUpdateProfileStrategy
{
    public async Task<UpdateDtoResponse> UpdateProfileAsync(UpdateUserDtoRequest updateUserDtoRequest, string userId)
    {
        Log.Information("[PROFILE UPDATE] Attempting to update profile for user [{UserId}]", userId);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            Log.Warning("[PROFILE UPDATE] User [{UserId}] not found.", userId);
            return new UpdateDtoResponse(false, "User not found.");
        }

        var validationErrors = await userDetailsValidatorStrategy.ValidateAsync(updateUserDtoRequest, user);
        if (validationErrors.Count != 0)
        {
            userDetailsValidatorStrategy.LogValidationErrors(userId, validationErrors);
            return new UpdateDtoResponse(false, string.Join(Environment.NewLine, validationErrors));
        }

        userFieldUpdaterStrategy.UpdateFields(user, updateUserDtoRequest);

        return await transactionHandlerStrategy.ExecuteInTransactionAsync(appDbContext, async () =>
        {
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                Log.Warning("[PROFILE UPDATE] Failed to update profile for user [{UserId}]", userId);
                return new UpdateDtoResponse(false, "Failed to update profile.");
            }

            Log.Information("[PROFILE UPDATE] Profile successfully updated for user [{UserId}]", userId);
            return new UpdateDtoResponse(true, "Profile updated successfully.");
        });
    }
}