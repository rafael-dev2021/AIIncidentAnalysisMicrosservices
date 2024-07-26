using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser.Interfaces;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser;

public class AuthStrategy(
    SignInManager<PoliceOfficer> signInManager,
    ILoginAttemptsManagerStrategy loginAttemptsManagerStrategy,
    ILoggerStrategies loggerStrategies)
    : IAuthStrategy
{
    public async Task<AuthDtoResponse> AuthenticatedAsync(LoginDtoRequest request)
    {
        var loginAttemptsKey = $"{LoginAttemptsManagerStrategy.LoginAttemptsKeyPrefix}{request.Email}";
        var loginAttempts = await loginAttemptsManagerStrategy.GetLoginAttemptsAsync(loginAttemptsKey);

        loggerStrategies.LogInformation("[AUTHENTICATION] Attempting to authenticate user [{Email}]",
            request.Email!);

        if (loginAttempts >= 3)
        {
            loggerStrategies.LogWarning(
                "[AUTHENTICATION] User [{Email}] account is locked due to multiple failed attempts.",
                request.Email!);

            return new AuthDtoResponse(false, "Your account is locked. Please contact support.");
        }

        var result = await signInManager.PasswordSignInAsync(
            request.Email!,
            request.Password!,
            isPersistent: request.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            await loginAttemptsManagerStrategy.ResetLoginAttemptsAsync(loginAttemptsKey);
            loggerStrategies.LogInformation("[AUTHENTICATION] User [{Email}] successfully authenticated.",
                request.Email!);

            return new AuthDtoResponse(true, "Login successfully.");
        }

        await loginAttemptsManagerStrategy.IncrementLoginAttemptsAsync(loginAttemptsKey);

        var errorMessage = result.IsLockedOut
            ? "Your account is locked. Please contact support."
            : "Invalid email or password. Please try again.";

        Log.Warning("[AUTHENTICATION] Failed authentication attempt for user [{Email}] with message: [{ErrorMessage}]",
            request.Email, errorMessage);

        return new AuthDtoResponse(false, errorMessage);
    }
}